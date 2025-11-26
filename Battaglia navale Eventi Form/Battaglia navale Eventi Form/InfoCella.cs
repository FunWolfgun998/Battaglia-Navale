using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battaglia_navale_Eventi_Form
{
    internal class InfoCella
    {
        public int stato { get; set; }
        /*
         Valore	Significato
            0	Acqua (Nessuna nave)
            1	Parte di Nave (Non colpita)
            2	Mancato (Colpo a vuoto)
            3	Colpito (Parte di nave colpita)
         */
        public int x { get; private set; }
        public int y { get; private set; }

        public Ship NaveRiferimento { get; set; }
        public InfoCella() 
        {
            this.stato = 0;
            this.x = -1;
            this.y = -1;
            this.NaveRiferimento = null; // null per rappresenta la mancanza di una nave
        }

        public InfoCella(int x, int y): this()
        {
            this.x = x;
            this.y = y;
            this.stato = 0;
        }
        public InfoCella(int x, int y, int stato) : this(x, y)
        {
            this.stato = stato;
        }
        public void AggiugiRiferimentoNave(Ship nave)
        {
            this.stato = 1;
            this.NaveRiferimento = nave;
        }
        public void RimuoviRiferimentoNave()
        {
            if(this.stato == 1)
                this.stato = 0;
            else this.stato = 2; 
            this.NaveRiferimento = null;
        }
        public Color OttieniColoreDaStato()
        {
            switch (this.stato)
            {
                case 0:
                    return Color.LightBlue; // Acqua
                case 1:
                    return Color.GhostWhite; // Parte di Nave (Colore per nasconderla durante il gioco o Grey per posizionamento)
                case 2:
                    return Color.DarkBlue;  // Mancato (Colpo a vuoto)
                case 3:
                    return Color.Red;       // Colpito (Parte di nave colpita)
                default:
                    return Color.LightGray; // Stato non riconosciuto
            }
        }
    }
}
