using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;


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
            if (process != null)
            {
                process.WaitForExit();
            }
            else
            {
                throw new InvalidOperationException("Der Prozess konnte nicht gestartet werden.");
            }
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

        private int ExtractValueFromOutput(string output)
        {
            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.Contains("Power Setting Index") || line.Contains("Energieeinstellungsindex"))
                {
                    var parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length == 2)
                    {
                        string hexPart = parts[1].Trim();
                        if (hexPart.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
                            int.TryParse(hexPart.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out int value))
                        {
                            return Math.Max(value, 5); // Verhindere zu kleine Werte
                        }
                    }
                }
            }

            throw new Exception("Konnte Power Setting Index nicht lesen.");
        }

        private int ReadCpuPerformanceValue(string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(processInfo))
            {
                using (var reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    MessageBox.Show("Powercfg-Ausgabe:\n" + result); // <-- Nur zum Debuggen
                    int value = ExtractValueFromOutput(result);
                    return value;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                int min = ReadProcessorSetting(GUID_PROCESSOR_THROTTLE_MIN);
                int max = ReadProcessorSetting(GUID_PROCESSOR_THROTTLE_MAX);

                // Werte in TrackBars übernehmen, mit Absicherung
                trackBarMin.Value = Math.Min(Math.Max(trackBarMin.Minimum, min), trackBarMin.Maximum);
                trackBarMax.Value = Math.Min(Math.Max(trackBarMax.Minimum, max), trackBarMax.Maximum);

                labelMin.Text = $"Mindestprozessorleistung: {trackBarMin.Value}%";
                labelMax.Text = $"Maximalprozessorleistung: {trackBarMax.Value}%";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Auslesen der CPU-Werte:\n" + ex.Message);
            }
        }


        // Windows-API: PowerReadACValue
        [DllImport("powrprof.dll", SetLastError = true)]
        private static extern uint PowerReadACValue(
            IntPtr RootPowerKey,
            ref Guid SchemeGuid,
            ref Guid SubGroupOfPowerSettingsGuid,
            ref Guid PowerSettingGuid,
            out uint Type,
            IntPtr Buffer,
            ref uint BufferSize);

        // GUIDs aus der Windows Doku
        private static Guid GUID_PROCESSOR_SETTINGS_SUBGROUP = new Guid("54533251-82be-4824-96c1-47b60b740d00");
        private static Guid GUID_PROCESSOR_THROTTLE_MIN = new Guid("893dee8e-2bef-41e0-89c6-b55d0929964c");
        private static Guid GUID_PROCESSOR_THROTTLE_MAX = new Guid("bc5038f7-23e0-4960-96da-33abaf5935ec");

        // Liest den aktuellen AC-Wert für eine bestimmte Einstellung
        private int ReadProcessorSetting(Guid settingGuid)
        {
            Guid schemeGuid = GetActiveSchemeGuid();
            uint valueType;
            uint value = 0;
            uint valueSize = sizeof(uint);
            IntPtr valuePtr = Marshal.AllocHGlobal((int)valueSize);

            try
            {
                uint result = PowerReadACValue(
                    IntPtr.Zero,
                    ref schemeGuid,
                    ref GUID_PROCESSOR_SETTINGS_SUBGROUP,
                    ref settingGuid,
                    out valueType,
                    valuePtr,
                    ref valueSize);

                if (result != 0)
                {
                    throw new Exception($"PowerReadACValue fehlgeschlagen (Code {result})");
                }

                value = (uint)Marshal.ReadInt32(valuePtr);
                return (int)value;
            }
            finally
            {
                Marshal.FreeHGlobal(valuePtr);
            }
        }

        // Liest die GUID des aktiven Energiesparplans
        private Guid GetActiveSchemeGuid()
        {
            uint size = 16;
            IntPtr guidPtr = Marshal.AllocHGlobal((int)size);

            try
            {
                uint result = PowerGetActiveScheme(IntPtr.Zero, out guidPtr);
                if (result != 0)
                    throw new Exception("PowerGetActiveScheme fehlgeschlagen");

                Guid schemeGuid = (Guid)Marshal.PtrToStructure(guidPtr, typeof(Guid));
                return schemeGuid;
            }
            finally
            {
                Marshal.FreeHGlobal(guidPtr);
            }
        }

        [DllImport("powrprof.dll", SetLastError = true)]
        private static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, out IntPtr ActivePolicyGuid);


    }
}
