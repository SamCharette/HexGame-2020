namespace WindowsGame
{
	partial class Game
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;


		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.labelXY = new System.Windows.Forms.Label();
            this.buttonTestBoard = new System.Windows.Forms.Button();
            this.textBoxHexBoardSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxPlayer2Type = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxPlayer1Type = new System.Windows.Forms.ComboBox();
            this.lblWInner = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelXY
            // 
            this.labelXY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelXY.AutoSize = true;
            this.labelXY.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXY.ForeColor = System.Drawing.Color.Blue;
            this.labelXY.Location = new System.Drawing.Point(1107, 27);
            this.labelXY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelXY.Name = "labelXY";
            this.labelXY.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelXY.Size = new System.Drawing.Size(0, 17);
            this.labelXY.TabIndex = 1;
            this.labelXY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonTestBoard
            // 
            this.buttonTestBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTestBoard.Location = new System.Drawing.Point(1086, 638);
            this.buttonTestBoard.Margin = new System.Windows.Forms.Padding(4);
            this.buttonTestBoard.Name = "buttonTestBoard";
            this.buttonTestBoard.Size = new System.Drawing.Size(109, 32);
            this.buttonTestBoard.TabIndex = 3;
            this.buttonTestBoard.Text = "Test Board";
            this.buttonTestBoard.UseVisualStyleBackColor = true;
            this.buttonTestBoard.Click += new System.EventHandler(this.buttonTestBoard_Click);
            // 
            // textBoxHexBoardSize
            // 
            this.textBoxHexBoardSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHexBoardSize.Location = new System.Drawing.Point(1086, 558);
            this.textBoxHexBoardSize.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxHexBoardSize.Name = "textBoxHexBoardSize";
            this.textBoxHexBoardSize.Size = new System.Drawing.Size(108, 22);
            this.textBoxHexBoardSize.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1082, 538);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Board Size";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1107, 11);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Mouse X,Y";
            // 
            // comboBoxPlayer2Type
            // 
            this.comboBoxPlayer2Type.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPlayer2Type.FormattingEnabled = true;
            this.comboBoxPlayer2Type.Items.AddRange(new object[] {
            "Random AI",
            "Human"});
            this.comboBoxPlayer2Type.Location = new System.Drawing.Point(1086, 412);
            this.comboBoxPlayer2Type.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxPlayer2Type.Name = "comboBoxPlayer2Type";
            this.comboBoxPlayer2Type.Size = new System.Drawing.Size(119, 24);
            this.comboBoxPlayer2Type.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1086, 327);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 17);
            this.label6.TabIndex = 15;
            this.label6.Text = "Player 1";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1082, 391);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 17);
            this.label7.TabIndex = 17;
            this.label7.Text = "Player 2";
            // 
            // comboBoxPlayer1Type
            // 
            this.comboBoxPlayer1Type.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPlayer1Type.FormattingEnabled = true;
            this.comboBoxPlayer1Type.Items.AddRange(new object[] {
            "Random AI",
            "Human"});
            this.comboBoxPlayer1Type.Location = new System.Drawing.Point(1086, 363);
            this.comboBoxPlayer1Type.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxPlayer1Type.Name = "comboBoxPlayer1Type";
            this.comboBoxPlayer1Type.Size = new System.Drawing.Size(119, 24);
            this.comboBoxPlayer1Type.TabIndex = 18;
            // 
            // lblWInner
            // 
            this.lblWInner.AutoSize = true;
            this.lblWInner.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWInner.Location = new System.Drawing.Point(320, 602);
            this.lblWInner.Name = "lblWInner";
            this.lblWInner.Size = new System.Drawing.Size(177, 39);
            this.lblWInner.TabIndex = 19;
            this.lblWInner.Text = "Winner is: ";
            this.lblWInner.Visible = false;
            // 
            // Game
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1214, 676);
            this.Controls.Add(this.lblWInner);
            this.Controls.Add(this.comboBoxPlayer1Type);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxPlayer2Type);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelXY);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxHexBoardSize);
            this.Controls.Add(this.buttonTestBoard);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Game";
            this.Text = "TestForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelXY;
		private System.Windows.Forms.Button buttonTestBoard;
		private System.Windows.Forms.TextBox textBoxHexBoardSize;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBoxPlayer2Type;
		private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxPlayer1Type;
        private System.Windows.Forms.Label lblWInner;
    }
}