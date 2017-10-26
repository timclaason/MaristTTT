namespace TicTacToe
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
            this.label1 = new System.Windows.Forms.Label();
            this._txtIPAddress = new TicTacToe.UI.Controls.SmartTextBox();
            this._btnStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this._cmbSymbol = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 335);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server IP";
            // 
            // _txtIPAddress
            // 
            this._txtIPAddress.Location = new System.Drawing.Point(102, 332);
            this._txtIPAddress.Name = "_txtIPAddress";
            this._txtIPAddress.Size = new System.Drawing.Size(163, 20);
            this._txtIPAddress.TabIndex = 1;
            this._txtIPAddress.Text = "192.168.1.3";
            // 
            // _btnStart
            // 
            this._btnStart.Location = new System.Drawing.Point(271, 330);
            this._btnStart.Name = "_btnStart";
            this._btnStart.Size = new System.Drawing.Size(75, 23);
            this._btnStart.TabIndex = 2;
            this._btnStart.Text = "Start Game";
            this._btnStart.UseVisualStyleBackColor = true;
            this._btnStart.Click += new System.EventHandler(this._btnStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 364);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Desired Symbol";
            // 
            // _cmbSymbol
            // 
            this._cmbSymbol.FormattingEnabled = true;
            this._cmbSymbol.Items.AddRange(new object[] {
            "X",
            "O",
            "2PLAYER"});
            this._cmbSymbol.Location = new System.Drawing.Point(102, 358);
            this._cmbSymbol.Name = "_cmbSymbol";
            this._cmbSymbol.Size = new System.Drawing.Size(129, 21);
            this._cmbSymbol.TabIndex = 4;
            this._cmbSymbol.Text = "X";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 398);
            this.Controls.Add(this._cmbSymbol);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._btnStart);
            this.Controls.Add(this._txtIPAddress);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Tic Tac Toe Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private TicTacToe.UI.Controls.SmartTextBox _txtIPAddress;
        private System.Windows.Forms.Button _btnStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox _cmbSymbol;
    }
}

