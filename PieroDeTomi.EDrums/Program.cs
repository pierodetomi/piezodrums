using System.Diagnostics;

namespace PieroDeTomi.EDrums.Console
{
    public class Program
    {
        private static EDrums eDrums;

        [STAThread]
        public static void Main()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            eDrums = new EDrums(
                midiVirtualDeviceName: "PDT eDrums",
                audioDeviceSearchKey: "Focusrite USB",
                audioDeviceSampleRate: 192000, // 44100,
                maxWaveImpulseValue: 1.5f);

            System.Console.ReadLine();
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            eDrums.Dispose();
        }
    }
}