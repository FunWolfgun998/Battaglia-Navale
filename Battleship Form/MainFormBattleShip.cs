using Battaglia_navale;
using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Battleship_Form
{
    public partial class MainFormBattleShip : Form
    {
        private Battleship _match;
        private DataGridView _dgvLeft;
        private DataGridView _dgvRight;
        private ListBox _logBox;
        private Label _lblTurnInfo;
        private Panel _pnlStartMenu;
        private Label _lblShipsLeftP1;
        private Label _lblShipsLeftP2;

        private int _moveCounter = 0;

        public MainFormBattleShip()
        {
            InitializeComponent();
            this.Size = new Size(1100, 700);
            this.Text = "Battleship using events by FunWolfgun998 (Cristian Borsan)";
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeStartMenu();
        }

        private void InitializeStartMenu()
        {
            _pnlStartMenu = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };

            Label title = new Label
            {
                Text = "BATTLESHIP",
                Font = new Font("Arial", 24, FontStyle.Bold),
                AutoSize = true,
                Top = 150,
                Left = 450
            };

            Button btnSingle = CreateMenuButton("Single Player", 220, GameMode.SinglePlayer);
            Button btnPvE = CreateMenuButton("Player vs CPU", 280, GameMode.PlayerVsCPU);
            Button btnPvP = CreateMenuButton("Player vs Player", 340, GameMode.PlayerVsPlayer);

            _pnlStartMenu.Controls.Add(title);
            _pnlStartMenu.Controls.Add(btnSingle);
            _pnlStartMenu.Controls.Add(btnPvE);
            _pnlStartMenu.Controls.Add(btnPvP);

            this.Controls.Add(_pnlStartMenu);
        }

        private Button CreateMenuButton(string text, int top, GameMode mode)
        {
            Button btn = new Button { Text = text, Top = top, Left = 400, Width = 300, Height = 50 };
            btn.Click += (s, e) => StartGame(mode);
            return btn;
        }

        private void StartGame(GameMode mode)
        {
            _match = new Battleship(mode);
            _match.ShotMade += OnShotMade;

            this.Controls.Remove(_pnlStartMenu);

            if (mode == GameMode.SinglePlayer)
            {
                InitializeGameInterface();
                StartBattleVisuals();
            }
            else if (mode == GameMode.PlayerVsCPU)
            {
                FormPlacementShips setupForm = new FormPlacementShips();
                setupForm.Text = "Player 1 Setup";

                if (setupForm.ShowDialog() == DialogResult.OK)
                {
                    _match.GridPlayer1 = setupForm.ResultGrid;
                    InitializeGameInterface();
                    StartBattleVisuals();
                }
                else InitializeStartMenu();
            }
            else if (mode == GameMode.PlayerVsPlayer)
            {
                FormPlacementShips setupP1 = new FormPlacementShips();
                setupP1.Text = "Player 1 Setup";
                if (setupP1.ShowDialog() == DialogResult.OK)
                {
                    _match.GridPlayer1 = setupP1.ResultGrid;
                    MessageBox.Show("Player 1 Configured. Player 2 Turn!");

                    FormPlacementShips setupP2 = new FormPlacementShips();
                    setupP2.Text = "Player 2 Setup";
                    if (setupP2.ShowDialog() == DialogResult.OK)
                    {
                        _match.GridPlayer2 = setupP2.ResultGrid;
                        InitializeGameInterface();
                        StartBattleVisuals();
                    }
                    else InitializeStartMenu();
                }
                else InitializeStartMenu();
            }
        }

        private void InitializeGameInterface()
        {
            bool isPvP = _match.Mode == GameMode.PlayerVsPlayer;

            // --- LEFT GRID ---
            Label lblLeft = new Label { Text = "Player 1 Waters", Location = new Point(20, 20), AutoSize = true, Font = new Font("Arial", 12, FontStyle.Bold) };
            this.Controls.Add(lblLeft);

            _dgvLeft = CreateDataGridView(20, 50);
            if (isPvP)
            {
                _dgvLeft.CellClick += DgvLeft_CellClick;
                _dgvLeft.Cursor = Cursors.Hand;
            }
            else
            {
                _dgvLeft.Enabled = false;
                _dgvLeft.DefaultCellStyle.BackColor = Color.LightGray;
            }
            this.Controls.Add(_dgvLeft);

            // Ship Counter Left
            _lblShipsLeftP1 = new Label { Text = "Ships Alive: 6", Location = new Point(20, 360), AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold), ForeColor = Color.DarkBlue };
            this.Controls.Add(_lblShipsLeftP1);


            // --- RIGHT GRID ---
            string enemyName = _match.Mode == GameMode.PlayerVsCPU ? "CPU" : "Player 2";
            Label lblRight = new Label { Text = $"{enemyName} Waters", Location = new Point(550, 20), AutoSize = true, Font = new Font("Arial", 12, FontStyle.Bold) };
            this.Controls.Add(lblRight);

            _dgvRight = CreateDataGridView(550, 50);
            _dgvRight.CellClick += DgvRight_CellClick;
            _dgvRight.Cursor = Cursors.Hand;
            this.Controls.Add(_dgvRight);

            //Ship Counter Right
            _lblShipsLeftP2 = new Label { Text = "Ships Alive: 6", Location = new Point(550, 360), AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold), ForeColor = Color.DarkRed };
            this.Controls.Add(_lblShipsLeftP2);


            // --- INFO ---
            _lblTurnInfo = new Label { Text = "Turn: Player 1", Location = new Point(400, 20), AutoSize = true, Font = new Font("Arial", 14, FontStyle.Bold) };
            this.Controls.Add(_lblTurnInfo);

            _logBox = new ListBox { Location = new Point(20, 400), Size = new Size(833, 150), Font = new Font("Consolas", 9) };
            this.Controls.Add(_logBox);

            // Single Player Adjustments
            if (_match.Mode == GameMode.SinglePlayer)
            {
                lblLeft.Visible = false;
                _dgvLeft.Visible = false;
                _lblShipsLeftP1.Visible = false;
                lblRight.Text = "Target Area";
            }
        }

        private void StartBattleVisuals()
        {
            bool showP1Ships = true;
            bool showP2Ships = false;

            if (_match.Mode == GameMode.PlayerVsPlayer)
            {
                showP1Ships = false;
                showP2Ships = false;
            }

            UpdateGridColors(_dgvLeft, _match.GridPlayer1, showP1Ships);
            UpdateGridColors(_dgvRight, _match.GridPlayer2, showP2Ships);
            UpdateLabels();
        }

        private DataGridView CreateDataGridView(int x, int y)
        {
            DataGridView dgv = new DataGridView();
            dgv.Location = new Point(x, y);
            dgv.Size = new Size(303, 303);
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;
            dgv.ColumnHeadersVisible = false;
            dgv.MultiSelect = false;
            dgv.ScrollBars = ScrollBars.None;
            dgv.ReadOnly = true;
            dgv.GridColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            dgv.ColumnCount = 10;
            dgv.RowCount = 10;
            for (int i = 0; i < 10; i++)
            {
                dgv.Columns[i].Width = 30;
                dgv.Rows[i].Height = 30;
            }
            return dgv;
        }

        private void DgvRight_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (_match.Mode == GameMode.PlayerVsPlayer && !_match.turnPlayer1)
            {
                MessageBox.Show("Not your turn! Player 2 must shoot on the Left grid.");
                return;
            }

            int x = e.ColumnIndex;
            int y = e.RowIndex;

            if (_match.GridPlayer2.cellsGrid[y, x].state == 2 || _match.GridPlayer2.cellsGrid[y, x].state == 3 || _match.GridPlayer2.cellsGrid[y, x].state == 4) return;

            // Execute Shot
            bool gameActive = _match.Shot(x, y);

            if (!gameActive) HandleGameOver();
        }

        private void DgvLeft_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (_match.turnPlayer1)
            {
                MessageBox.Show("Not your turn! Player 1 must shoot on the Right grid.");
                return;
            }

            int x = e.ColumnIndex;
            int y = e.RowIndex;

            if (_match.GridPlayer1.cellsGrid[y, x].state == 2 || _match.GridPlayer1.cellsGrid[y, x].state == 3 || _match.GridPlayer1.cellsGrid[y, x].state == 4) return;

            bool gameActive = _match.Shot(x, y);

            if (!gameActive) HandleGameOver();
        }

        private void HandleGameOver()
        {
            _dgvLeft.Enabled = false;
            _dgvRight.Enabled = false;

            string msg = "";
            string title = "Game Over";

            if (_match.Mode == GameMode.SinglePlayer)
            {
                msg = $"MISSION ACCOMPLISHED!\nTotal moves: {_moveCounter}\n\nDo you want to play again?";
                title = "Victory!";
            }
            else
            {
                // The current turn holder is the one who made the winning shot
                string winner = _match.turnPlayer1 ? "PLAYER 1" : (_match.Mode == GameMode.PlayerVsCPU ? "CPU" : "PLAYER 2");
                msg = $"GAME OVER!\nTHE WINNER IS: {winner}\n\nDo you want to play again?";
            }

            DialogResult result = MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);


            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
            else
            {
                Application.Exit(); 
            }
        }

        private void OnShotMade(object sender, ShotArgs e)
        {
            _moveCounter++;
            if (e.Hit) SystemSounds.Beep.Play(); else SystemSounds.Hand.Play();

            string shooter = e.ShotsPlayer1 ? "Player 1" : "CPU/Player 2";
            string logText = $"Move {_moveCounter} [{shooter}]: {e.Message} at ({e.X},{e.Y})";
            if (e.Sank) logText += " -> SHIP SUNK!";
            _logBox.Items.Insert(0, logText);

            DataGridView targetDgv = e.ShotsPlayer1 ? _dgvRight : _dgvLeft;
            Grid targetGrid = e.ShotsPlayer1 ? _match.GridPlayer2 : _match.GridPlayer1;

            UpdateSingleCell(targetDgv, targetGrid, e.X, e.Y);

            if (e.Sank)
            {
                bool show = (!e.ShotsPlayer1 && _match.Mode == GameMode.PlayerVsCPU);
                UpdateGridColors(targetDgv, targetGrid, show);
            }

            UpdateLabels();
        }

        private void UpdateLabels()
        {
            _lblShipsLeftP1.Text = $"Ships Alive: {_match.GridPlayer1.ShipsAlive}";
            _lblShipsLeftP2.Text = $"Ships Alive: {_match.GridPlayer2.ShipsAlive}";

            if (_match.Mode == GameMode.SinglePlayer)
            {
                _lblTurnInfo.Text = $"Moves: {_moveCounter}";
                return;
            }

            if (_match.turnPlayer1)
            {
                _lblTurnInfo.Text = "Turn: Player 1";
                _lblTurnInfo.ForeColor = Color.Blue;
            }
            else
            {
                string p2name = _match.Mode == GameMode.PlayerVsCPU ? "CPU" : "Player 2";
                _lblTurnInfo.Text = $"Turn: {p2name}";
                _lblTurnInfo.ForeColor = Color.Red;
            }
        }

        private void UpdateSingleCell(DataGridView dgv, Grid logicGrid, int x, int y)
        {
            var cellState = logicGrid.cellsGrid[y, x].state;
            dgv.Rows[y].Cells[x].Style.BackColor = GetColorFromState(cellState);
            dgv.Rows[y].Cells[x].Style.SelectionBackColor = GetColorFromState(cellState);
        }

        private void UpdateGridColors(DataGridView dgv, Grid logicGrid, bool showShips)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    int state = logicGrid.cellsGrid[y, x].state;
                    Color c = GetColorFromState(state);
                    if (state == 1)
                    {
                        if (showShips) c = Color.Gray; else c = Color.LightBlue;
                    }
                    dgv.Rows[y].Cells[x].Style.BackColor = c;
                    dgv.Rows[y].Cells[x].Style.SelectionBackColor = c;
                }
            }
        }

        private Color GetColorFromState(int state)
        {
            switch (state)
            {
                case 0: return Color.LightBlue;
                case 1: return Color.LightBlue;
                case 2: return Color.DarkBlue;
                case 3: return Color.Orange;
                case 4: return Color.Red;
                default: return Color.White;
            }
        }
    }
}