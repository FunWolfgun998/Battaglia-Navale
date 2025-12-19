using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battaglia_navale
{
    public class Cell
    {
        public int x { get; private set; }
        public int y { get; private set; }
        /*
         Value	Meaning
            0	Water    (Not hit)
            1	Ship     (Not hit)
            2	Missed   (Hit)
            3	Hit ship (Hit)
            4   Sank ship
        */
        public int state { get; set; }
        public Ship ShipInside { get; set; }
        public Cell()
        {
            this.x = 0;
            this.y = 0;
            this.state = 0;
            this.ShipInside = null;
        }
        public Cell(int x, int y): this()
        {
            this.x = x;
            this.y = y;
        }
    }
}
