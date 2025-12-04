using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battaglia_navale_Eventi_Form
{
    public partial class Battaglia_navale : Form
    {
        DataGridView dgv1;
        DataGridView dgv2;
        List<Ship> flottePL1;
        List<Ship> flotta2;
        CPU cpu;
        bool isPvPGame;
        bool TurnPL1 = true;
        private bool isProcessingClick = false;
        int nmosse = 0;
        public Battaglia_navale(bool isPvP)
        {
            InitializeComponent();
            this.isPvPGame = isPvP; // Salva la modalità di gioco
        }

        public Battaglia_navale()
        {
            InitializeComponent();
        }
        private void Battaglia_navale_Load(object sender, EventArgs e)
        {
            TurnPL1 = true;
            nmosse = 0;

            lbl_turn.Location = new Point(480, 10);
            lbl_n_moves.Location = new Point(480, 60);
            lbl_turn.Text = TurnPL1 ? "Turno: Player 1" : "Turno: Player 2";

            this.Size = new Size(700, 500);

            if (isPvPGame)
            {
                PvP_Setup();
            }
            else
            {
                PvE_Setup();
            }
            
        }
        #region PvP
        private void PvP_Setup()
        {
            using (Posiziona_navi n1 = new Posiziona_navi())
            {
                n1.PosizionateTutteLeNavi += SalvaPosizionamentoNavi;
                //Caso Alt+F4
                if (n1.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Posizionamento delle navi del Giocatore 1 annullato. L'applicazione verrà chiusa.");
                    Application.Exit();
                    return;
                }
            }
            MessageBox.Show("Posizioni Player 1 salvate");

            using (Posiziona_navi n2 = new Posiziona_navi())
            {
                n2.PosizionateTutteLeNavi += SalvaPosizionamentoNavi;
                //Caso Alt+F4
                if (n2.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Posizionamento delle navi del Giocatore 2 annullato. L'applicazione verrà chiusa.");
                    Application.Exit();
                    return;
                }
            }
            MessageBox.Show("Posizioni Player 2 salvate");

            SetUpGrids();
            PlayTurn();

        }
        private void SalvaPosizionamentoNavi(object sender, PosizionamentoEventArgs e)
        {

            if (dgv1 == null)
            {
                dgv1 = e.Griglia;

                flottePL1 = e.Flotta;
            }
            else
            {
                dgv2 = e.Griglia;

                flotta2 = e.Flotta;
            }
        }
        private async void DataGridViewMappa_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!isPvPGame && !TurnPL1) return;
            if (isProcessingClick)
            {
                return;
            }
            int riga = e.RowIndex;
            int colonna = e.ColumnIndex;

            if (riga >= 0 && colonna >= 0)
            {
                try
                {
                    isProcessingClick = true;
                    DataGridView dgv = (DataGridView)sender;
                    DataGridViewCell cellaCliccata = dgv.Rows[riga].Cells[colonna];
                    InfoCella info = (InfoCella)cellaCliccata.Tag;
                    cellaCliccata.Selected = false;
                    if (!(info.stato == 0 || info.stato == 1))
                    {
                        return;//ignora se la cella è già stata schiacciata
                    }
                    switch (info.stato)
                    {
                        case 0:
                            info.stato = 2;
                            cellaCliccata.Style.BackColor = info.OttieniColoreDaStato();
                            //Application.DoEvents(); da usare se il play è sincrono
                            SoundManager.PlayAcqua();
                            TurnPL1 = !TurnPL1;
                            break;
                        case 1:
                            info.stato = 3;
                            info.NaveRiferimento.Colpito();
                            cellaCliccata.Style.BackColor = info.OttieniColoreDaStato();
                            //Application.DoEvents();//colorare prima la cella
                            SoundManager.PlayColpito();
                            break;
                    }
                    Console.WriteLine($"Click su: Colonna {colonna}, Riga {riga}");
                    nmosse++;
                    await Task.Delay(1000);
                    PlayTurn();
                }

                finally
                {
                    // 5. IMPORTANTE: Reimposta il flag alla fine, anche se si verificano errori.
                    isProcessingClick = false;
                }
            }
        }

        #endregion
        private void SetUpGrids(bool PvP = true)
        {

            dgv1.Location = new Point(this.Width / 2, 10);
            this.Controls.Add(dgv1);

            dgv2.Location = new Point((this.Width - dgv2.Width) / 2, (this.Height - dgv2.Height) / 2);
            this.Controls.Add(dgv2);

            if (PvP)
            dgv1.CellMouseClick += DataGridViewMappa_CellMouseClick;
            dgv2.CellMouseClick += DataGridViewMappa_CellMouseClick;

        }
        #region PvE
        private void PvE_Setup()
        {

            using (Posiziona_navi n = new Posiziona_navi())
            {
                n.PosizionateTutteLeNavi += (sender, e) =>
                {
                    dgv1 = e.Griglia;
                    flottePL1 = e.Flotta;
                };

                if (n.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
            }
            MessageBox.Show("Le tue posizioni sono state salvate. Ora la CPU posizionerà le sue navi.");

            cpu = new CPU();
            var setupCPU = CreaGrigliaCPUConNavi();

            dgv2 = setupCPU.Griglia;
            flotta2 = setupCPU.Flotta;
            SetUpGrids(false);


            PlayTurn();
        }

        private void Cpu_OnLauch(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private (DataGridView Griglia, List<Ship> Flotta) CreaGrigliaCPUConNavi()
        {
            // 1. Crea la flotta che la CPU deve posizionare
            List<Ship> flottaCPU = new List<Ship>
            {
                new Ship(4),
                new Ship(3),
                new Ship(3),
                new Ship(2),
                new Ship(2),
                new Ship(1)
            };

            // 2. Genera una griglia di gioco vuota
            DataGridView grigliaCPU = GeneraGrigliaGioco();
            Random rand = new Random(Environment.TickCount);

            // 3. Itera su ogni nave della flotta per posizionarla
            foreach (var nave in flottaCPU)
            {
                while (true)
                {
                    // A. Genera una posizione e un orientamento casuali
                    int startX = rand.Next(0, 10);
                    int startY = rand.Next(0, 10);
                    bool verticale = rand.Next(0, 2) == 0;

                    // B. Tenta di validare la posizione usando la logica esistente
                    List<(int X, int Y)> coordinateValide = CalcolaEValidaCoordinate(nave.size, startX, startY, verticale, grigliaCPU);

                    // C. Se la posizione è valida (non si sovrappone e non esce dai bordi)
                    if (coordinateValide != null)
                    {
                        // Assegna le coordinate all'oggetto nave
                        nave.CoordinateOccupate = coordinateValide;

                        // Aggiorna lo STATO LOGICO delle celle nella griglia della CPU
                        foreach ((int x, int y) in nave.CoordinateOccupate)
                        {
                            DataGridViewCell cella = grigliaCPU.Rows[y].Cells[x];
                            InfoCella info = (InfoCella)cella.Tag;
                            info.AggiugiRiferimentoNave(nave); // Imposta stato = 1 e NaveRiferimento
                            cella.Tag = info;
                            cella.Style.BackColor = info.OttieniColoreDaStato();

                        }
                        break;
                    }
                }
            }

            return (grigliaCPU, flottaCPU);
        }
        private List<(int X, int Y)> CalcolaEValidaCoordinate(int lunghezza, int startX, int startY, bool verticale, DataGridView grigliaDiLavoro)
        {
            const int DimensioneGriglia = 10;
            List<(int X, int Y)> coordinateProposte = new List<(int, int)>();

            // 1. Validazione bordi (rimane invariata)
            if (verticale)
            {
                if (startY + lunghezza > DimensioneGriglia) return null;
            }
            else
            {
                if (startX + lunghezza > DimensioneGriglia) return null;
            }

            // 2. Validazione sovrapposizione
            for (int i = 0; i < lunghezza; i++)
            {
                int x = verticale ? startX : startX + i;
                int y = verticale ? startY + i : startY;

                // USA LA GRIGLIA PASSATA COME PARAMETRO, non dgvMappa
                InfoCella info = (InfoCella)grigliaDiLavoro.Rows[y].Cells[x].Tag;

                if (info.stato != 0) // Se la cella è già occupata
                {
                    return null; // Posizione non valida
                }

                coordinateProposte.Add((x, y));
            }

            return coordinateProposte;
        }
        #endregion
        private DataGridView GeneraGrigliaGioco(int DimensioneGriglia = 10, int DimensioneCella = 45)
        {
            DataGridView dgv = new DataGridView();

            // Impostazioni di base e layout
            dgv.Dock = DockStyle.None; // Il Form deciderà la posizione
            dgv.Size = new Size(
                DimensioneGriglia * DimensioneCella, // Larghezza calcolata
                DimensioneGriglia * DimensioneCella // Altezza calcolata
            );
            dgv.RowHeadersVisible = false;
            dgv.ColumnHeadersVisible = false;
            dgv.ScrollBars = ScrollBars.None;
            dgv.BackgroundColor = Color.LightBlue; // Colore per simulare l'acqua
            dgv.AutoGenerateColumns = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.DefaultCellStyle.SelectionBackColor = Color.Transparent;

            // 2. Configurazione delle Colonne
            for (int i = 0; i < DimensioneGriglia; i++)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn
                {
                    Width = DimensioneCella
                };
                dgv.Columns.Add(col);
            }

            // 3. Configurazione delle Righe
            dgv.RowCount = DimensioneGriglia;
            for (int i = 0; i < DimensioneGriglia; i++)
            {
                dgv.Rows[i].Height = DimensioneCella;


            }

            for (int riga = 0; riga < 10; riga++)
            {
                for (int colonna = 0; colonna < 10; colonna++)
                {
                    dgv.Rows[riga].Cells[colonna].Style.BackColor = Color.LightBlue;
                    // Inizialmente, tutte le celle sono Acqua (NaveRiferimento = null)
                    InfoCella dati = new InfoCella(colonna, riga);

                    // Assegna l'oggetto CellaDati alla proprietà Tag della cella UI
                    dgv.Rows[riga].Cells[colonna].Tag = dati;
                }
            }

            return dgv;
        }
        private void PlayTurn()
        {
            if (flotta2.All(nave => nave.Affondata)|| flottePL1.All(nave => nave.Affondata))
            {
                Fine();
                return;
            }

            lbl_n_moves.Text = $"Mosse: {nmosse}";

            dgv2.Visible = TurnPL1;
            dgv1.Visible = !TurnPL1;

            lbl_turn.Text = !isPvPGame ? TurnPL1 ? "Tocca a te!" : "Turno della CPU...": TurnPL1? "Turno: Player 1" : "Turno: Player 2";
            if(!isPvPGame && !TurnPL1)
            {
                cpu.Attack();
                //EseguiTurnoCPU();
            }
        }
        private void AttaccoRandom()
        {

            (int x, int y) = cpu.AttackRandomCell();
            DataGridViewCell cella = dgv1.Rows[x].Cells[y];
            InfoCella info = (InfoCella)cella.Tag;
            if (info.stato == 0) // ACQUA (Vuoto)
            {
                info.stato = 2; // Imposta stato "Mancato"
                cella.Style.BackColor = info.OttieniColoreDaStato();
                SoundManager.PlayAcqua();
                Console.WriteLine($"CPU ha mancato in {x},{y}");
            }
            else if (info.stato == 1) // NAVE
            {
                info.stato = 3; // Imposta stato "Colpito"
                info.NaveRiferimento.Colpito(); // Riduce la vita della nave
                cella.Style.BackColor = info.OttieniColoreDaStato();
                SoundManager.PlayColpito();
                Console.WriteLine($"CPU ha colpito in {x},{y}");
            }

            // 5. Passa il turno al giocatore
            TurnPL1 = true; // Ora tocca al Player 1

            // Richiama PlayTurn per aggiornare le label e verificare vittorie
            PlayTurn();

        }
        private void UpdateUICell(DataGridViewCell cell)
        {

            InfoCella info = (InfoCella)cell.Tag;
            cell.Style.BackColor = info.OttieniColoreDaStato();
        }
        private void Fine()
        {
            dgv1.Enabled = false;
            dgv2.Enabled = false;
            string vincitore = !isPvPGame ? TurnPL1 ? "Hai vinto te!" : "Ha vinto la CPU" : TurnPL1 ? "Ha vinto Player 1" : "Ha vinto Player 2";
            string messaggio = $"{vincitore}\n\nVuoi giocare un'altra partita?";
            string titolo = "Partita Terminata";

            // 3. Mostra il MessageBox con le opzioni "Sì" e "No"
            DialogResult risultato = MessageBox.Show(messaggio, titolo, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            // 4. Esegui l'azione in base alla scelta dell'utente
            if (risultato == DialogResult.Yes)
            {
                // Se l'utente clicca "Sì", riavvia l'applicazione.
                Application.Restart();
            }
            else
            {
                // Se l'utente clicca "No" (o chiude la finestra di dialogo), chiudi l'applicazione.
                Application.Exit();
            }
        }
    }
    public static class SoundManager
    {
        // Creiamo un SoundPlayer per ogni suono
        private static readonly SoundPlayer playerColpito = new SoundPlayer();
        private static readonly SoundPlayer playerAcqua = new SoundPlayer();
        private static SoundPlayer currentPlayer = null;
        static SoundManager()
        {
            playerColpito.Stream = Properties.Resources.suono_colpito;
            playerAcqua.Stream = Properties.Resources.suono_acqua;

            playerColpito.Load();
            playerAcqua.Load();
        }

        private static void StopCurrentSound()
        {
            if (currentPlayer != null)
            {
                currentPlayer.Stop();
                currentPlayer = null;
            }
        }

        public static void PlayColpito()
        {
            StopCurrentSound();

            currentPlayer = playerColpito;
            currentPlayer.Play();
        }

        public static void PlayAcqua()
        {
            // 1. Ferma il suono 
            StopCurrentSound();

            currentPlayer = playerAcqua;
            currentPlayer.Play();
        }
    }


}
