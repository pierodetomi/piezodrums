using NAudio.Wave;
using PiezoDrums.Models;
using PiezoDrums.Models.Configuration;
using PiezoDrums.Utilities;

namespace PiezoDrums.Managers
{
    public class InputChannelManager : ManagerBase
    {
        private readonly int _channelIndex;

        private readonly string _asioDriverName;

        private readonly AsioOut _asioOut;

        private readonly Action<int> _midiCallback;

        private readonly DrumModuleConfiguration _configuration;

        private float[] _samples;
        
        private WaveformAnalyzer _waveformAnalyzer;

        public InputChannelManager(int channelIndex, string asioDriverName, Action<int> midiCallback, DrumModuleConfiguration configuration)
        {
            _channelIndex = channelIndex;
            _asioDriverName = asioDriverName;
            _midiCallback = midiCallback;
            _configuration = configuration;

            _asioOut = new AsioOut(_asioDriverName) { InputChannelOffset = channelIndex };
            _asioOut.InitRecordAndPlayback(null, 1, _configuration.SampleRate);
            _asioOut.AudioAvailable += OnAudioAvailable;

            _asioOut.Play(); // start recording

            _samples = new float[_asioOut.FramesPerBuffer];

            _waveformAnalyzer = new WaveformAnalyzer(peak =>
            {
                var velocity = peak.ToVelocity(_configuration.MaxWaveImpulseValue);
                _midiCallback(velocity);
            });
        }

        public override void Dispose()
        {
            try { _asioOut?.Dispose(); } catch { }
        }

        private void OnAudioAvailable(object sender, AsioAudioAvailableEventArgs e)
        {
            e.GetAsInterleavedSamples(_samples);

            var samplePeakValue = GetSamplePeakValue();
            _waveformAnalyzer.AddSamplePeakValue(samplePeakValue);
        }

        private AudioSamplePeakValue GetSamplePeakValue()
        {
            var max = 0f;

            for (var i = 0; i < _samples.Length; i += 4)
            {
                // Unroll the array 4 times (for performance purposes)
                var sample1 = _samples[i + 0];
                var sample2 = _samples[i + 1];
                var sample3 = _samples[i + 2];
                var sample4 = _samples[i + 3];

                // absolute value 
                if (sample1 < 0) sample1 = -sample1;
                if (sample2 < 0) sample2 = -sample2;
                if (sample3 < 0) sample3 = -sample3;
                if (sample4 < 0) sample4 = -sample4;

                // is this the max value?
                if (sample1 > max) max = sample1;
                if (sample2 > max) max = sample2;
                if (sample3 > max) max = sample3;
                if (sample4 > max) max = sample4;
            }

            return new AudioSamplePeakValue(max);
        }
    }
}
