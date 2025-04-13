using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_game
{
    public class CardDeck
    {
        // List of all cards that make up the deck
        public List<Card> Cards {  get; set; }

        // Default constructor to create a standard UNO deck
        public CardDeck() 
        { 
            Cards = new List<Card>();

            // Loop through all possible card colors and add the coresponding amount of cards
            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                if (color != CardColor.Wild)
                {
                    foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                    {
                        switch(value)
                        {
                            case CardValue.Zero:
                                Cards.Add(new Card()
                                {
                                    Color = color,
                                    Value = value,
                                    Score = 0
                                });
                                break;
                            case CardValue.One:
                            case CardValue.Two:
                            case CardValue.Three:
                            case CardValue.Four:
                            case CardValue.Five:
                            case CardValue.Six:
                            case CardValue.Seven:
                            case CardValue.Eight:
                            case CardValue.Nine:
                                for (int i = 0; i < 2; i++)
                                {
                                    Cards.Add(new Card()
                                    {
                                        Color = color,
                                        Value = value,
                                        Score = (int)value
                                    });
                                }
                                break;
                            case CardValue.Skip:
                            case CardValue.Reverse:
                            case CardValue.DrawTwo:
                                for (int i = 0; i < 2; i++)
                                {
                                    Cards.Add(new Card()
                                    {
                                        Color = color,
                                        Value = value,
                                        Score = 20
                                    });
                                }
                                break;
                        }
                    }
                }
                else 
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Cards.Add(new Card()
                        {
                            Color = color,
                            Value = CardValue.DrawFour,
                            Score = 50
                        });
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        Cards.Add(new Card()
                        {
                            Color = color,
                            Value = CardValue.ChangeColor,
                            Score = 50
                        });
                    }
                }
            }
        }

        // Constructor to create a deck from an existing list of cards
        public CardDeck(List<Card> cards)
        {
            Cards = cards;
        }

        // Method to shuffle the cards that are in a deck
        public void Shuffle() 
        {
            Random random = new Random();
            List<Card> cards = Cards;

            for (int i = cards.Count - 1; i > 0; --i)
            {
                // Generate a random index within the range [0, i]
                int aux = random.Next(i + 1);

                // Swap the current card with the card at the random index
                Card temp = cards[i];
                cards[i] = cards[aux];
                cards[aux] = temp;
            }
        }

        // Method to draw a specified number of cards from the deck
        public List<Card> Draw (int count) 
        {
            // Get the first 'count' number of cards from the deck
            List<Card> drawnCards = Cards.Take(count).ToList();

            // Remove the drawn cards from the deck
            Cards.RemoveAll(x => drawnCards.Contains(x));

            // Return the drawn cards
            return drawnCards;
        }
    }
}
