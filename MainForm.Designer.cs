namespace ArtificialIntelligenceIHW
{
    partial class MainForm
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
            tableLayout = new TableLayoutPanel();
            taskComboBox = new ComboBox();
            algorithmComboBox = new ComboBox();
            taskLabel = new Label();
            algLabel = new Label();
            runButton = new Button();
            optionsPanel = new GroupBox();
            mainPanel = new PictureBox();
            timeLabel = new Label();
            drawDelay = new NumericUpDown();
            tableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainPanel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)drawDelay).BeginInit();
            SuspendLayout();
            // 
            // tableLayout
            // 
            tableLayout.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayout.ColumnCount = 4;
            tableLayout.ColumnStyles.Add(new ColumnStyle());
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.ColumnStyles.Add(new ColumnStyle());
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.Controls.Add(taskComboBox, 1, 0);
            tableLayout.Controls.Add(algorithmComboBox, 3, 0);
            tableLayout.Controls.Add(taskLabel, 0, 0);
            tableLayout.Controls.Add(algLabel, 2, 0);
            tableLayout.Location = new Point(12, 12);
            tableLayout.Name = "tableLayout";
            tableLayout.Padding = new Padding(5);
            tableLayout.RowCount = 1;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayout.Size = new Size(838, 54);
            tableLayout.TabIndex = 0;
            // 
            // taskComboBox
            // 
            taskComboBox.Dock = DockStyle.Fill;
            taskComboBox.FormattingEnabled = true;
            taskComboBox.Location = new Point(94, 8);
            taskComboBox.Name = "taskComboBox";
            taskComboBox.Size = new Size(322, 28);
            taskComboBox.TabIndex = 1;
            taskComboBox.SelectedIndexChanged += OnTaskChange;
            // 
            // algorithmComboBox
            // 
            algorithmComboBox.Dock = DockStyle.Fill;
            algorithmComboBox.FormattingEnabled = true;
            algorithmComboBox.Location = new Point(508, 8);
            algorithmComboBox.Name = "algorithmComboBox";
            algorithmComboBox.Size = new Size(322, 28);
            algorithmComboBox.TabIndex = 2;
            algorithmComboBox.SelectedIndexChanged += OnAlgorithmChange;
            // 
            // taskLabel
            // 
            taskLabel.Location = new Point(8, 5);
            taskLabel.Name = "taskLabel";
            taskLabel.Size = new Size(80, 31);
            taskLabel.TabIndex = 0;
            taskLabel.Text = "Task:";
            taskLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // algLabel
            // 
            algLabel.Location = new Point(422, 5);
            algLabel.Name = "algLabel";
            algLabel.Size = new Size(80, 31);
            algLabel.TabIndex = 0;
            algLabel.Text = "Algorithm:";
            algLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // runButton
            // 
            runButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            runButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            runButton.Location = new Point(551, 550);
            runButton.Name = "runButton";
            runButton.Size = new Size(299, 51);
            runButton.TabIndex = 4;
            runButton.Text = "Run";
            runButton.UseVisualStyleBackColor = true;
            runButton.Click += OnRun;
            // 
            // optionsPanel
            // 
            optionsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            optionsPanel.Location = new Point(551, 72);
            optionsPanel.Name = "optionsPanel";
            optionsPanel.Padding = new Padding(3, 15, 3, 3);
            optionsPanel.Size = new Size(299, 438);
            optionsPanel.TabIndex = 5;
            optionsPanel.TabStop = false;
            optionsPanel.Text = "{Algorithm} options";
            // 
            // mainPanel
            // 
            mainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            mainPanel.BackColor = SystemColors.ControlLightLight;
            mainPanel.BorderStyle = BorderStyle.FixedSingle;
            mainPanel.Location = new Point(12, 72);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(533, 529);
            mainPanel.SizeMode = PictureBoxSizeMode.StretchImage;
            mainPanel.TabIndex = 6;
            mainPanel.TabStop = false;
            // 
            // timeLabel
            // 
            timeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            timeLabel.AutoSize = true;
            timeLabel.Location = new Point(551, 518);
            timeLabel.Name = "timeLabel";
            timeLabel.Size = new Size(87, 20);
            timeLabel.TabIndex = 7;
            timeLabel.Text = "Draw delay:";
            // 
            // drawDelay
            // 
            drawDelay.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            drawDelay.Location = new Point(740, 516);
            drawDelay.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            drawDelay.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            drawDelay.Name = "drawDelay";
            drawDelay.Size = new Size(110, 27);
            drawDelay.TabIndex = 8;
            drawDelay.UpDownAlign = LeftRightAlignment.Left;
            drawDelay.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(862, 613);
            Controls.Add(drawDelay);
            Controls.Add(timeLabel);
            Controls.Add(mainPanel);
            Controls.Add(optionsPanel);
            Controls.Add(runButton);
            Controls.Add(tableLayout);
            Name = "MainForm";
            Text = "MainForm";
            tableLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainPanel).EndInit();
            ((System.ComponentModel.ISupportInitialize)drawDelay).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel tableLayout;
        private Label taskLabel;
        private Label algLabel;
        private ComboBox taskComboBox;
        private Button runButton;
        private ComboBox algorithmComboBox;
        private GroupBox optionsPanel;
        private PictureBox mainPanel;
        private Label timeLabel;
        private NumericUpDown drawDelay;
    }
}