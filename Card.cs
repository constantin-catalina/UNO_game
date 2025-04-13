using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_game
{
    // Enum to define the possible colors of an UNO card
    public enum CardColor
    { 
        Red,
        Blue,
        Yellow,
        Green,
        Wild // Wild represents cards that aren't tied to a specific color
    }

    // Enum to define the possible values of an UNO card
    public enum CardValue
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Reverse,       // Reverse the turn order in the game.
        Skip,          // Skip the next player's turn.
        DrawTwo,       // Force the next player to draw two cards.
        DrawFour,      // Wild card that forces the next player to draw four cards.
        ChangeColor    // Wild card that allows the player to change the color in play.
    }
    public class Card
    {
        public CardColor Color { get; set; }
        public CardValue Value { get; set; }
        public int Score { get; set; } // Property to store the score associated with the card
        // Property to display the card's value in a readable format
        public string DisplayValue
        {
            get
            {
                // If the card is a ChangeColor card, display only its value
                if (Value == CardValue.ChangeColor)
                {
                    return Value.ToString();
                }
                // Otherwise, display both the card's color and value
                return Color.ToString() + " " + Value.ToString();
            }
        }
    }
}
