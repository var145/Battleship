using System;
using System.Collections.Generic;

namespace Game
{

class StateTrackerService
    {
        static void Main(string[] args)
        {
            Console.WriteLine("please give player 1 name");
            String playerName1 = Console.ReadLine();

            var player1 = new Player(playerName1);
            
            Console.WriteLine("please give player 2 name");
            String playerName2 = Console.ReadLine();

            var player2 = new Player(playerName2);

            var emptyBoard1 = player1.CreateBoard();
            var boardWithShips1 = player1.PlaceShips(emptyBoard1,2);

            var emptyBoard2 = player2.CreateBoard();
            var boardWithShips2 = player2.PlaceShips(emptyBoard1,2);
            var activeTurn = 1;
            while (!player1.HasLost && !player2.HasLost)
            {
                if (!player1.HasLost && activeTurn==1) //If player 1 already lost, we can't let them take another turn.
                {
                    PlayTurn(player1,boardWithShips1);
                }
                activeTurn++;
                if (!player2.HasLost && activeTurn == 2) //If player 2 already lost, we can't let them take another turn.
                {
                    PlayTurn(player2,boardWithShips2);
                }
                activeTurn--;
            }

            if (player1.HasLost)
            {
                Console.WriteLine(player2.Name + " has won the game!");
            }
            else if (player2.HasLost)
            {
                Console.WriteLine(player1.Name + " has won the game!");
            }

        }

        public static ShotResult PlayTurn(Player player, List<BoardCell> boardWithShips)
        {
            var attackedCoords = player.TakeTurn(boardWithShips);
            return player.ProcessShot(attackedCoords, boardWithShips);

        }

    }
}
