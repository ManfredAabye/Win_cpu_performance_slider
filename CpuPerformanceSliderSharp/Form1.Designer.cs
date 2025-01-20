namespace CpuPerformanceSlider
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            trackBarMin = new TrackBar();
            trackBarMax = new TrackBar();
            labelMin = new Label();
            labelMax = new Label();
            buttonApply = new Button();
            ((System.ComponentModel.ISupportInitialize)trackBarMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMax).BeginInit();
            SuspendLayout();
            // 
            // trackBarMin
            // 
            trackBarMin.Location = new Point(12, 12);
            trackBarMin.Maximum = 100;
            trackBarMin.Minimum = 5;
            trackBarMin.Name = "trackBarMin";
            trackBarMin.Size = new Size(260, 90);
            trackBarMin.TabIndex = 0;
            trackBarMin.Value = 5;
            trackBarMin.Scroll += trackBarMin_Scroll;
            // 
            // trackBarMax
            // 
            trackBarMax.Location = new Point(12, 63);
            trackBarMax.Maximum = 100;
            trackBarMax.Minimum = 5;
            trackBarMax.Name = "trackBarMax";
            trackBarMax.Size = new Size(260, 90);
            trackBarMax.TabIndex = 1;
            trackBarMax.Value = 100;
            trackBarMax.Scroll += trackBarMax_Scroll;
            // 
            // labelMin
            // 
            labelMin.AutoSize = true;
            labelMin.Location = new Point(12, 44);
            labelMin.Name = "labelMin";
            labelMin.Size = new Size(333, 32);
            labelMin.TabIndex = 2;
            labelMin.Text = "Mindestprozessorleistung: 5%";
            // 
            // labelMax
            // 
            labelMax.AutoSize = true;
            labelMax.Location = new Point(12, 95);
            labelMax.Name = "labelMax";
            labelMax.Size = new Size(362, 32);
            labelMax.TabIndex = 3;
            labelMax.Text = "Maximalprozessorleistung: 100%";
            // 
            // buttonApply
            // 
            buttonApply.Location = new Point(12, 120);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(260, 23);
            buttonApply.TabIndex = 4;
            buttonApply.Text = "Einstellungen anwenden";
            buttonApply.UseVisualStyleBackColor = true;
            buttonApply.Click += buttonApply_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(284, 155);
            Controls.Add(buttonApply);
            Controls.Add(labelMax);
            Controls.Add(labelMin);
            Controls.Add(trackBarMax);
            Controls.Add(trackBarMin);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "CPU Leistungseinstellungen";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)trackBarMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMax).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TrackBar trackBarMin;
        private System.Windows.Forms.TrackBar trackBarMax;
        private System.Windows.Forms.Label labelMin;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.Button buttonApply;
    }
}
