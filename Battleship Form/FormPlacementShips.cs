using Battaglia_navale;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Battleship_Form
{
    public partial class FormPlacementShips : Form
    {
        public Grid ResultGrid { get; private set; }

        private DataGridView _dgvMap;
        private ListBox _lstShips;
        private Button _btnRotate;
        private Button _btnDone;

        private bool _isVertical = false;

        // Tracks where the mouse currently is (-1, -1 means outside)
        private Point _currentMousePos = new Point(-1, -1);
        private List<Point> _previousPreviewCells = new List<Point>();

        private class ShipItem
        {
            public string Name { get; set; }
            public int Length { get; set; }
            public override string ToString() => $"{Name} ({Length})";
        }

        public FormPlacementShips()
        {
            InitializeComponent();
            this.Size = new Size(850, 600);
            this.Text = "Deploy your Fleet - Right Click to Remove";
            this.StartPosition = FormStartPosition.CenterParent;

            ResultGrid = new Grid();

            InitializeInterface();
            PopulateShipList();
        }

        private void InitializeInterface()
        {
            // DataGridView Setup
            _dgvMap = new DataGridView();
            _dgvMap.Location = new Point(20, 20);
            _dgvMap.Size = new Size(453, 453);
            _dgvMap.ScrollBars = ScrollBars.None;
            _dgvMap.RowHeadersVisible = false;
            _dgvMap.ColumnHeadersVisible = false;
            _dgvMap.AllowUserToAddRows = false;
            _dgvMap.AllowUserToResizeColumns = false;
            _dgvMap.AllowUserToResizeRows = false;
            _dgvMap.MultiSelect = false;
            _dgvMap.ReadOnly = true;
            _dgvMap.DefaultCellStyle.SelectionBackColor = Color.Transparent; // Hides default blue selection

            // Columns
            _dgvMap.ColumnCount = 10;
            for (int i = 0; i < 10; i++) _dgvMap.Columns[i].Width = 45;

            // Rows
            _dgvMap.RowCount = 10;
            for (int i = 0; i < 10; i++) _dgvMap.Rows[i].Height = 45;

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    _dgvMap.Rows[y].Cells[x].Style.BackColor = Color.LightBlue;
                }
            }

            // Events
            _dgvMap.CellMouseClick += DgvMap_CellMouseClick;
            _dgvMap.CellMouseEnter += DgvMap_CellMouseEnter;
            _dgvMap.CellMouseLeave += DgvMap_CellMouseLeave;

            this.Controls.Add(_dgvMap);

            // Ships List
            Label lblList = new Label { Text = "Ships to Place:", Location = new Point(500, 20), AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold) };
            this.Controls.Add(lblList);

            _lstShips = new ListBox();
            _lstShips.Location = new Point(500, 50);
            _lstShips.Size = new Size(200, 200);
            _lstShips.SelectedIndexChanged += (s, e) => UpdateGrid(); // Redraw if selection changes
            this.Controls.Add(_lstShips);

            // Rotate Button
            _btnRotate = new Button { Text = "Rotate: Horizontal", Location = new Point(500, 260), Size = new Size(200, 50) };
            _btnRotate.Click += BtnRotate_Click;
            this.Controls.Add(_btnRotate);

            // Done Button
            _btnDone = new Button { Text = "START BATTLE", Location = new Point(500, 330), Size = new Size(200, 60), BackColor = Color.LightGray, Enabled = false };
            _btnDone.Click += BtnDone_Click;
            this.Controls.Add(_btnDone);

            // Help Text
            Label lblHelp = new Label
            {
                Text = "HOVER to see preview.\nLEFT CLICK to place.\nRIGHT CLICK on a ship to remove.",
                Location = new Point(500, 410),
                AutoSize = true,
                ForeColor = Color.DarkSlateGray
            };
            this.Controls.Add(lblHelp);

            // Initial Draw
            UpdateGrid();
        }

        private void PopulateShipList()
        {
            _lstShips.Items.Add(new ShipItem { Name = "Aircraft Carrier", Length = 4 });
            _lstShips.Items.Add(new ShipItem { Name = "Battleship", Length = 3 });
            _lstShips.Items.Add(new ShipItem { Name = "Cruiser", Length = 3 });
            _lstShips.Items.Add(new ShipItem { Name = "Destroyer", Length = 2 });
            _lstShips.Items.Add(new ShipItem { Name = "Destroyer", Length = 2 });
            _lstShips.Items.Add(new ShipItem { Name = "Submarine", Length = 1 });

            if (_lstShips.Items.Count > 0) _lstShips.SelectedIndex = 0;
        }

        private void UpdateGrid()
        {
            List<Point> currentPreviewCells = new List<Point>();
            bool isPreviewValid = true;

            if (_currentMousePos.X != -1 && _lstShips.SelectedIndex >= 0)
            {
                int len = ((ShipItem)_lstShips.SelectedItem).Length;

                for (int i = 0; i < len; i++)
                {
                    int x = _isVertical ? _currentMousePos.X : _currentMousePos.X + i;
                    int y = _isVertical ? _currentMousePos.Y + i : _currentMousePos.Y;

                    // Check Borders (allow drawing red outside if partially visible)
                    if (x >= 10 || y >= 10)
                    {
                        isPreviewValid = false;
                        if (x < 10 && y < 10) currentPreviewCells.Add(new Point(x, y));
                    }
                    else
                    {
                        currentPreviewCells.Add(new Point(x, y));
                        // Check Overlap
                        if (ResultGrid.cellsGrid[y, x].state == 1) isPreviewValid = false;
                    }
                }
            }

            if (ListsAreSame(_previousPreviewCells, currentPreviewCells)) return;

            foreach (Point p in _previousPreviewCells)
            {
                int state = ResultGrid.cellsGrid[p.Y, p.X].state;
                Color originalColor = (state == 1) ? Color.Gray : Color.LightBlue;
                _dgvMap.Rows[p.Y].Cells[p.X].Style.BackColor = originalColor;
            }

            Color ghostColor = isPreviewValid ? Color.LightGreen : Color.Salmon;

            foreach (Point p in currentPreviewCells)
            {
                _dgvMap.Rows[p.Y].Cells[p.X].Style.BackColor = ghostColor;
            }

            _previousPreviewCells = currentPreviewCells;

            _dgvMap.ClearSelection();
        }

        private bool ListsAreSame(List<Point> a, List<Point> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        // --- MOUSE EVENTS ---

        private void DgvMap_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            _currentMousePos = new Point(e.ColumnIndex, e.RowIndex);
            UpdateGrid();
        }

        private void DgvMap_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            _currentMousePos = new Point(-1, -1); // Mouse is outside
            UpdateGrid();
        }

        private void DgvMap_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // RIGHT CLICK: Remove
            if (e.Button == MouseButtons.Right)
            {
                RemoveShipAt(e.ColumnIndex, e.RowIndex);
                UpdateGrid();
                return;
            }

            // LEFT CLICK: Place
            if (e.Button == MouseButtons.Left && _lstShips.SelectedIndex >= 0)
            {
                ShipItem item = (ShipItem)_lstShips.SelectedItem;

                // Try to place in Logic
                bool placed = ResultGrid.SetShip(e.ColumnIndex, e.RowIndex, item.Length, _isVertical);

                if (placed)
                {
                    // Success: Remove from list
                    _lstShips.Items.RemoveAt(_lstShips.SelectedIndex);
                    if (_lstShips.Items.Count > 0) _lstShips.SelectedIndex = 0;

                    CheckCompletion();
                }
                else
                {
                    // Invalid placement beep
                    System.Media.SystemSounds.Hand.Play();
                }

                UpdateGrid();
            }
        }

        // --- UTILITIES ---

        private void RemoveShipAt(int x, int y)
        {
            var cell = ResultGrid.cellsGrid[y, x];
            if (cell.ShipInside == null) return;

            Ship ship = cell.ShipInside;
            ResultGrid.ships.Remove(ship);

            // Clear cells
            foreach (var c in ship.cellsLocated)
            {
                c.state = 0;
                c.ShipInside = null;
            }

            // Add back to list
            string name = GetNameFromLength(ship.lengthShip);
            _lstShips.Items.Add(new ShipItem { Name = name, Length = ship.lengthShip });
            _lstShips.SelectedIndex = _lstShips.Items.Count - 1;

            CheckCompletion();
        }

        private void BtnRotate_Click(object sender, EventArgs e)
        {
            _isVertical = !_isVertical;
            _btnRotate.Text = _isVertical ? "Rotate: Vertical" : "Rotate: Horizontal";
            UpdateGrid(); // Redraw preview with new orientation
        }

        private void CheckCompletion()
        {
            if (_lstShips.Items.Count == 0)
            {
                _btnDone.Enabled = true;
                _btnDone.BackColor = Color.LightGreen;
            }
            else
            {
                _btnDone.Enabled = false;
                _btnDone.BackColor = Color.LightGray;
            }
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string GetNameFromLength(int len)
        {
            switch (len)
            {
                case 4: return "Aircraft Carrier";
                case 3: return "Warship";
                case 2: return "Destroyer";
                case 1: return "Submarine";
                default: return "Ship";
            }
        }

        private void FormPlacementShips_Load(object sender, EventArgs e)
        {

        }
    }
}