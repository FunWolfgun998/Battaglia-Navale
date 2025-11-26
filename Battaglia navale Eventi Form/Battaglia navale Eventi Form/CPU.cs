using System;
using System.Collections.Generic;
using System.Linq;

namespace Battaglia_navale_Eventi_Form
{
    public class CPU
    {
        public List<(int x, int y)> coordinate {  get; set; }
        public Random r { get; private set; } = new Random(Environment.TickCount);
        public bool IsSearching { get; set; }
        public CPU() {
            IsSearching = false;
            coordinate = new List<(int x, int y)>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    coordinate.Add((j, i));
                }
            }
        }
        public void Attack()
        {
            OnLauch?.Invoke(this,EventArgs.Empty);
        }
        public event EventHandler OnLauch;
    }
}