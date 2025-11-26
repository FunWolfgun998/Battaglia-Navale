namespace Battaglia_navale_Eventi_Form
{
    partial class Battaglia_navale
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_turn = new System.Windows.Forms.Label();
            this.lbl_n_moves = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_turn
            // 
            this.lbl_turn.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.06154F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_turn.Location = new System.Drawing.Point(7, 7);
            this.lbl_turn.Name = "lbl_turn";
            this.lbl_turn.Size = new System.Drawing.Size(270, 40);
            this.lbl_turn.TabIndex = 0;
            this.lbl_turn.Text = "Turno :";
            this.lbl_turn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_n_moves
            // 
            this.lbl_n_moves.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.06154F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_n_moves.Location = new System.Drawing.Point(0, 0);
            this.lbl_n_moves.Name = "lbl_n_moves";
            this.lbl_n_moves.Size = new System.Drawing.Size(270, 40);
            this.lbl_n_moves.TabIndex = 1;
            this.lbl_n_moves.Text = "Mosse:";
            this.lbl_n_moves.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Battaglia_navale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbl_n_moves);
            this.Controls.Add(this.lbl_turn);
            this.Name = "Battaglia_navale";
            this.Text = " ";
            this.Load += new System.EventHandler(this.Battaglia_navale_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbl_turn;
        private System.Windows.Forms.Label lbl_n_moves;
    }
}

