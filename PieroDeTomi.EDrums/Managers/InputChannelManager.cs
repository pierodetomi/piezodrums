using NAudio.Wave;
using PieroDeTomi.EDrums.Models;
using PieroDeTomi.EDrums.Utilities;

namespace PieroDeTomi.EDrums.Managers
{
    public class InputChannelManager : ManagerBase
    {
        private readonly int _channelIndex;

        private readonly string _asioDriverName;

        private readonly AsioOut _asioOut;

        private readonly Action<int> _midiCallback;

        private readonly float _maxWaveImpulseValue;

        private float[] _samples;
        
        private int _lastVelocity;

        private DateTime _lastPlayedNote = DateTime.MinValue;

        private Debouncer _debouncer;

        private PeaksQueue _peaksQueue;

        private WaveformAnalyzer _waveformAnalyzer;

        public InputChannelManager(int channelIndex, int sampleRate, string asioDriverName, Action<int> midiCallback, float maxWaveImpulseValue)
        {
            _channelIndex = channelIndex;
            _asioDriverName = asioDriverName;
            _midiCallback = midiCallback;
            _maxWaveImpulseValue = maxWaveImpulseValue;

            _asioOut = new AsioOut(_asioDriverName) { InputChannelOffset = channelIndex };
            _asioOut.InitRecordAndPlayback(null, 1, sampleRate);
            _asioOut.AudioAvailable += OnAudioAvailable;

            _asioOut.Play(); // start recording

            _samples = new float[_asioOut.FramesPerBuffer];
            _lastVelocity = 0;
            _debouncer = new Debouncer(5);

            _peaksQueue = new PeaksQueue(peak =>
            {
                var velocity = peak.ToVelocity(_maxWaveImpulseValue);
                _midiCallback(velocity);
            });

            _waveformAnalyzer = new WaveformAnalyzer(peak =>
            {
                var velocity = peak.ToVelocity(_maxWaveImpulseValue);
                _midiCallback(velocity);
            });
        }

        public override void Dispose()
        {
            try { _asioOut.Dispose(); } catch { }
        }

        private void OnAudioAvailable(object sender, AsioAudioAvailableEventArgs e)
        {
            e.GetAsInterleavedSamples(_samples);

            var waveValue = GetWaveValue();
            // var velocity = waveValue.ToVelocity(_maxWaveImpulseValue);

            //if (waveValue.IsAudible && velocity > _lastVelocity)
            //{
            //    _peaksQueue.Add(waveValue);
            //    // _midiCallback(velocity);
            //    // System.Console.WriteLine($"Delta time: {delta}ms");
            //}

            _waveformAnalyzer.AddSampleValue(waveValue);

            // _lastVelocity = velocity;
        }

        private WaveValue GetWaveValue()
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

            return new WaveValue(max);
        }
    }
}
