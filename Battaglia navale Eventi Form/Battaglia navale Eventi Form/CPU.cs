using System;
using System.Collections.Generic;
using System.Linq;

namespace Battaglia_navale_Eventi_Form
{
    public class CPU
    {
        public List<(int x, int y)> hittablecells {  get; set; }
        public Random r { get; private set; }
        public bool IsSearching { get; set; }
        public CPU() {
            IsSearching = false;
            r = new Random(Environment.TickCount);
            hittablecells = new List<(int x, int y)>();
            //get a list of all possible cells that can attack
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    hittablecells.Add((j, i));
                }
            }
        }
        public (int, int) AttackRandomCell()
        {
            int index = r.Next(0, hittablecells.Count-1);
            (int, int) cell = hittablecells[index];
            hittablecells.RemoveAt(index);
            return cell;
        }
        public void Attack()
        {
            AttackCPU?.Invoke(this,EventArgs.Empty);
        }
        public event EventHandler<EventArgs> AttackCPU;
    }
}