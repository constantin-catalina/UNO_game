using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_game
{
    // Manages the overall game state, player actions, and game flow
    public class GameManager
    {
        // List of players in the game
        public List<Player> Players { get; set; }

        // The deck of cards used for drawing
        public CardDeck DrawPile { get; set; }

        // The pile of cards that have been discarded
        public List<Card> DiscardPile { get; set; }
        
        // Constructor to initialize the game with a given number of players
        public GameManager(int numPlayers, bool playingPlayer)
        {
            Players = new List<Player>();
            DrawPile = new CardDeck();
            // Shuffle the draw pile at the start of the game
            DrawPile.Shuffle();

            // Add players to the game
            AddPlayers(numPlayers, playingPlayer);

            // Deal 7 cards to each player
            int maxCards = 7 * Players.Count;
            int dealtCards = 0;

            // Distribute the cards evenly among the players
            while (dealtCards < maxCards)
            {
                for (int i = 0; i < numPlayers; i++)
                {
                    Players[i].Hand.Add(DrawPile.Cards.First());
                    DrawPile.Cards.RemoveAt(0);
                    dealtCards++;
                }
            }

            // Initialize the discard pile with the first card from the draw pile
            DiscardPile = new List<Card>();
            DiscardPile.Add(DrawPile.Cards.First());
            DrawPile.Cards.RemoveAt(0);

            // Ensure the first card is not a wild card
            while (DiscardPile.First().Value == CardValue.ChangeColor || DiscardPile.First().Value == CardValue.DrawFour)
            {
                DiscardPile.Insert(0, DrawPile.Cards.First());
                DrawPile.Cards.RemoveAt(0);
            }
        }

        // Adds players to the game, including a human or robot players based on the input flag
        private void AddPlayers(int numPlayers, bool playingPlayer)
        {
            if (playingPlayer)
            {
                Players.Add(new HumanPlayer() { Position = 1 });
            }
            else
            {
                Players.Add(new RobotPlayer() { Position = 1 });
            }

            // Add the rest of the players as robot players
            for (int i = 2; i <= numPlayers; i++)
            {
                Players.Add(new RobotPlayer() { Position = i });
            }
        }

        // Adds the played card to the discard pile if the turn was successful
        private void AddToDiscardPile(PlayerTurn currentTurn)
        {
            if (currentTurn.Result == TurnResult.PlayedCard
                    || currentTurn.Result == TurnResult.DrawTwo
                    || currentTurn.Result == TurnResult.Skip
                    || currentTurn.Result == TurnResult.WildCard
                    || currentTurn.Result == TurnResult.WildDrawFour
                    || currentTurn.Result == TurnResult.Reversed)
            {
                DiscardPile.Insert(0, currentTurn.Card);
            }
        }

        // Resets players' hands and reinitializes the deck and discard pile for a new round
        private void ResetHands(int numPlayers)
        {
            // Clear each player's hand
            foreach (var player in Players)
            {
                player.Hand.Clear();
            }

            // Reinitialize and shuffle the draw pile
            DrawPile = new CardDeck();
            DrawPile.Shuffle();

            // Deal new cards to each player
            int maxCards = 7 * Players.Count;
            int dealtCards = 0;

            while (dealtCards < maxCards)
            {
                for (int i = 0; i < numPlayers; i++)
                {
                    Players[i].Hand.Add(DrawPile.Cards.First());
                    DrawPile.Cards.RemoveAt(0);
                    dealtCards++;
                }
            }

            // Set up the discard pile again
            DiscardPile = new List<Card>();
            DiscardPile.Add(DrawPile.Cards.First());
            DrawPile.Cards.RemoveAt(0);

            // Ensure the first card is not a wild card
            while (DiscardPile.First().Value == CardValue.ChangeColor || DiscardPile.First().Value == CardValue.DrawFour)
            {
                DiscardPile.Insert(0, DrawPile.Cards.First());
                DrawPile.Cards.RemoveAt(0);
            }
        }

        // The main game loop that runs until one player reaches 500 points
        public void PlayGame(int numPlayers, bool playingPlayer)
        {
            while (!Players.Any(player => player.Points >= 500))
            {
                Console.WriteLine("\nStarting a new round...");

                int i = 0;
                bool isAscending = true;

                // Show hands if there are no human players
                if (playingPlayer == false)
                {
                    foreach (Player player in Players)
                    {
                        player.ShowHand();
                    }
                }

                Console.WriteLine("\nPress ENTER to START the round!");
                Console.ReadLine();

                // Initialize the first turn with the first card from the discard pile
                PlayerTurn currentTurn = new PlayerTurn()
                {
                    Result = TurnResult.GameStart,
                    Card = DiscardPile.First(),
                    DeclaredColor = DiscardPile.First().Color
                };

                Console.WriteLine("First card is a " + currentTurn.Card.DisplayValue + ".");

                // Main game loop until a player has no cards left
                while (!Players.Any(x => !x.Hand.Any()))
                {
                    // Shuffle the discard pile into the draw pile if it gets too low
                    if (DrawPile.Cards.Count < 4)
                    {
                        var currentCard = DiscardPile.First();

                        DrawPile.Cards = DiscardPile.Skip(1).ToList();
                        DrawPile.Shuffle();

                        DiscardPile = new List<Card>();
                        DiscardPile.Add(currentCard);

                        Console.WriteLine("Shuffling cards!");
                    }

                    // Process the current player's turn
                    var currentPlayer = Players[i];

                    currentTurn = currentPlayer.PlayTurn(currentTurn, DrawPile);

                    AddToDiscardPile(currentTurn);

                    // Reverse the play direction if a Reverse card is played
                    if (currentTurn.Result == TurnResult.Reversed)
                    {
                        if(numPlayers == 2)
                        {
                            continue;
                        }
                        else
                        {
                            isAscending = !isAscending;
                        }
                    }

                    // Move to the next player in the appropriate direction
                    if (isAscending)
                    {
                        i++;
                        if (i >= Players.Count)
                        {
                            i = 0;
                        }
                    }
                    else
                    {
                        i--;
                        if (i < 0)
                        {
                            i = Players.Count - 1;
                        }
                    }

                }

                // Identify the winning player
                var winningPlayer = Players.Where(x => !x.Hand.Any()).First();

                int totalPoints = 0;

                // Calculate points from other players' remaining cards
                foreach (var player in Players)
                {
                    totalPoints += player.Hand.Sum(x => x.Score);
                }

                // Update the winning player's points
                winningPlayer.UpdatePoints(totalPoints);
                Console.WriteLine("\nTHE ROUND IS OVER!!");
                Console.WriteLine("Player " + winningPlayer.Position.ToString() + " won " + totalPoints + " points this round!!");

                // Check for game-over condition
                if (winningPlayer.Points >= 500)
                {
                    Console.WriteLine("\nGAME OVER!!");
                    Console.WriteLine($"Player {winningPlayer.Position} has reached 500 points and wins the game!");
                    break;
                }
                else
                {
                    // Display current scores
                    Console.WriteLine("\n----- TOTAL SCORE -----");
                    foreach (var player in Players)
                    {
                        Console.WriteLine(player.Position.ToString() + ": " + player.Points + " points");
                    }
                }

                // Reset hands and deck for the next round
                ResetHands(numPlayers);
            }
        }
    }
}
