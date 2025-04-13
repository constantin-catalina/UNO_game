using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_game
{

    // Enum representing the possible outcomes of a player's turn
    public enum TurnResult
    {
        GameStart,          // The start of the game 
        PlayedCard,         // The player has successfully played a card
        Skip,               // The player played a Skip card
        DrawTwo,            // The player played a Draw Two card
        Attacked,           // The player has been attacked
        ForceDraw,          // The player is forced to draw cards
        ForceDrawPlay,      // The player has to draw and then play a card
        WildCard,           // The player played a Wild Change Color card
        WildDrawFour,       // The player played a Wild Draw Four card
        Reversed            // The player played a Reverse card
    }
    public class PlayerTurn
    {
        public Card Card { get; set; }                  // The card played by the player    
        public CardColor DeclaredColor { get; set; }    // The color declared by the player (used for Wild Cards)
        public TurnResult Result { get; set; }          // The result of the turn
    }
}
