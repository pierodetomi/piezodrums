using NAudio.Wave;
using PiezoDrums.Base;
using PiezoDrums.Models.Configuration;

namespace PiezoDrums.Managers
{
    public class AudioDeviceManager : LoggingComponentBase, IDisposable
    {
        private readonly DrumModuleConfiguration _configuration;

        private readonly string _asioDriverName;

        private List<InputChannelManager> _inputChannelManagers = new();
        
        public AudioDeviceManager(DrumModuleConfiguration configuration)
        {
            _configuration = configuration;
            _asioDriverName = FindAsioDriverName(configuration.AudioDeviceSearchKey);

            if (string.IsNullOrEmpty(_asioDriverName))
            {
                LogError($"Unable to find input audio device (looking for key \"{configuration.AudioDeviceSearchKey}\")", true);
                Environment.Exit(-1);
            }
        }

        public void BindInputChannel(int inputChannel, Action<int> midiCallback)
        {
            var channelIndex = inputChannel - 1; // Channels are 0-indexed!
            _inputChannelManagers.Add(new InputChannelManager(channelIndex, _asioDriverName, midiCallback, _configuration));
        }

        public void Dispose()
        {
            _inputChannelManagers.ForEach(_ =>
            {
                try { _.Dispose(); } catch { }
            });

            _inputChannelManagers.Clear();
        }

        private string FindAsioDriverName(string searchKeyword)
        {
            string asioDriverName = null;

            var asioOutDrivers = AsioOut.GetDriverNames();

            foreach (var driverName in asioOutDrivers)
            {
                if (driverName.Contains(searchKeyword))
                {
                    asioDriverName = driverName;
                    break;
                }
            }

            return asioDriverName;
        }
    }
}
