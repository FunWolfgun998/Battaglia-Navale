using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battaglia_navale
{
    public class Grid
    {
        public Cell[,] cellsGrid {  get; private set; }
        public List<Ship> ships { get; private set; } = new List<Ship>();
        public int ShipsAlive => ships.Count(s => !s.ItSank);

        public Grid()
        {
            cellsGrid = new Cell[10, 10];
            for (int y = 0; y < 10; y++)
            {
                for(int x = 0; x < 10; x++)
                {
                    //Console.WriteLine($"{x},{y}");
                    cellsGrid[y, x] = new Cell();
                }
            }
        }
        public bool SetShip(int xhead, int yhead, int lengthShip, bool IsVerticalOrientated)
        {
            if (IsVerticalOrientated && yhead + lengthShip > 10) return false;
            if (!IsVerticalOrientated && xhead + lengthShip > 10) return false;

            List<Cell> occupedCells = new List<Cell>();
            for (int i = 0; i < lengthShip; i++)
            {
                int cx = IsVerticalOrientated ? xhead : xhead + i;
                int cy = IsVerticalOrientated ? yhead + i : yhead;

                if (cellsGrid[cy, cx].state == 1) return false;
                occupedCells.Add(cellsGrid[cy, cx]);
            }

            Ship ship = new Ship(lengthShip);
            ship.AssingLocation(occupedCells);
            foreach (Cell cell in occupedCells)
            {
                cell.state = 1;
                cell.ShipInside = ship;
            }
            ships.Add(ship);
            return true;
        }

    }
}
