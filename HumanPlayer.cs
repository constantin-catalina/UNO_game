using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_game
{
    // Represents a human player in the UNO game
    class HumanPlayer : Player
    {
        // Manages the human player's turn
        public override PlayerTurn PlayTurn(PlayerTurn currentTurn, CardDeck drawPile)
        {
            // Display the current top card and the player's hand
            Console.WriteLine($"\nThe current top card is: {currentTurn.Card.DisplayValue}");
            Console.WriteLine("Your hand:");
            ShowHand();

            PlayerTurn turn = new PlayerTurn();

            // If it's an attack turn, process the attack
            if (IsAttackTurn(currentTurn))
            {
                return ProcessAttack(currentTurn.Card, currentTurn.DeclaredColor, drawPile);
            }

            while (true)
            {
                // Ask the player for input on which card to play or if they want to draw
                Console.WriteLine("Enter the index of the card to play or 'draw' to draw a card:");
                string input = Console.ReadLine() ?? throw new InvalidOperationException("Input cannot be null.");

                if (input.ToLower() == "draw")
                {
                    turn = DrawCard(currentTurn, drawPile);
                    break;
                }
                else
                {
                    // Try to parse the input and select a card to play
                    if (int.TryParse(input, out int cardIndex) && cardIndex > 0 && cardIndex <= Hand.Count)
                    {
                        cardIndex--;
                        var selectedCard = Hand[cardIndex];

                        // Check if the selected card can be played
                        if (CanPlayCard(selectedCard, currentTurn))
                        {
                            HandleCards(selectedCard, ref turn);
                            turn.Card = selectedCard;
                            Hand.RemoveAt(cardIndex);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("You cannot play that card. Please select a different card or draw.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. You must enter a valid card index or 'draw'.");
                    }
                }
            }

            // Display the turn result
            DisplayTurn(turn);

            return turn;
        }

        // Check if the chosen card can be played
        private bool CanPlayCard(Card selectedCard, PlayerTurn currentTurn)
        {
            return selectedCard.Color == currentTurn.DeclaredColor ||
                   selectedCard.Value == currentTurn.Card.Value ||
                   selectedCard.Color == currentTurn.Card.Color ||
                   selectedCard.Color == CardColor.Wild;
        }

        // Handles special card effects like Wild, DrawTwo, etc.
        private void HandleCards(Card selectedCard, ref PlayerTurn turn)
        {
            if (selectedCard.Value == CardValue.DrawFour)
            {
                turn.Result = TurnResult.WildDrawFour;
                turn.DeclaredColor = AskForColor();
            }
            else if (selectedCard.Value == CardValue.ChangeColor)
            {
                turn.Result = TurnResult.WildCard;
                turn.DeclaredColor = AskForColor();
            }
            else if (selectedCard.Value == CardValue.Reverse)
            {
                turn.Result = TurnResult.Reversed;
            }
            else if (selectedCard.Value == CardValue.Skip)
            {
                turn.Result = TurnResult.Skip;
            }
            else if (selectedCard.Value == CardValue.DrawTwo)
            {
                turn.Result = TurnResult.DrawTwo;
            }
            else
            {
                turn.Result = TurnResult.PlayedCard;
            }
        }

        // Checks if the previous turn was an attack turn
        private bool IsAttackTurn(PlayerTurn previousTurn)
        {
            return previousTurn.Result == TurnResult.Skip || previousTurn.Result == TurnResult.DrawTwo || previousTurn.Result == TurnResult.WildDrawFour;
        }

        // Asks the player to choose a new color for Wild or DrawFour cards
        private CardColor AskForColor()
        {
            Console.WriteLine("Choose a new color (Red, Blue, Green, Yellow):");
            while (true)
            {
                string colorInput = Console.ReadLine() ?? throw new InvalidOperationException("Color input cannot be null.");
                colorInput = colorInput.ToLower();

                switch (colorInput)
                {
                    case "red": return CardColor.Red;
                    case "blue": return CardColor.Blue;
                    case "green": return CardColor.Green;
                    case "yellow": return CardColor.Yellow;
                    default:
                        Console.WriteLine("Invalid color. Please choose Red, Blue, Green, or Yellow.");
                        break;
                }
            }
        }

        // Draws a card and checks if it can be played immediately
        protected PlayerTurn DrawCard(PlayerTurn previousTurn, CardDeck drawPile)
        {
            PlayerTurn turn = new PlayerTurn();
            var drawnCard = drawPile.Draw(1);
            Hand.Add(drawnCard.First());

            if (HasMatch(previousTurn.Card))
            {
                turn = PlayMatchingCard(previousTurn.Card);
                turn.Result = TurnResult.ForceDrawPlay;
            }
            else
            {
                turn.Result = TurnResult.ForceDraw;
                turn.Card = previousTurn.Card;
                turn.DeclaredColor = previousTurn.DeclaredColor;
            }

            return turn;
        }

        // Displays the player's hand
        public override void ShowHand()
        {
            int i = 1;
            SortHand();
            Console.WriteLine("Player " + Position + "'s Hand: ");

            foreach (var card in Hand)
            {
                Console.Write(i + ") " + Enum.GetName(typeof(CardColor), card.Color) + " " + Enum.GetName(typeof(CardValue), card.Value) + "  ");
                i++;
            }
            Console.WriteLine();
        }

        // Plays a card that matches the current discard pile card
        private PlayerTurn PlayMatchingCard(Card currentDiscard)
        {
            PlayerTurn turn = new PlayerTurn();
            turn.Result = TurnResult.PlayedCard;
            var matching = Hand.Where(x => x.Color == currentDiscard.Color || x.Value == currentDiscard.Value || x.Color == CardColor.Wild).ToList();

            if (matching.Any(x => x.Value == CardValue.DrawFour))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawFour);
                turn.DeclaredColor = AskForColor();
                turn.Result = TurnResult.WildDrawFour;
                Hand.Remove(turn.Card);

                return turn;
            }
            if (matching.Any(x => x.Value == CardValue.DrawTwo))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawTwo);
                turn.Result = TurnResult.DrawTwo;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }
            if (matching.Any(x => x.Value == CardValue.Skip))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Skip);
                turn.Result = TurnResult.Skip;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }
            if (matching.Any(x => x.Value == CardValue.Reverse))
            {
                turn.Card = matching.First(x => x.Value == CardValue.Reverse);
                turn.Result = TurnResult.Reversed;
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(turn.Card);

                return turn;
            }

            var matchOnColor = matching.Where(x => x.Color == currentDiscard.Color);
            var matchOnValue = matching.Where(x => x.Value == currentDiscard.Value);
            if (matchOnColor.Any() && matchOnValue.Any())
            {
                var correspondingColor = Hand.Where(x => x.Color == matchOnColor.First().Color);
                var correspondingValue = Hand.Where(x => x.Value == matchOnValue.First().Value);
                if (correspondingColor.Count() >= correspondingValue.Count())
                {
                    turn.Card = matchOnColor.First();
                    turn.DeclaredColor = turn.Card.Color;
                    Hand.Remove(matchOnColor.First());

                    return turn;
                }
                else
                {
                    turn.Card = matchOnValue.First();
                    turn.DeclaredColor = turn.Card.Color;
                    Hand.Remove(matchOnValue.First());

                    return turn;
                }
            }
            else if (matchOnColor.Any())
            {
                turn.Card = matchOnColor.First();
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(matchOnColor.First());

                return turn;
            }
            else if (matchOnValue.Any())
            {
                turn.Card = matchOnValue.First();
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(matchOnValue.First());

                return turn;
            }

            if (matching.Any(x => x.Value == CardValue.ChangeColor))
            {
                turn.Card = matching.First(x => x.Value == CardValue.ChangeColor);
                turn.DeclaredColor = AskForColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(turn.Card);

                return turn;
            }

            turn.Result = TurnResult.ForceDraw;
            return turn;
        }
    }
}
