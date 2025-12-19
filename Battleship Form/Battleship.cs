using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Battaglia_navale
{
    public enum GameMode
    {
        SinglePlayer, // The player shoots only to find the ships (puzzle)
        PlayerVsCPU,
        PlayerVsPlayer
    }


    public class Battleship
    {
        public Grid GridPlayer1 { get; set; } // Grid player 1
        public Grid GridPlayer2 { get; set; } // Griglia player 2 or CPU grid

        public GameMode Mode { get; private set; }
        public bool turnPlayer1;

        // CPU variables
        private Random _rnd = new Random();
        private List<Point> _cpuAvailableCells = new List<Point>();
        private Stack<Point> _cpuTargetStack = new Stack<Point>();
        private bool _cpuInTargetMode = false;


        // event that updates the form
        public event EventHandler<ShotArgs> ShotMade;

        public Battleship(GameMode mode)
        {
            this.Mode = mode;
            this.GridPlayer1 = new Grid();
            this.GridPlayer2 = new Grid();
            this.turnPlayer1 = true; //always starts player 1;

            // Setup based of the gamemode
            if (mode == GameMode.SinglePlayer)
            {
                GenerateRandomSetGrid(GridPlayer2); // Enemy ships
            }
            else if (mode == GameMode.PlayerVsCPU)
            {
                InizializeCPU();
                GenerateRandomSetGrid(GridPlayer2); // CPU ships
                // Player 1 ships
                GenerateRandomSetGrid(GridPlayer1);
            }
            else // PvP
            {
                // Player 1 ships
                GenerateRandomSetGrid(GridPlayer1);
                // Player 2 ships
                GenerateRandomSetGrid(GridPlayer2);
            }
        }

        // 2. Modifica Shot per controllare la vittoria e i turni
        public bool Shot(int x, int y)
        {
            Grid targetGrid = (turnPlayer1) ? GridPlayer2 : GridPlayer1;

            // Execute the shot logic
            bool Hit = ProcessShot(targetGrid, x, y);

            // Check if the specific cell is now Sunk
            bool justSank = targetGrid.cellsGrid[y, x].state == 4;

            // Fire the event for UI
            ShotArgs args = new ShotArgs
            {
                X = x,
                Y = y,
                ShotsPlayer1 = turnPlayer1,
                Hit = Hit,
                Sank = justSank,
                Message = Hit ? "Hit!" : "Missed"
            };
            ShotMade?.Invoke(this, args);

            // Check if ALL ships in the target grid are sunk => END GAME
            bool victory = targetGrid.ships.Count > 0 && targetGrid.ships.All(s => s.ItSank);

            if (victory)
            {
                return false; // Game Over (Active = false)
            }

            // --- TURN MANAGEMENT ---
            if (Mode != GameMode.SinglePlayer)
            {
                ChangeTurn();
            }

            return true; // Game Continues (Active = true)
        }

        private void ChangeTurn()
        {
            if (turnPlayer1)
            {
                turnPlayer1 = false;

                if (Mode == GameMode.PlayerVsCPU)
                {
                    CPUShot();
                }
            }
            else
            {
                turnPlayer1 = true;
            }
        }

        // --- CPU Logic ---
        public void InizializeCPU()
        {
            _cpuAvailableCells.Clear();
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                    _cpuAvailableCells.Add(new Point(x, y));
        }

        public void CPUShot()
        {
            Point target;

            if (_cpuInTargetMode && _cpuTargetStack.Count > 0)
                target = _cpuTargetStack.Pop();
            else
            {
                _cpuInTargetMode = false;
                if (_cpuAvailableCells.Count == 0) return; // End game
                int index = _rnd.Next(_cpuAvailableCells.Count);
                target = _cpuAvailableCells[index];
            }

            // Remove from available
            _cpuAvailableCells.RemoveAll(p => p.X == target.X && p.Y == target.Y);

            bool hit = ProcessShot(GridPlayer1, target.X, target.Y);

            // CPU Logic 
            if (hit && GridPlayer1.cellsGrid[target.Y, target.X].state == 3)//if it hit a undiscovered ship
            {
                _cpuInTargetMode = true;
                AddAdjacentTargets(target);
            }

            // Invocation event for form update
            ShotMade?.Invoke(this, new ShotArgs
            {
                X = target.X,
                Y = target.Y,
                ShotsPlayer1 = turnPlayer1,
                Hit = hit,
                Sank = GridPlayer1.cellsGrid[target.Y, target.X].state == 4
            });

            turnPlayer1 = true;
        }

        // --- Logic for both ---
        public bool ProcessShot(Grid grid, int x, int y)
        {
            Cell cell = grid.cellsGrid[y, x];
            bool hit = false;

            if (cell.state == 1) // Ship
            {
                hit = true;
                cell.state = 3; // First mark as Hit (3)

                // Check if the WHOLE ship is down
                if (cell.ShipInside.ItSank)
                {
                    // Update all cells of that ship to Sunk (4)
                    foreach (var c in cell.ShipInside.cellsLocated)
                    {
                        c.state = 4;
                    }
                }
            }
            else if (cell.state == 0) // Water
            {
                cell.state = 2; // Missed
            }

            return hit;
        }
        private void AddAdjacentTargets(Point p)
        {
            // +y,-y,x,-x
            Point[] directions = { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };

            foreach (var d in directions)
            {
                int nx = p.X + d.X;
                int ny = p.Y + d.Y;
                // Check if isn't out of index
                if (nx >= 0 && nx < 10 && ny >= 0 && ny < 10 && _cpuAvailableCells.Any(b => b.X == nx && b.Y == ny))
                {
                    _cpuTargetStack.Push(new Point(nx, ny));
                }
            }
        }
        public void GenerateRandomSetGrid(Grid g)
        {
            int[] shipSizes = { 4, 3, 3, 2, 2, 1 };
            foreach (int len in shipSizes)
            {
                bool placed = false;
                while (!placed)
                {
                    int x = _rnd.Next(10);
                    int y = _rnd.Next(10);
                    bool orientation = _rnd.Next(2) == 0;
                    placed = g.SetShip(x, y, len, orientation);
                }
            }
        }
    }

    public class ShotArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool ShotsPlayer1 { get; set; }
        public bool Hit { get; set; }
        public bool Sank { get; set; }
        public string Message { get; set; }
    }
}