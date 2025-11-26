namespace Battaglia_navale_Eventi_Form
{
    partial class Selezione_Modalità
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btt_PvP = new System.Windows.Forms.Button();
            this.btt_PvE = new System.Windows.Forms.Button();
            this.lbl_gameplaytitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btt_PvP
            // 
            this.btt_PvP.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btt_PvP.Location = new System.Drawing.Point(0, 0);
            this.btt_PvP.Name = "btt_PvP";
            this.btt_PvP.Size = new System.Drawing.Size(10, 10);
            this.btt_PvP.TabIndex = 0;
            this.btt_PvP.Text = "Player Vs Player";
            this.btt_PvP.UseVisualStyleBackColor = true;
            this.btt_PvP.Click += new System.EventHandler(this.btnPvP_Click);
            // 
            // btt_PvE
            // 
            this.btt_PvE.Location = new System.Drawing.Point(0, 0);
            this.btt_PvE.Name = "btt_PvE";
            this.btt_PvE.Size = new System.Drawing.Size(10, 10);
            this.btt_PvE.TabIndex = 1;
            this.btt_PvE.Text = "Player Vs CPU";
            this.btt_PvE.UseVisualStyleBackColor = true;
            this.btt_PvE.Click += new System.EventHandler(this.btnPvE_Click);
            // 
            // lbl_gameplaytitle
            // 
            this.lbl_gameplaytitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbl_gameplaytitle.Font = new System.Drawing.Font("Microsoft YaHei", 19.93846F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_gameplaytitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_gameplaytitle.Location = new System.Drawing.Point(0, 0);
            this.lbl_gameplaytitle.Name = "lbl_gameplaytitle";
            this.lbl_gameplaytitle.Size = new System.Drawing.Size(100, 23);
            this.lbl_gameplaytitle.TabIndex = 2;
            this.lbl_gameplaytitle.Text = "Select gameplay";
            // 
            // Selezione_Modalità
            // 
            this.AcceptButton = this.btt_PvP;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btt_PvE);
            this.Controls.Add(this.btt_PvP);
            this.Controls.Add(this.lbl_gameplaytitle);
            this.Name = "Selezione_Modalità";
            this.Text = "Selezione_Modalità";
            this.Load += new System.EventHandler(this.Selezione_Modalità_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btt_PvP;
        private System.Windows.Forms.Button btt_PvE;
        private System.Windows.Forms.Label lbl_gameplaytitle;
    }
}