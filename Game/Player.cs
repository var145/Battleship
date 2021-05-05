using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Player
    {
        public string Name { get; set; }
        public List<Battleship> Battleships { get; set; }
        public bool HasLost
        {
            get
            {
                return Battleships.All(x => x.IsSunk);
            }
        }

        public Player(string name)
        {
            Name = name;
        }

        public Coordinates TakeTurn(List<BoardCell> boardCells)
        {
            var attackedCoordinates = FireShot(boardCells);
            return attackedCoordinates;
        }

        public ShotResult ProcessShot(Coordinates attackedCoordinates,List<BoardCell> gameBoard)
        {
            //Locate the targeted boardcell on the GameBoard
            var boardCell = gameBoard.FirstOrDefault(x=>x.Coordinates.Row == attackedCoordinates.Row
                                                        && x.Coordinates.Column == attackedCoordinates.Column);

            //If the boardcell is NOT occupied by a ship
            if (!boardCell.IsOccupied)
            {
                //Call out a miss
                Console.WriteLine(Name + " says: \"Miss!\"");
                return ShotResult.Miss;
            }

            //If the boardcell IS occupied by a ship, determine which one.
            var ship = new Battleship();
            foreach(var aShip in this.Battleships)
            {
               foreach(var c in aShip.coords)
                {
                    if(c.Row == attackedCoordinates.Row && c.Column == attackedCoordinates.Column)
                    {
                        ship = aShip;
                        break;
                    }
                }

            }
            //Increment the hit counter
            ship.Hits++;
            
            //Call out a hit
            Console.WriteLine(Name + " says: \"Hit!\"");

            //If the ship is now sunk, call out which ship was sunk
            if (ship.IsSunk)
            {
                Console.WriteLine(Name + " says: \"You sunk my " + ship.Name + "!\"");
            }

            //For either a hit or a sunk, return a Hit status
            return ShotResult.Hit;
        }

        public Coordinates FireShot(List<BoardCell> boardWithShips)
        {
            //If there are hits on the board with neighbors which don't have shots,
            //we should fire at those first.
            var hitNeighbors = GetHitNeighbors(boardWithShips);
            Coordinates coords;
            if (hitNeighbors.Any())
            {
                coords = SearchShot(boardWithShips,hitNeighbors);
            }
            else
            {
                coords = RandomShot(boardWithShips);
            }
            Console.WriteLine(Name + " says: \"Firing shot at "
                              + coords.Row.ToString()
                              + ", " + coords.Column.ToString()
                              + "\"");
            return coords;
        }

        private Coordinates SearchShot(List<BoardCell> boardWithShips, List<Coordinates> hitNeighbors)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            var neighborID = rand.Next(hitNeighbors.Count);
            return hitNeighbors[neighborID];
        }

        private Coordinates RandomShot(List<BoardCell> boardWithShips)
        {
            var availableCells = GetOpenRandomCells(boardWithShips);
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            var cellID = rand.Next(availableCells.Count);
            return availableCells[cellID];
        }

        public List<Coordinates> GetOpenRandomCells(List<BoardCell> boardCells)
        {
            return boardCells.Where(x => x.IsEmpty
                                     && x.IsRandomAvailable)
                         .Select(x => x.Coordinates)
                         .ToList();
        }

        public List<Coordinates> GetHitNeighbors(List<BoardCell> boardCells)
        {
            var hits = boardCells.Where(x => x.IsHit);
            foreach (var hit in hits)
            {
                boardCells.AddRange(GetNeighbors(hit.Coordinates).ToList());
            }
            return boardCells.Distinct()
                         .Where(x => x.IsEmpty)
                         .Select(x => x.Coordinates)
                         .ToList();
        }

        public List<BoardCell> GetNeighbors(Coordinates coordinates)
        {
            var row = coordinates.Row;
            var column = coordinates.Column;
            var adjacentBoardCellTop = new BoardCell(row-1,column)
            {
                Coordinates = new Coordinates(row - 1, column),
                IsOccupied = false
            };
            var adjacentBoardCellBottom = new BoardCell(row + 1, column)
            {
                Coordinates = new Coordinates(row + 1, column),
                IsOccupied = false
            };
            var adjacentBoardCellLeft = new BoardCell(row, column-1)
            {
                Coordinates = new Coordinates(row, column-1),
                IsOccupied = false
            };
            var adjacentBoardCellRight = new BoardCell(row, column+1)
            {
                Coordinates = new Coordinates(row, column+1),
                IsOccupied = false
            };
            var boardCells = new List<BoardCell>();
            boardCells.Add(adjacentBoardCellTop);
            boardCells.Add(adjacentBoardCellBottom);
            boardCells.Add(adjacentBoardCellLeft);
            boardCells.Add(adjacentBoardCellRight);
            return boardCells;
        }

        public List<BoardCell> Range(List<BoardCell> boardCells,
                                    int startRow,
                                    int startColumn,
                                    int endRow,
                                    int endColumn)
        {
            return boardCells.Where(x => x.Coordinates.Row >= startRow
                                     && x.Coordinates.Column >= startColumn
                                     && x.Coordinates.Row <= endRow
                                     && x.Coordinates.Column <= endColumn).ToList();
        }
        public List<BoardCell> CreateBoard()
        {
            
            var boardCells = new List<BoardCell>();
            for (int i = 1; i <= 10; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    boardCells.Add(new BoardCell(i, j));
                }
            }
            return boardCells;
        }

        public List<BoardCell> PlaceShips(List<BoardCell> boardCells,int shipWidth)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            //foreach (var ship in Battleships)
            //{
                bool isOpen = true;
                while (isOpen)
                {
                    var startcolumn = rand.Next(1, 11);
                    var startrow = rand.Next(1, 11);
                    int endrow = startrow, endcolumn = startcolumn;
                    var orientation = rand.Next(1, 101) % 2; //0 for Horizontal

                    List<int> cellNumbers = new List<int>();
                    if (orientation == 0)
                    {
                        for (int i = 1; i < shipWidth; i++)
                        {
                            endrow++;
                        }
                    }
                    else
                    {
                        for (int i = 1; i < shipWidth; i++)
                        {
                            endcolumn++;
                        }
                    }

                    //We cannot place ships beyond the boundaries of the board
                    if (endrow > 10 || endcolumn > 10)
                    {
                        isOpen = true;
                        continue; //Restart the while loop to select a new random boardcells
                    }

                    //Check if specified boardcells are occupied
                    var affectedBoardCells = Range(boardCells, startrow,
                                                             startcolumn,
                                                             endrow,
                                                             endcolumn);

                    if (affectedBoardCells.Any(x => x.IsOccupied))
                    {

                        isOpen = true;
                        continue;
                    }

                    isOpen = false;
                    var listShipCoords = new List<Coordinates>();
                    foreach (var boardCell in affectedBoardCells)
                    {
                        listShipCoords.Add(boardCell.Coordinates);
                        boardCell.IsOccupied = true;
                    }
                    var playerShip = new Battleship(listShipCoords);
                this.Battleships = new List<Battleship>();
                this.Battleships.Add(playerShip);

                }
            //}

            return boardCells;
        }
    }
}