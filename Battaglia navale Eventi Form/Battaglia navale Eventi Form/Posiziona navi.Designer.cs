namespace Battaglia_navale_Eventi_Form
{
    partial class Posiziona_navi
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
            this.lst_select_boats = new System.Windows.Forms.ListBox();
            this.btt_cambia_orientamento = new System.Windows.Forms.Button();
            this.btt_done = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lst_select_boats
            // 
            this.lst_select_boats.FormattingEnabled = true;
            this.lst_select_boats.ItemHeight = 16;
            this.lst_select_boats.Location = new System.Drawing.Point(0, 0);
            this.lst_select_boats.Name = "lst_select_boats";
            this.lst_select_boats.Size = new System.Drawing.Size(10, 4);
            this.lst_select_boats.TabIndex = 0;
            // 
            // btt_cambia_orientamento
            // 
            this.btt_cambia_orientamento.Location = new System.Drawing.Point(0, 0);
            this.btt_cambia_orientamento.Name = "btt_cambia_orientamento";
            this.btt_cambia_orientamento.Size = new System.Drawing.Size(10, 10);
            this.btt_cambia_orientamento.TabIndex = 1;
            this.btt_cambia_orientamento.Text = "Orizzontale";
            this.btt_cambia_orientamento.UseVisualStyleBackColor = true;
            this.btt_cambia_orientamento.Click += new System.EventHandler(this.btt_cambia_orientamento_Click);
            // 
            // btt_done
            // 
            this.btt_done.Location = new System.Drawing.Point(0, 0);
            this.btt_done.Name = "btt_done";
            this.btt_done.Size = new System.Drawing.Size(10, 10);
            this.btt_done.TabIndex = 2;
            this.btt_done.Text = "Done";
            this.btt_done.UseVisualStyleBackColor = true;
            this.btt_done.Click += new System.EventHandler(this.btt_done_Click);
            // 
            // Posiziona_navi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 753);
            this.ControlBox = false;
            this.Controls.Add(this.btt_done);
            this.Controls.Add(this.btt_cambia_orientamento);
            this.Controls.Add(this.lst_select_boats);
            this.Name = "Posiziona_navi";
            this.Text = "Posiziona_navi";
            this.Load += new System.EventHandler(this.Posiziona_navi_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lst_select_boats;
        private System.Windows.Forms.Button btt_cambia_orientamento;
        private System.Windows.Forms.Button btt_done;
    }
}