using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battaglia_navale
{
    public class Ship
    {
        public List<Cell> cellsLocated { get; private set; }
        public int lengthShip { get; private set; }
        public bool ItSank => cellsLocated.All(c => c.state == 3 || c.state == 4);

        public Ship()
        {
            this.cellsLocated = new List<Cell>();
            this.lengthShip = 0;
        }
        public Ship(int lenghtShip) : this() 
        {
            this.lengthShip = lenghtShip;
        }
        public void AssingLocation(List<Cell> cellsLocated)
        {
            this.cellsLocated = cellsLocated;
        }
    }
}
