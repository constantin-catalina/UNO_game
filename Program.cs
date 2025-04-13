using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UNO_game
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("A new UNO Game is starting...");
                
                Console.WriteLine("Enter the number of players: ");
                int noOfPlayers = int.Parse(Console.ReadLine());

                // Check if the number of players is within the valid range (2 to 10)
                if (noOfPlayers < 2 || noOfPlayers > 10)
                {
                    throw new ArgumentOutOfRangeException(nameof(noOfPlayers), "The number of players must be between 2 and 10.");
                }

                // Prompt the user to choose whether they want to play or watch the game
                Console.WriteLine("Do you to play or watch the game?");
                Console.WriteLine("Press 1 - to play\nPress 2 - to watch");
                int option = int.Parse(Console.ReadLine());
                bool playingPlayer;

                // Determine the mode based on the user's input
                if (option == 1)
                {
                    playingPlayer = true; // The user chooses to play
                }
                else if (option == 2)
                {
                     playingPlayer= false; // The user chooses to watch
                }
                else
                {
                    // Throw an exception if the input is not 1 or 2
                    throw new ArgumentOutOfRangeException(nameof(option), "You must choose between option 1 or 2.");
                }

                // Initialize the GameManager with the specified number of players
                GameManager manager = new GameManager(noOfPlayers, playingPlayer);

                // Start the game with the chosen settings (play or watch)
                manager.PlayGame(noOfPlayers, playingPlayer);

                Console.ReadKey();
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input! Please enter a valid number.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}