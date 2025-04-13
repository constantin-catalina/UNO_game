using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_game
{
    // Abstract base class representing a player in the UNO game
    public abstract class Player
    {
        // List of cards in the player's hand
        public List<Card> Hand { get; set; }

        // The position of the player in the game (e.g., 1st, 2nd, etc.)
        public int Position { get; set; }

        // The total points accumulated by the player
        public int Points { get; private set; }

        // Constructor initializing the player's hand and points
        public Player()
        {
            Hand = new List<Card>();
            Points = 0;
        }

        // Updates the player's points based on the points earned in the current round
        public void UpdatePoints(int roundPoints)
        {
            Points += roundPoints;
        }

        // Abstract method to be implemented by subclasses to define how a player plays their turn
        public abstract PlayerTurn PlayTurn(PlayerTurn previousTurn, CardDeck drawPile);

        // Handles the effects of an attack card being played (e.g., Draw Two, Draw Four)
        protected PlayerTurn ProcessAttack(Card currentDiscard, CardColor previousColor, CardDeck drawPile)
        {
            PlayerTurn turn = new PlayerTurn();
            turn.Result = TurnResult.Attacked;

            turn.Card = currentDiscard;

            // Set the declared color for special cards like Draw Four
            if (currentDiscard.Value == CardValue.DrawFour)
            {
                turn.DeclaredColor = previousColor;
            }
            else
            {
                turn.DeclaredColor = currentDiscard.Color;
            }

            // Handle attack for Skip cards
            if (currentDiscard.Value == CardValue.Skip)
            {
                Console.WriteLine("Player " + Position.ToString() + " was skipped!");
            }
            // Handle attack for Draw Two cards
            else if (currentDiscard.Value == CardValue.DrawTwo)
            {
                Console.WriteLine("Player " + Position.ToString() + " must draw two cards!");
                Hand.AddRange(drawPile.Draw(2));
            }
            // Handle attack for Wild Draw Four cards
            else if (currentDiscard.Value == CardValue.DrawFour)
            {
                Console.WriteLine("Player " + Position.ToString() + " must draw four cards!");
                Hand.AddRange(drawPile.Draw(4));
            }
            return turn;
        }

        // Displays information about the player's current turn
        protected void DisplayTurn(PlayerTurn currentTurn)
        {
            if (currentTurn.Result == TurnResult.ForceDraw)
            {
                Console.WriteLine("Player " + Position.ToString() + " is forced to draw.");
            }
            if (currentTurn.Result == TurnResult.ForceDrawPlay)
            {
                Console.WriteLine("Player " + Position.ToString() + " is forced to draw AND can play the drawn card!");
            }
            if (currentTurn.Result == TurnResult.PlayedCard || currentTurn.Result == TurnResult.Skip || currentTurn.Result == TurnResult.DrawTwo
                                                            || currentTurn.Result == TurnResult.WildCard || currentTurn.Result == TurnResult.WildDrawFour
                                                            || currentTurn.Result == TurnResult.Reversed || currentTurn.Result == TurnResult.ForceDrawPlay)
            {
                Console.WriteLine("Player " + Position.ToString() + " plays a " + currentTurn.Card.DisplayValue + " card.");
                if (currentTurn.Card.Color == CardColor.Wild)
                {
                    Console.WriteLine("Player " + Position.ToString() + " declares " + currentTurn.DeclaredColor.ToString() + " as the new color.");
                }
                if (currentTurn.Result == TurnResult.Reversed)
                {
                    Console.WriteLine("Turn order reversed!");
                }
            }

            // Player shouts "Uno!" when they have only one card left
            if (Hand.Count == 1)
            {
                Console.WriteLine("Player " + Position.ToString() + " shouts Uno!");
            }
        }

        // Checks if the player has a card in their hand that matches the given card (by color or value)
        protected bool HasMatch(Card card)
        {
            if (card == null) return false;
            return Hand.Any(x => x.Color == card.Color || x.Value == card.Value || x.Color == CardColor.Wild);
        }

        // Checks if the player has a card in their hand that matches the given color
        protected bool HasMatch(CardColor color)
        {
            return Hand.Any(x => x.Color == color || x.Color == CardColor.Wild);
        }

        // Sorts the player's hand by color and then by value for easier display and strategy
        protected void SortHand()
        {
            Hand = Hand.OrderBy(x => x.Color).ThenBy(x => x.Value).ToList();
        }

        // Abstract method to display the player's hand, to be implemented by subclasses
        public abstract void ShowHand();
    }
}
