using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battaglia_navale_Eventi_Form
{
    public partial class Posiziona_navi : Form
    {
        private DataGridView dgvMappa;
        bool verticale = false;
        private Dictionary<string, Ship> navi;
        public event EventHandler<PosizionamentoEventArgs> PosizionateTutteLeNavi;
        public Posiziona_navi()
        {
            InitializeComponent();
        }

        private void Posiziona_navi_Load(object sender, EventArgs e)
        {
            navi = new Dictionary<string, Ship>()
            {
                { "Portaerei", new Ship(4) },
                { "Corazzata", new Ship(3) },
                { "Incrociatore", new Ship(3) },
                { "Sottomarino", new Ship(2) },
                { "Cacciatorpediniere", new Ship(2) },
                { "MiniScafo", new Ship(1) }
            };
            ImpostaMenù();


        }
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

            dgv.CellMouseClick += DataGridViewMappa_CellMouseClick;
            return dgv;
        }
        private void DataGridViewMappa_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Clic destro viene gestito separatamente per l'annullamento
            if (e.Button == MouseButtons.Right)
            {
                AnnullaPosizionamento(e.ColumnIndex, e.RowIndex);
                return;
            }

            // Solo se è un Click Sinistro
            if (e.Button == MouseButtons.Left && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cellaCliccata = dgvMappa.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cellaCliccata.Selected = false;

                // 1. Controllo: Clic su cella non valida (rossa)
                if (cellaCliccata.Style.BackColor == Color.Red)
                {
                    MessageBox.Show("Posizione iniziale non valida. La nave andrebbe fuori dalla griglia.");
                    return;
                }

                // 2. Controllo: Nave Selezionata
                if (lst_select_boats.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleziona prima una nave da posizionare.");
                    return;
                }

                int indiceSelezionato = lst_select_boats.SelectedIndex;
                int lunghezzaSelezionata = navi[(string)lst_select_boats.SelectedItem].size;

                if (TentaPosizionamento(lunghezzaSelezionata, e.ColumnIndex, e.RowIndex, verticale))
                {
                    Ship nave = navi[(string)lst_select_boats.Items[indiceSelezionato]];
                    nave.assingCoordinate(Ritornaposizionamento(lunghezzaSelezionata, e.ColumnIndex, e.RowIndex, verticale));
                    foreach (var item in nave.CoordinateOccupate)
                    {
                        DataGridViewCell cella = dgvMappa.Rows[item.Y].Cells[item.X];
                        InfoCella infoCella = (InfoCella)cella.Tag;
                        infoCella.AggiugiRiferimentoNave(nave);
                        infoCella.stato = 1;
                        cella.Style.BackColor = infoCella.OttieniColoreDaStato();
                        cella.Tag = infoCella;
                    }

                    lst_select_boats.Items.RemoveAt(lst_select_boats.SelectedIndex);

                    // Ricalcola le restrizioni per la prossima nave
                    if (lst_select_boats.Items.Count > 0)
                    {
                        lst_select_boats.SelectedIndex = 0; // Seleziona la prima nave rimasta
                    }
                    else
                    {
                        //Rimuovo effetto rosso sulla griglia
                        MostraRestrizioniPosizionamento(0, verticale); // Pulisce il rosso
                    }
                }
                else
                {
                    MessageBox.Show("Posizione non valida! La nave si sovrappone ad un'altra.");
                }
            }
        }
        private bool TentaPosizionamento(int lunghezza, int startX, int startY, bool verticale)
        {
            const int DimensioneGriglia = 10;

            // 1. Pre-validazione (fuori griglia - dovrebbe essere già gestita dal colore rosso, ma è sicurezza)
            if (verticale)
            {
                if (startY + lunghezza > DimensioneGriglia) return false;
            }
            else
            {
                if (startX + lunghezza > DimensioneGriglia) return false;
            }

            // 2. Validazione Sovrapposizione e Registrazione
            List<(int X, int Y)> coordinateNave = new List<(int X, int Y)>();

            for (int i = 0; i < lunghezza; i++)
            {
                int x = verticale ? startX : startX + i;
                int y = verticale ? startY + i : startY;

                DataGridViewCell cella = dgvMappa.Rows[y].Cells[x];
                InfoCella info = (InfoCella)cella.Tag;

                // Se la cella non è Acqua (stato 0), c'è una sovrapposizione!
                if (info.stato != 0)
                {
                    return false; // Posizionamento non valido per sovrapposizione
                }

                coordinateNave.Add((x, y));
            }

            // 3. Esecuzione (Se la validazione è OK)
            // Qui si dovrebbe creare l'oggetto Nave e registrarlo, ma per ora aggiorniamo InfoCella direttamente

            foreach ((int x, int y) in coordinateNave)
            {
                DataGridViewCell cella = dgvMappa.Rows[y].Cells[x];
                InfoCella info = (InfoCella)cella.Tag;

                info.stato = 1; // Stato: Parte di Nave (Non colpita)
                cella.Style.BackColor = info.OttieniColoreDaStato(); // Grey/Colore nave
            }

            // Nota: Avresti bisogno di una struttura per registrare che queste celle (stato 1) 
            // appartengono a una singola Nave per l'annullamento. Per semplicità, vedremo il modo di risalire.

            return true;
        }
        private List<(int X, int Y)> Ritornaposizionamento(int lunghezza, int startX, int startY, bool verticale)
        {
            const int DimensioneGriglia = 10;
            List<(int X, int Y)> coordinateProposte = new List<(int, int)>();

            // 1. Validazione preliminare: controlla se la nave esce dai bordi della griglia
            if (verticale)
            {
                if (startY + lunghezza > DimensioneGriglia) return null; // Fuori griglia
            }
            else
            {
                if (startX + lunghezza > DimensioneGriglia) return null; // Fuori griglia
            }

            // 2. Validazione sovrapposizione: controlla se le celle sono già occupate
            for (int i = 0; i < lunghezza; i++)
            {
                int x = verticale ? startX : startX + i;
                int y = verticale ? startY + i : startY;

                coordinateProposte.Add((x, y));
            }

            return coordinateProposte;
        }
        private void AnnullaPosizionamento(int clickX, int clickY)
        {
            DataGridViewCell cellaCliccata = dgvMappa.Rows[clickY].Cells[clickX];
            InfoCella info = (InfoCella)cellaCliccata.Tag;

            Ship naveDaRimuovere = info.NaveRiferimento;
            // Controllo: Il click destro deve avvenire su una cella dove non è presente una nave;
            if ((info.stato != 1 || naveDaRimuovere==null))
            {
                MessageBox.Show("Click destro deve avvenire su una nave posizionata e rilevata.");
                return;
            }
            foreach (var item in naveDaRimuovere.CoordinateOccupate)
            {
                DataGridViewCell cella = dgvMappa.Rows[item.Y].Cells[item.X];
                InfoCella infoCella = (InfoCella)cella.Tag;
                infoCella.RimuoviRiferimentoNave();
                infoCella.stato = 0; // Imposta lo stato ad Acqua
                cella.Style.BackColor = infoCella.OttieniColoreDaStato();
                cella.Tag = infoCella;
            }


            string nomeNaveDaAggiungere = navi.FirstOrDefault(kvp => kvp.Value.Id == naveDaRimuovere.Id).Key;

            if (!string.IsNullOrEmpty(nomeNaveDaAggiungere))
            {
                // Ri-aggiungi la nave alla ListBox per poterla riposizionare
                lst_select_boats.Items.Add(nomeNaveDaAggiungere);
            }

            // Resetta le coordinate della nave per un futuro posizionamento
            naveDaRimuovere.CoordinateOccupate.Clear();

            // Seleziona l'ultima nave (quella appena aggiunta)
            if (lst_select_boats.Items.Count > 0)
            {
                lst_select_boats.SelectedIndex = lst_select_boats.Items.Count - 1;
            }
        }
        private void btt_cambia_orientamento_Click(object sender, EventArgs e)
        {
            verticale = !verticale;
            btt_cambia_orientamento.Text = verticale ? "Orizzontale" : "Verticale";
            // Recupera la lunghezza della nave selezionata
            if (lst_select_boats.SelectedIndex >= 0)
            {
                int lunghezzaSelezionata = navi[(string)lst_select_boats.SelectedItem].size;
                MostraRestrizioniPosizionamento(lunghezzaSelezionata, verticale);
            }
        }
        private void MostraRestrizioniPosizionamento(int lunghezzaNave, bool verticale)
        {
            const int DimensioneGriglia = 10;

            // Controlla che la griglia sia stata inizializzata
            if (dgvMappa == null) return;

            // controllo per ciascuna cella della griglia
            for (int riga = 0; riga < DimensioneGriglia; riga++)
            {
                for (int colonna = 0; colonna < DimensioneGriglia; colonna++)
                {
                    DataGridViewCell cella = dgvMappa.Rows[riga].Cells[colonna];
                    bool fuoriGriglia = false;

                    fuoriGriglia = verticale?  riga + lunghezzaNave > DimensioneGriglia : fuoriGriglia = colonna + lunghezzaNave > DimensioneGriglia;

                    InfoCella info = (InfoCella)cella.Tag;

                    cella.Style.BackColor = fuoriGriglia? info.stato == 0 ? Color.Red : Color.DarkRed : info.OttieniColoreDaStato();
                  
                }
            }
        }
        private void lst_select_boats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_select_boats.SelectedIndex >= 0)
            {
                int lunghezzaSelezionata = navi[(string)lst_select_boats.SelectedItem].size;
                MostraRestrizioniPosizionamento(lunghezzaSelezionata, verticale);
            }
        }
        private void ImpostaMenù()
        {
            dgvMappa = GeneraGrigliaGioco();
            dgvMappa.Location = new Point(10, 10);

            lst_select_boats.Location = new Point(480, 10);
            lst_select_boats.Size = new Size(180, 300);
            lst_select_boats.Items.Clear();
            foreach (string lunghezza in navi.Keys)
            {
                lst_select_boats.Items.Add(lunghezza);
            }

            lst_select_boats.SelectedIndexChanged += lst_select_boats_SelectedIndexChanged;

            if (lst_select_boats.Items.Count > 0)
                lst_select_boats.SelectedIndex = 0;

            btt_cambia_orientamento.Location = new Point(480, 310);
            btt_cambia_orientamento.Size = new Size(180, 40);

            btt_done.Location = new Point(480, 360);
            btt_done.Size = new Size(180, 40);

            this.Controls.Add(dgvMappa);


        }

        private void btt_done_Click(object sender, EventArgs e)
        {
            if (lst_select_boats.Items.Count > 0)
            {
                MessageBox.Show("Devi posizionare tutte le navi prima di continuare.", "Posizionamento Incompleto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Non chiudere il form
            }
            List<Ship> flottaPosizionata = navi.Values
                                         .Where(nave => nave.CoordinateOccupate != null && nave.CoordinateOccupate.Count > 0)
                                         .ToList();

            // Passo come argomenti il datagridview e la lista delle navi posizionate
            PosizionamentoEventArgs args = new PosizionamentoEventArgs(dgvMappa, flottaPosizionata);
            DialogResult = DialogResult.OK;
            this.dgvMappa.CellMouseClick -= DataGridViewMappa_CellMouseClick;
            this.Controls.Remove(dgvMappa);
            PosizionateTutteLeNavi?.Invoke(this, args);
            this.Close();
        }
    }
    public class PosizionamentoEventArgs : EventArgs
    {
        public DataGridView Griglia { get; set; }
        public List<Ship> Flotta { get; set; }

        public PosizionamentoEventArgs(DataGridView griglia, List<Ship> flotta)
        {
            this.Griglia = griglia;
            this.Flotta = flotta;
        }
    }
}
