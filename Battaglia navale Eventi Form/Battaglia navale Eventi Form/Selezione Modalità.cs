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
    public partial class Selezione_Modalità : Form
    {
        public Selezione_Modalità()
        {
            InitializeComponent();
        }

        private void Selezione_Modalità_Load(object sender, EventArgs e)
        {
            this.Size = new Size(500, 300);
            btt_PvP.Size = new Size(150, 40);
            btt_PvE.Size = new Size(150, 40);
            lbl_gameplaytitle.Size = new Size(300, 40);

            btt_PvP.Location= new Point((this.Size.Width-btt_PvP.Size.Width-40)/ 2, 100);
            btt_PvE.Location = new Point((this.Size.Width - btt_PvE.Size.Width-40) / 2, 150);
            lbl_gameplaytitle.Location = new Point((this.Size.Width - lbl_gameplaytitle.Size.Width) / 2, 30);
        }

        // Evento per il pulsante "Giocatore vs Giocatore"
        private void btnPvP_Click(object sender, EventArgs e)
        {
            Battaglia_navale gameForm = new Battaglia_navale(true);

            gameForm.Show();

            this.Hide();
        }

        // Evento per il pulsante "Giocatore vs Computer"
        private void btnPvE_Click(object sender, EventArgs e)
        {
            Battaglia_navale gameForm = new Battaglia_navale(false);

            gameForm.Show();

            this.Hide();
        }
    }
}
