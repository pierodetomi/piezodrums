using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PieroDeTomi.EDrums.Models.Configuration;
using System.Diagnostics;

namespace PieroDeTomi.EDrums.Console
{
    public class Program
    {
        private static EDrums _eDrums;

        [STAThread]
        public static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            using IHost host = Host.CreateDefaultBuilder(args).Build();
            var config = host.Services.GetService<IConfiguration>();

            var drumModuleConfiguration = config.GetRequiredSection("DrumModule").Get<DrumModuleConfiguration>();
            _eDrums = new EDrums(drumModuleConfiguration);

            host.Run();
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            _eDrums?.Dispose();
        }
    }
}