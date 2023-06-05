using PieroDeTomi.EDrums.Models;

namespace PieroDeTomi.EDrums.Utilities
{
    public class WaveformAnalyzer
    {
        private List<WaveValue> _samplesQueue = new();

        private bool _isInsideWave = false;

        private Action<WaveValue> _onPeakDetected;

        public WaveformAnalyzer(Action<WaveValue> onPeakDetected)
        {
            _onPeakDetected = onPeakDetected;
        }

        public void AddSampleValue(WaveValue waveValue)
        {
            if (!_isInsideWave)
            {
                if (waveValue.IsAudible)
                {
                    _samplesQueue.Add(waveValue);
                    _isInsideWave = true;
                }
            }
            else
            {
                if (waveValue.IsAudible)
                {
                    _samplesQueue.Add(waveValue);
                }
                else
                {
                    // Calculate wave peak
                    var max = _samplesQueue[0];

                    for (var i = 1; i < _samplesQueue.Count; i++)
                    {
                        var sampleValue = _samplesQueue[i];
                        
                        if (sampleValue.NormalizedValue > max.NormalizedValue)
                            max = sampleValue;
                    }

                    // Emit peak
                    _onPeakDetected(max);

                    // Clear state
                    _samplesQueue.Clear();
                    _isInsideWave = false;
                }
            }
        }
    }
}