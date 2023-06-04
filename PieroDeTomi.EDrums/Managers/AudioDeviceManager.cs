using NAudio.Wave;

namespace PieroDeTomi.EDrums.Managers
{
    public class AudioDeviceManager : IDisposable
    {
        private readonly int _sampleRate;

        private readonly float _maxWaveImpulseValue;

        private readonly string _asioDriverName;

        private List<ChannelManager> _channelManagers = new();
        
        public AudioDeviceManager(string deviceSearchKey, int sampleRate, float maxImpulseValue)
        {
            _sampleRate = sampleRate;
            _maxWaveImpulseValue = maxImpulseValue;
            _asioDriverName = FindAsioDriverName(deviceSearchKey);
        }

        public void BindInputChannel(int inputChannel, Action<int> midiCallback)
        {
            var channelIndex = inputChannel - 1; // Channels are 0-indexed!
            _channelManagers.Add(new ChannelManager(channelIndex, _sampleRate, _asioDriverName, midiCallback, _maxWaveImpulseValue));
        }

        public void Dispose()
        {
            try { _channelManagers.ForEach(_ => _.Dispose()); } catch { }
            _channelManagers.Clear();
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
