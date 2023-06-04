using PieroDeTomi.EDrums.Models;
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
            // TestForLoop();

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            eDrums = new EDrums(
                midiVirtualDeviceName: "PDT eDrums",
                audioDeviceSearchKey: "Focusrite USB",
                audioDeviceSampleRate: 44100,
                maxWaveImpulseValue: 1.5f);

            System.Console.ReadLine();
        }

        private static void TestForLoop()
        {
            //// Fill test array
            //var samples = new float[192];
            //for (int i = 0; i < samples.Length; i++)
            //    samples[i] = (float)new Random(i).NextDouble();

            //// Start watch
            //var watch = new Stopwatch();

            //// Test 1st method
            //var method1ms = TestMethod(() => GetWaveValue(samples, samples.Length), 10000000, watch);
            //var method2ms = TestMethod(() => GetWaveValueWithAbs(samples, samples.Length), 10000000, watch);
            //var method3ms = TestMethod(() => GetWaveValueUnrolled(samples, samples.Length), 10000000, watch);
            //var method4ms = TestMethod(() => GetWaveValueUnrolledWithAbs(samples, samples.Length), 10000000, watch);
            //var method5ms = TestMethod(() => GetWaveValueUnrolledFourTimes(samples, samples.Length), 10000000, watch);

            //System.Console.WriteLine($"GetWaveValue\tGetWaveValueWithAbs\tGetWaveValueUnrolled\tGetWaveValueUnrolledWithAbs\tGetWaveValueUnrolledFourTimes");
            //System.Console.WriteLine($"{method1ms} ticks\t{method2ms} ticks\t{method3ms} ticks\t{method4ms} ticks\t{method5ms} ticks");
        }

        private static long TestMethod(Func<WaveValue> execution, int executionCount, Stopwatch watch)
        {
            watch.Reset();

            long elapsedSum = 0;
            
            for (int i = 0; i < executionCount; i++)
            {
                watch.Reset();
                watch.Start();
                var result = execution();
                watch.Stop();
                
                elapsedSum += watch.ElapsedTicks;

                if (i == 0)
                    System.Console.WriteLine(result.UnderlyingValue);
            }

            return elapsedSum / executionCount;
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            eDrums.Dispose();
        }
    }
}