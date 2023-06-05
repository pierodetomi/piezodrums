using PiezoDrums.Models;

namespace PiezoDrums.Utilities
{
    public class WaveformAnalyzer
    {
        private readonly List<AudioSamplePeakValue> _samplePeaksQueue = new();

        private bool _isInsideWave = false;

        private Action<AudioSamplePeakValue> _onWavePeakDetected;

        public WaveformAnalyzer(Action<AudioSamplePeakValue> onWavePeakDetected)
        {
            _onWavePeakDetected = onWavePeakDetected;
        }

        public void AddSamplePeakValue(AudioSamplePeakValue waveValue)
        {
            if (!_isInsideWave)
            {
                if (waveValue.IsAudible)
                {
                    _samplePeaksQueue.Add(waveValue);
                    _isInsideWave = true;
                }
            }
            else
            {
                if (waveValue.IsAudible)
                {
                    _samplePeaksQueue.Add(waveValue);
                }
                else
                {
                    // Calculate wave peak
                    var max = _samplePeaksQueue[0];

                    for (var i = 1; i < _samplePeaksQueue.Count; i++)
                    {
                        var sampleValue = _samplePeaksQueue[i];
                        
                        if (sampleValue.NormalizedValue > max.NormalizedValue)
                            max = sampleValue;
                    }

                    // Emit wave peak value
                    _onWavePeakDetected(max);

                    // Clear state
                    _samplePeaksQueue.Clear();
                    _isInsideWave = false;
                }
            }
        }
    }
}