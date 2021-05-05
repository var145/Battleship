using System.Collections.Generic;

namespace Game
{
    public class Battleship
    {
        public Battleship()
        {
        }

        public Battleship(List<Coordinates> coords1, int w = 2)
        {
            Name = "Battleship";
            Width = w;
            coords = coords1;
        }

        public string Name { get; set; }
        public int Width { get; set; }
        public List<Coordinates> coords { get; set; }
        public int Hits { get; set; }
        public bool IsSunk
        {
            get
            {
                return Hits >= Width;
            }
        }
    }
}