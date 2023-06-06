using PiezoDrums.Models;

namespace PiezoDrums.Utilities
{
    public class WaveformAnalyzer
    {
        private readonly List<AudioSamplePeakValue> _samplePeaksQueue = new();

        private bool _isInsideWave = false;

        private Action<AudioSamplePeakValue> _onWaveMaxPeakDetected;

        public WaveformAnalyzer(Action<AudioSamplePeakValue> onWaveMaxPeakDetected)
        {
            _onWaveMaxPeakDetected = onWaveMaxPeakDetected;
        }

        public void AddSamplePeakValue(AudioSamplePeakValue samplePeak)
        {
            if (!_isInsideWave)
            {
                if (samplePeak.IsAudible)
                {
                    _samplePeaksQueue.Add(samplePeak);
                    _isInsideWave = true;
                }
            }
            else
            {
                if (samplePeak.IsAudible)
                {
                    _samplePeaksQueue.Add(samplePeak);
                }
                else
                {
                    /*
                     * Calculate wave peak
                     * 
                     * This is given by the greatest peak value between collected samples.
                     * 
                     */
                    var maxPeak = GetWaveMaxPeak();

                    /* Emit wave MAX peak value.
                     * 
                     * For performance purposes, this callback will be executed async, to avoid blocking/slowing 
                     * the processing of future incoming samples.
                     * 
                     */
                    Task.Run(() => _onWaveMaxPeakDetected(maxPeak));

                    // Clear state
                    _samplePeaksQueue.Clear();
                    _isInsideWave = false;
                }
            }
        }

        private AudioSamplePeakValue GetWaveMaxPeak()
        {
            var max = _samplePeaksQueue[0];

            if (_samplePeaksQueue.Count % 2 == 0)
            {
                for (var i = 1; i < _samplePeaksQueue.Count / 2; i += 2)
                {
                    var sampleValue = _samplePeaksQueue[i + 0];
                    var nextSampleValue = _samplePeaksQueue[i + 1];

                    if (sampleValue.NormalizedValue > max.NormalizedValue)
                        max = sampleValue;
                    if (nextSampleValue.NormalizedValue > max.NormalizedValue)
                        max = nextSampleValue;
                }
            }
            else
            {
                for (var i = 1; i < _samplePeaksQueue.Count; i++)
                {
                    var sampleValue = _samplePeaksQueue[i];

                    if (sampleValue.NormalizedValue > max.NormalizedValue)
                        max = sampleValue;
                }
            }

            return max;
        }
    }
}