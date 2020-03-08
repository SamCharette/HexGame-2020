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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.player1Metrics = new System.Windows.Forms.Label();
            this.player2Metrics = new System.Windows.Forms.Label();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.lblBlueTime = new System.Windows.Forms.Label();
            this.lblRedTime = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelXY
            // 
            this.labelXY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelXY.AutoSize = true;
            this.labelXY.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXY.ForeColor = System.Drawing.Color.Blue;
            this.labelXY.Location = new System.Drawing.Point(975, 60);
            this.labelXY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelXY.Name = "labelXY";
            this.labelXY.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelXY.Size = new System.Drawing.Size(94, 17);
            this.labelXY.TabIndex = 1;
            this.labelXY.Text = "Current Hex";
            this.labelXY.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonTestBoard
            // 
            this.buttonTestBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTestBoard.Location = new System.Drawing.Point(1085, 638);
            this.buttonTestBoard.Margin = new System.Windows.Forms.Padding(4);
            this.buttonTestBoard.Name = "buttonTestBoard";
            this.buttonTestBoard.Size = new System.Drawing.Size(109, 32);
            this.buttonTestBoard.TabIndex = 3;
            this.buttonTestBoard.Text = "Play Game!";
            this.buttonTestBoard.UseVisualStyleBackColor = true;
            this.buttonTestBoard.Click += new System.EventHandler(this.buttonTestBoard_Click);
            // 
            // textBoxHexBoardSize
            // 
            this.textBoxHexBoardSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHexBoardSize.Location = new System.Drawing.Point(1085, 558);
            this.textBoxHexBoardSize.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxHexBoardSize.Name = "textBoxHexBoardSize";
            this.textBoxHexBoardSize.Size = new System.Drawing.Size(108, 22);
            this.textBoxHexBoardSize.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1083, 538);
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
            this.label4.Location = new System.Drawing.Point(975, 32);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Mouse X,Y";
            // 
            // comboBoxPlayer2Type
            // 
            this.comboBoxPlayer2Type.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.comboBoxPlayer2Type.FormattingEnabled = true;
            this.comboBoxPlayer2Type.Location = new System.Drawing.Point(969, 344);
            this.comboBoxPlayer2Type.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxPlayer2Type.Name = "comboBoxPlayer2Type";
            this.comboBoxPlayer2Type.Size = new System.Drawing.Size(224, 24);
            this.comboBoxPlayer2Type.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(966, 97);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 17);
            this.label6.TabIndex = 15;
            this.label6.Text = "Player 1";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(967, 323);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 17);
            this.label7.TabIndex = 17;
            this.label7.Text = "Player 2";
            // 
            // comboBoxPlayer1Type
            // 
            this.comboBoxPlayer1Type.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.comboBoxPlayer1Type.FormattingEnabled = true;
            this.comboBoxPlayer1Type.Location = new System.Drawing.Point(966, 118);
            this.comboBoxPlayer1Type.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxPlayer1Type.Name = "comboBoxPlayer1Type";
            this.comboBoxPlayer1Type.Size = new System.Drawing.Size(227, 24);
            this.comboBoxPlayer1Type.TabIndex = 18;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "xml";
            this.openFileDialog1.InitialDirectory = "C:\\GameFiles";
            this.openFileDialog1.ShowHelp = true;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xml";
            this.saveFileDialog1.FileName = "game.xml";
            this.saveFileDialog1.InitialDirectory = "C:\\GameFiles";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1213, 28);
            this.menuStrip1.TabIndex = 23;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveGameToolStripMenuItem,
            this.loadGameToolStripMenuItem,
            this.reloadConfigurationToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveGameToolStripMenuItem
            // 
            this.saveGameToolStripMenuItem.Name = "saveGameToolStripMenuItem";
            this.saveGameToolStripMenuItem.Size = new System.Drawing.Size(234, 26);
            this.saveGameToolStripMenuItem.Text = "Save Game";
            this.saveGameToolStripMenuItem.Click += new System.EventHandler(this.saveGameToolStripMenuItem_Click);
            // 
            // loadGameToolStripMenuItem
            // 
            this.loadGameToolStripMenuItem.Name = "loadGameToolStripMenuItem";
            this.loadGameToolStripMenuItem.Size = new System.Drawing.Size(234, 26);
            this.loadGameToolStripMenuItem.Text = "Load Game";
            this.loadGameToolStripMenuItem.Click += new System.EventHandler(this.loadGameToolStripMenuItem_Click);
            // 
            // reloadConfigurationToolStripMenuItem
            // 
            this.reloadConfigurationToolStripMenuItem.Name = "reloadConfigurationToolStripMenuItem";
            this.reloadConfigurationToolStripMenuItem.Size = new System.Drawing.Size(234, 26);
            this.reloadConfigurationToolStripMenuItem.Text = "Reload Configuration";
            this.reloadConfigurationToolStripMenuItem.Click += new System.EventHandler(this.reloadConfigurationToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(234, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(133, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // player1Metrics
            // 
            this.player1Metrics.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.player1Metrics.AutoSize = true;
            this.player1Metrics.Location = new System.Drawing.Point(966, 146);
            this.player1Metrics.Name = "player1Metrics";
            this.player1Metrics.Size = new System.Drawing.Size(109, 17);
            this.player1Metrics.TabIndex = 24;
            this.player1Metrics.Text = "Player 1 metrics";
            // 
            // player2Metrics
            // 
            this.player2Metrics.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.player2Metrics.AutoSize = true;
            this.player2Metrics.Location = new System.Drawing.Point(966, 381);
            this.player2Metrics.Name = "player2Metrics";
            this.player2Metrics.Size = new System.Drawing.Size(109, 17);
            this.player2Metrics.TabIndex = 25;
            this.player2Metrics.Text = "Player 2 metrics";
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTime.Location = new System.Drawing.Point(32, 635);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(116, 25);
            this.lblTotalTime.TabIndex = 26;
            this.lblTotalTime.Text = "Total Time: ";
            // 
            // lblBlueTime
            // 
            this.lblBlueTime.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblBlueTime.AutoSize = true;
            this.lblBlueTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBlueTime.Location = new System.Drawing.Point(382, 635);
            this.lblBlueTime.Name = "lblBlueTime";
            this.lblBlueTime.Size = new System.Drawing.Size(106, 25);
            this.lblBlueTime.TabIndex = 27;
            this.lblBlueTime.Text = "Blue Time:";
            // 
            // lblRedTime
            // 
            this.lblRedTime.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblRedTime.AutoSize = true;
            this.lblRedTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRedTime.Location = new System.Drawing.Point(692, 635);
            this.lblRedTime.Name = "lblRedTime";
            this.lblRedTime.Size = new System.Drawing.Size(102, 25);
            this.lblRedTime.TabIndex = 28;
            this.lblRedTime.Text = "Red Time:";
            // 
            // Game
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1213, 676);
            this.Controls.Add(this.lblRedTime);
            this.Controls.Add(this.lblBlueTime);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.player2Metrics);
            this.Controls.Add(this.player1Metrics);
            this.Controls.Add(this.comboBoxPlayer1Type);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxPlayer2Type);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelXY);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxHexBoardSize);
            this.Controls.Add(this.buttonTestBoard);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Game";
            this.Text = "HexGame";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label player1Metrics;
        private System.Windows.Forms.Label player2Metrics;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.Label lblBlueTime;
        private System.Windows.Forms.Label lblRedTime;
    }
}