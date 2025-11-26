using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battaglia_navale_Eventi_Form
{
    public class Ship
    {
        public Guid Id { get; private set; }
        public int size {  get; private set; }
        public int cellecolpite { get; private set; }
        public bool Affondata => cellecolpite >= size;
        public List<(int X, int Y)> CoordinateOccupate { get; set; }
        public Ship(int size)
        {
            this.Id = Guid.NewGuid();
            this.size = size;
            this.cellecolpite = 0;
            this.CoordinateOccupate = new List<(int X, int Y)>();
        }
        public Ship(int size, List<(int X, int Y)> coordinate) : this(size)
        {
            this.CoordinateOccupate = coordinate;
        }
        public void OccupaCoordinata((int X, int Y) coord)
        {
            if (this.CoordinateOccupate.Count < this.size)
            {
                this.CoordinateOccupate.Add(coord);
            }
            else
            {
                throw new InvalidOperationException("Non è possibile aggiungere altre coordinate, la nave ha raggiunto la sua dimensione massima.");
            }
        }
        public void assingCoordinate(List<(int X, int Y)> coord)
        {
            foreach (var c in coord)
            {
                OccupaCoordinata(c);
            }
        }
        public void Colpito()
        {
            cellecolpite++;
        }
    }
}
