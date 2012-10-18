namespace PCMonitor
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
            this.components = new System.ComponentModel.Container();
            this.cbPort = new System.Windows.Forms.ComboBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.tbVystup = new System.Windows.Forms.TextBox();
            this.lbNapeti1 = new System.Windows.Forms.Label();
            this.lbTeplota1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.lbNapeti2 = new System.Windows.Forms.Label();
            this.lbNapeti3 = new System.Windows.Forms.Label();
            this.lbNapeti4 = new System.Windows.Forms.Label();
            this.lbNapeti5 = new System.Windows.Forms.Label();
            this.lbNapeti6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbTeplota2 = new System.Windows.Forms.Label();
            this.lbTeplota3 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lbAku = new System.Windows.Forms.Label();
            this.lblID = new System.Windows.Forms.Label();
            this.lblLat = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblLon = new System.Windows.Forms.Label();
            this.lblDatum = new System.Windows.Forms.Label();
            this.lblVyska = new System.Windows.Forms.Label();
            this.lblRychlost = new System.Windows.Forms.Label();
            this.lblKurz = new System.Windows.Forms.Label();
            this.lblBaseTelemetry = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbPort
            // 
            this.cbPort.FormattingEnabled = true;
            this.cbPort.Location = new System.Drawing.Point(12, 12);
            this.cbPort.Name = "cbPort";
            this.cbPort.Size = new System.Drawing.Size(83, 21);
            this.cbPort.TabIndex = 0;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(102, 9);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Open";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // tbVystup
            // 
            this.tbVystup.Location = new System.Drawing.Point(12, 229);
            this.tbVystup.MaxLength = 10;
            this.tbVystup.Multiline = true;
            this.tbVystup.Name = "tbVystup";
            this.tbVystup.ReadOnly = true;
            this.tbVystup.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbVystup.Size = new System.Drawing.Size(614, 186);
            this.tbVystup.TabIndex = 2;
            // 
            // lbNapeti1
            // 
            this.lbNapeti1.AutoSize = true;
            this.lbNapeti1.Location = new System.Drawing.Point(326, 56);
            this.lbNapeti1.Name = "lbNapeti1";
            this.lbNapeti1.Size = new System.Drawing.Size(28, 13);
            this.lbNapeti1.TabIndex = 3;
            this.lbNapeti1.Text = "0.00";
            // 
            // lbTeplota1
            // 
            this.lbTeplota1.AutoSize = true;
            this.lbTeplota1.Location = new System.Drawing.Point(25, 56);
            this.lbTeplota1.Name = "lbTeplota1";
            this.lbTeplota1.Size = new System.Drawing.Size(22, 13);
            this.lbTeplota1.TabIndex = 4;
            this.lbTeplota1.Text = "0.0";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(290, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Napětí aku:";
            // 
            // lbNapeti2
            // 
            this.lbNapeti2.AutoSize = true;
            this.lbNapeti2.Location = new System.Drawing.Point(396, 56);
            this.lbNapeti2.Name = "lbNapeti2";
            this.lbNapeti2.Size = new System.Drawing.Size(28, 13);
            this.lbNapeti2.TabIndex = 6;
            this.lbNapeti2.Text = "0.00";
            // 
            // lbNapeti3
            // 
            this.lbNapeti3.AutoSize = true;
            this.lbNapeti3.Location = new System.Drawing.Point(326, 69);
            this.lbNapeti3.Name = "lbNapeti3";
            this.lbNapeti3.Size = new System.Drawing.Size(28, 13);
            this.lbNapeti3.TabIndex = 7;
            this.lbNapeti3.Text = "0.00";
            // 
            // lbNapeti4
            // 
            this.lbNapeti4.AutoSize = true;
            this.lbNapeti4.Location = new System.Drawing.Point(396, 69);
            this.lbNapeti4.Name = "lbNapeti4";
            this.lbNapeti4.Size = new System.Drawing.Size(28, 13);
            this.lbNapeti4.TabIndex = 8;
            this.lbNapeti4.Text = "0.00";
            // 
            // lbNapeti5
            // 
            this.lbNapeti5.AutoSize = true;
            this.lbNapeti5.Location = new System.Drawing.Point(466, 69);
            this.lbNapeti5.Name = "lbNapeti5";
            this.lbNapeti5.Size = new System.Drawing.Size(28, 13);
            this.lbNapeti5.TabIndex = 9;
            this.lbNapeti5.Text = "0.00";
            // 
            // lbNapeti6
            // 
            this.lbNapeti6.AutoSize = true;
            this.lbNapeti6.Location = new System.Drawing.Point(374, 96);
            this.lbNapeti6.Name = "lbNapeti6";
            this.lbNapeti6.Size = new System.Drawing.Size(28, 13);
            this.lbNapeti6.TabIndex = 10;
            this.lbNapeti6.Text = "0.00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Teplota";
            // 
            // lbTeplota2
            // 
            this.lbTeplota2.AutoSize = true;
            this.lbTeplota2.Location = new System.Drawing.Point(65, 56);
            this.lbTeplota2.Name = "lbTeplota2";
            this.lbTeplota2.Size = new System.Drawing.Size(22, 13);
            this.lbTeplota2.TabIndex = 12;
            this.lbTeplota2.Text = "0.0";
            // 
            // lbTeplota3
            // 
            this.lbTeplota3.AutoSize = true;
            this.lbTeplota3.Location = new System.Drawing.Point(105, 56);
            this.lbTeplota3.Name = "lbTeplota3";
            this.lbTeplota3.Size = new System.Drawing.Size(22, 13);
            this.lbTeplota3.TabIndex = 13;
            this.lbTeplota3.Text = "0.0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(290, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "1.čl.:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(360, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "2.čl.:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(290, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "3.čl.:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(360, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "4.čl.:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(430, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "5.čl.:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(290, 96);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Palubní napětí:";
            // 
            // lbAku
            // 
            this.lbAku.AutoSize = true;
            this.lbAku.Location = new System.Drawing.Point(360, 38);
            this.lbAku.Name = "lbAku";
            this.lbAku.Size = new System.Drawing.Size(28, 13);
            this.lbAku.TabIndex = 22;
            this.lbAku.Text = "5.00";
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.Location = new System.Drawing.Point(198, 9);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(0, 13);
            this.lblID.TabIndex = 23;
            // 
            // lblLat
            // 
            this.lblLat.AutoSize = true;
            this.lblLat.Location = new System.Drawing.Point(28, 110);
            this.lblLat.Name = "lblLat";
            this.lblLat.Size = new System.Drawing.Size(0, 13);
            this.lblLat.TabIndex = 24;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 94);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "GPS";
            // 
            // lblLon
            // 
            this.lblLon.AutoSize = true;
            this.lblLon.Location = new System.Drawing.Point(126, 109);
            this.lblLon.Name = "lblLon";
            this.lblLon.Size = new System.Drawing.Size(0, 13);
            this.lblLon.TabIndex = 26;
            // 
            // lblDatum
            // 
            this.lblDatum.AutoSize = true;
            this.lblDatum.Location = new System.Drawing.Point(31, 139);
            this.lblDatum.Name = "lblDatum";
            this.lblDatum.Size = new System.Drawing.Size(0, 13);
            this.lblDatum.TabIndex = 27;
            // 
            // lblVyska
            // 
            this.lblVyska.AutoSize = true;
            this.lblVyska.Location = new System.Drawing.Point(31, 173);
            this.lblVyska.Name = "lblVyska";
            this.lblVyska.Size = new System.Drawing.Size(0, 13);
            this.lblVyska.TabIndex = 28;
            // 
            // lblRychlost
            // 
            this.lblRychlost.AutoSize = true;
            this.lblRychlost.Location = new System.Drawing.Point(140, 173);
            this.lblRychlost.Name = "lblRychlost";
            this.lblRychlost.Size = new System.Drawing.Size(0, 13);
            this.lblRychlost.TabIndex = 29;
            // 
            // lblKurz
            // 
            this.lblKurz.AutoSize = true;
            this.lblKurz.Location = new System.Drawing.Point(229, 172);
            this.lblKurz.Name = "lblKurz";
            this.lblKurz.Size = new System.Drawing.Size(0, 13);
            this.lblKurz.TabIndex = 30;
            // 
            // lblBaseTelemetry
            // 
            this.lblBaseTelemetry.AutoSize = true;
            this.lblBaseTelemetry.Location = new System.Drawing.Point(22, 201);
            this.lblBaseTelemetry.Name = "lblBaseTelemetry";
            this.lblBaseTelemetry.Size = new System.Drawing.Size(0, 13);
            this.lblBaseTelemetry.TabIndex = 31;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 427);
            this.Controls.Add(this.lblBaseTelemetry);
            this.Controls.Add(this.lblKurz);
            this.Controls.Add(this.lblRychlost);
            this.Controls.Add(this.lblVyska);
            this.Controls.Add(this.lblDatum);
            this.Controls.Add(this.lblLon);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblLat);
            this.Controls.Add(this.lblID);
            this.Controls.Add(this.lbAku);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbTeplota3);
            this.Controls.Add(this.lbTeplota2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbNapeti6);
            this.Controls.Add(this.lbNapeti5);
            this.Controls.Add(this.lbNapeti4);
            this.Controls.Add(this.lbNapeti3);
            this.Controls.Add(this.lbNapeti2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbTeplota1);
            this.Controls.Add(this.lbNapeti1);
            this.Controls.Add(this.tbVystup);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.cbPort);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbPort;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.TextBox tbVystup;
        private System.Windows.Forms.Label lbNapeti1;
        private System.Windows.Forms.Label lbTeplota1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbNapeti2;
        private System.Windows.Forms.Label lbNapeti3;
        private System.Windows.Forms.Label lbNapeti4;
        private System.Windows.Forms.Label lbNapeti5;
        private System.Windows.Forms.Label lbNapeti6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbTeplota2;
        private System.Windows.Forms.Label lbTeplota3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbAku;
        private System.Windows.Forms.Label lblID;
        private System.Windows.Forms.Label lblLat;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblLon;
        private System.Windows.Forms.Label lblDatum;
        private System.Windows.Forms.Label lblVyska;
        private System.Windows.Forms.Label lblRychlost;
        private System.Windows.Forms.Label lblKurz;
        private System.Windows.Forms.Label lblBaseTelemetry;
    }
}

