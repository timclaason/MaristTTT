namespace TicTacToeServer
{
    partial class Form1
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
            this._txtOutput = new System.Windows.Forms.TextBox();
            this._btnRestart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _txtOutput
            // 
            this._txtOutput.Location = new System.Drawing.Point(12, 49);
            this._txtOutput.Multiline = true;
            this._txtOutput.Name = "_txtOutput";
            this._txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._txtOutput.Size = new System.Drawing.Size(554, 372);
            this._txtOutput.TabIndex = 0;
            // 
            // _btnRestart
            // 
            this._btnRestart.Location = new System.Drawing.Point(491, 12);
            this._btnRestart.Name = "_btnRestart";
            this._btnRestart.Size = new System.Drawing.Size(75, 23);
            this._btnRestart.TabIndex = 1;
            this._btnRestart.Text = "Restart";
            this._btnRestart.UseVisualStyleBackColor = true;
            this._btnRestart.Click += new System.EventHandler(this._btnRestart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 433);
            this.Controls.Add(this._btnRestart);
            this.Controls.Add(this._txtOutput);
            this.Name = "Form1";
            this.Text = "Tic Tac Toe Server";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _txtOutput;
        private System.Windows.Forms.Button _btnRestart;
    }
}

