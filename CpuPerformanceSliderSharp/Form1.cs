using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CpuPerformanceSlider
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SetCpuPerformance(int minPercentage, int maxPercentage)
        {
            try
            {
                string minCommandAc = $"powercfg -setacvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMIN {minPercentage}";
                string minCommandDc = $"powercfg -setdcvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMIN {minPercentage}";
                string maxCommandAc = $"powercfg -setacvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMAX {maxPercentage}";
                string maxCommandDc = $"powercfg -setdcvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMAX {maxPercentage}";

                RunCommand(minCommandAc);
                RunCommand(minCommandDc);
                RunCommand(maxCommandAc);
                RunCommand(maxCommandDc);

                // Aktiviere den aktuellen Energiesparplan erneut, um die Änderungen sofort wirksam zu machen
                string activateScheme = "powercfg -setactive SCHEME_CURRENT";
                RunCommand(activateScheme);

                MessageBox.Show($"Mindestprozessorleistung auf {minPercentage}% und Maximalprozessorleistung auf {maxPercentage}% gesetzt.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Setzen der Prozessorleistung: {ex.Message}");
            }
        }

        private void RunCommand(string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var process = Process.Start(processInfo);
            process.WaitForExit();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            int minPercentage = trackBarMin.Value;
            int maxPercentage = trackBarMax.Value;
            SetCpuPerformance(minPercentage, maxPercentage);
        }

        private void trackBarMin_Scroll(object sender, EventArgs e)
        {
            labelMin.Text = $"Mindestprozessorleistung: {trackBarMin.Value}%";
        }

        private void trackBarMax_Scroll(object sender, EventArgs e)
        {
            labelMax.Text = $"Maximalprozessorleistung: {trackBarMax.Value}%";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
