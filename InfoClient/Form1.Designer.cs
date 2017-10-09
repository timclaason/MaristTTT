namespace InfoClient
{
    partial class InfoClientForm
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
            this._cmbRequest = new System.Windows.Forms.ComboBox();
            this._btnSendRequest = new System.Windows.Forms.Button();
            this._txtOutput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._txtIPAddress = new TicTacToe.UI.Controls.SmartTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Request To Send";
            // 
            // _cmbRequest
            // 
            this._cmbRequest.FormattingEnabled = true;
            this._cmbRequest.Items.AddRange(new object[] {
            "Date",
            "Day",
            "Time",
            "Year"});
            this._cmbRequest.Location = new System.Drawing.Point(109, 38);
            this._cmbRequest.Name = "_cmbRequest";
            this._cmbRequest.Size = new System.Drawing.Size(186, 21);
            this._cmbRequest.TabIndex = 1;
            // 
            // _btnSendRequest
            // 
            this._btnSendRequest.Location = new System.Drawing.Point(176, 65);
            this._btnSendRequest.Name = "_btnSendRequest";
            this._btnSendRequest.Size = new System.Drawing.Size(119, 23);
            this._btnSendRequest.TabIndex = 2;
            this._btnSendRequest.Text = "Send Request";
            this._btnSendRequest.UseVisualStyleBackColor = true;
            this._btnSendRequest.Click += new System.EventHandler(this._btnSendRequest_Click);
            // 
            // _txtOutput
            // 
            this._txtOutput.Location = new System.Drawing.Point(15, 115);
            this._txtOutput.Multiline = true;
            this._txtOutput.Name = "_txtOutput";
            this._txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._txtOutput.Size = new System.Drawing.Size(562, 214);
            this._txtOutput.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Log";
            // 
            // _txtIPAddress
            // 
            this._txtIPAddress.Location = new System.Drawing.Point(109, 12);
            this._txtIPAddress.Name = "_txtIPAddress";
            this._txtIPAddress.Size = new System.Drawing.Size(163, 20);
            this._txtIPAddress.TabIndex = 6;
            this._txtIPAddress.Text = "192.168.1.3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Server IP";
            // 
            // InfoClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 347);
            this.Controls.Add(this._txtIPAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._txtOutput);
            this.Controls.Add(this._btnSendRequest);
            this.Controls.Add(this._cmbRequest);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "InfoClientForm";
            this.Text = "Info Client";
            this.Load += new System.EventHandler(this.InfoClientForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _cmbRequest;
        private System.Windows.Forms.Button _btnSendRequest;
        private System.Windows.Forms.TextBox _txtOutput;
        private System.Windows.Forms.Label label2;
        private TicTacToe.UI.Controls.SmartTextBox _txtIPAddress;
        private System.Windows.Forms.Label label3;
    }
}

