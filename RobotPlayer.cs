using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_game
{
    // The RobotPlayer class inherits from Player and implements the logic for an AI player in the UNO game
    class RobotPlayer : Player
    {
        // Executes the robot's turn based on the previous turn's outcome
        public override PlayerTurn PlayTurn(PlayerTurn previousTurn, CardDeck drawPile) 
        {
            PlayerTurn turn = new PlayerTurn();

            // Check if the previous turn resulted in an attack (Skip, DrawTwo, WildDrawFour)
            if (previousTurn.Result == TurnResult.Skip || previousTurn.Result == TurnResult.DrawTwo || previousTurn.Result == TurnResult.WildDrawFour)
            {
                return ProcessAttack(previousTurn.Card, previousTurn.DeclaredColor, drawPile);
            }
            // If previous turn played a WildCard, was attacked, or forced a draw, check if there's a matching card by color
            else if ((previousTurn.Result == TurnResult.WildCard || previousTurn.Result == TurnResult.Attacked || previousTurn.Result == TurnResult.ForceDraw) && HasMatch(previousTurn.DeclaredColor))
            {
                turn = PlayMatchingCard(previousTurn.DeclaredColor);
            }
            // If a card matches the current discard card, play it
            else if (HasMatch(previousTurn.Card))
            {
                turn = PlayMatchingCard(previousTurn.Card);
            }
            // If no matching card, draw a card
            else
            {
                turn = DrawCard(previousTurn, drawPile);
            }

            // Display the robot's turn details
            DisplayTurn(turn);
            return turn;
        }

        // Handles drawing a card and playing it if it matches the discard pile
        protected PlayerTurn DrawCard(PlayerTurn previousTurn, CardDeck drawPile) 
        {
            PlayerTurn turn = new PlayerTurn();
            var drawnCard = drawPile.Draw(1);
            Hand.AddRange(drawnCard);

            // Check if the drawn card matches the discard pile
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

        // Plays a card from the hand that matches a given card color
        private PlayerTurn PlayMatchingCard(CardColor color)
        { 
            PlayerTurn turn = new PlayerTurn();
            turn.Result = TurnResult.PlayedCard;

            // Find cards in the hand that match the given color or are Wild cards
            var matching = Hand.Where(x => x.Color == color || x.Color == CardColor.Wild).ToList();

            // If a Draw Four card is available, play it and select a dominant color
            if (matching.Any(x => x.Value == CardValue.DrawFour))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawFour);
                turn.DeclaredColor = SelectDominantColor();
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

            // Play a regular matching card by color
            var matchOnColor = matching.Where(x => x.Color == color);
            if (matchOnColor.Any())
            {
                turn.Card = matchOnColor.First();
                turn.DeclaredColor = turn.Card.Color;
                Hand.Remove(matchOnColor.First());

                return turn;
            }
            // If no color match, check for ChangeColor (WildCard) and play it
            if (matching.Any(x => x.Value == CardValue.ChangeColor))
            {
                turn.Card = matching.First(x => x.Value == CardValue.ChangeColor);
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(turn.Card);

                return turn;
            }

            // If no cards can be played, the turn results in a forced draw
            turn.Result = TurnResult.ForceDraw;
            return turn;
        }

        // Plays a card from the hand that matches a given card (by color or value).
        private PlayerTurn PlayMatchingCard(Card currentDiscard)
        {
            PlayerTurn turn = new PlayerTurn();
            turn.Result = TurnResult.PlayedCard;
            // Find cards in the hand that match the discard pile's color, value, or are Wild cards
            var matching = Hand.Where(x => x.Color == currentDiscard.Color || x.Value == currentDiscard.Value || x.Color == CardColor.Wild).ToList();

            if (matching.Any(x => x.Value == CardValue.DrawFour))
            {
                turn.Card = matching.First(x => x.Value == CardValue.DrawFour);
                turn.DeclaredColor = SelectDominantColor();
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

            // Determine whether to match by color or value
            var matchOnColor = matching.Where(x => x.Color == currentDiscard.Color);
            var matchOnValue = matching.Where(x => x.Value == currentDiscard.Value);
            if (matchOnColor.Any() && matchOnValue.Any())
            {
                // Choose between matching by color or value based on card frequency in hand
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

            // If a ChangeColor card is available, play it
            if (matching.Any(x => x.Value == CardValue.ChangeColor))
            {
                turn.Card = matching.First(x => x.Value == CardValue.ChangeColor);
                turn.DeclaredColor = SelectDominantColor();
                turn.Result = TurnResult.WildCard;
                Hand.Remove(turn.Card);

                return turn;
            }

            // If no match is found, the result is a forced draw
            turn.Result = TurnResult.ForceDraw;
            return turn;
        }

        // Selects the most frequent color in the hand to declare
        private CardColor SelectDominantColor() 
        {
            // Exclude Wild cards from the selection
            var validColors = Hand.Where(x => x.Color != CardColor.Wild);

            // If only Wild cards are present, select a random color
            if (!validColors.Any())
            {
                var random = new Random();
                var allColors = Enum.GetValues(typeof(CardColor))
                                    .Cast<CardColor>()
                                    .Where(color => color != CardColor.Wild)
                                    .ToList();

                return allColors[random.Next(allColors.Count)];
            }

            // Group by color and select the most frequent one
            var colors = validColors.GroupBy(x => x.Color).OrderByDescending(x => x.Count());
            return colors.First().Key;
        }

        // Displays the robot's hand in the console
        public override void ShowHand()
        {
            SortHand();
            Console.WriteLine("Player " + Position + "'s Hand: ");

            foreach (var card in Hand)
            {
                Console.Write(Enum.GetName(typeof(CardColor), card.Color) + " " + Enum.GetName(typeof(CardValue), card.Value) + "  ");
            }
            Console.WriteLine();
        }
    }
}
