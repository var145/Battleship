namespace Game
{
    public class BoardCell
    {
        public Coordinates Coordinates { get; set; }

        public BoardCell(int row, int column)
        {
            Coordinates = new Coordinates(row, column);
        }
        public bool IsOccupied { get; set; }
        public bool IsHit { get; set; }
        public bool IsMiss { get; set; }
        public bool IsEmpty { get; set; } = true;

        public bool IsRandomAvailable
        {
            get
            {
                return (Coordinates.Row % 2 == 0 && Coordinates.Column % 2 == 0)
                    || (Coordinates.Row % 2 == 1 && Coordinates.Column % 2 == 1);
            }
        }
    }
}