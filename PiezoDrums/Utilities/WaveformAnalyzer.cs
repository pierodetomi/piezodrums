using PiezoDrums.Base;
using PiezoDrums.Models;
using System.Diagnostics;

namespace PiezoDrums.Utilities
{
    public class WaveformAnalyzer : LoggingComponentBase
    {
        private bool _isInsideWave = false;

        private AudioSamplePeakValue _currentMaxPeak;

        private Action<AudioSamplePeakValue> _onWaveMaxPeakDetected;

#if DEBUG
        private Stopwatch _watch = new Stopwatch();

        private decimal _detectedPeaks = 0;

        private decimal _cumulativeElapsedTicks = 0;
#endif

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
#if DEBUG
                    _watch.Start();
#endif

                    _currentMaxPeak = samplePeak;
                    _isInsideWave = true;
                }
            }
            else
            {
                if (samplePeak.IsAudible && samplePeak.UnderlyingValue > _currentMaxPeak.UnderlyingValue)
                {
                    _currentMaxPeak = samplePeak;
                }
                else
                {
                    /* Emit wave MAX peak value.
                     * 
                     * For performance purposes, this callback will be executed async, to avoid blocking/slowing 
                     * the processing of future incoming samples.
                     * 
                     */
                    Task.Run(() => _onWaveMaxPeakDetected(_currentMaxPeak));

                    // Clear state
                    _isInsideWave = false;

#if DEBUG
                    _watch.Stop();
                    _detectedPeaks++;
                    _cumulativeElapsedTicks += _watch.ElapsedTicks;
                    var _meanElapsedTicks = _cumulativeElapsedTicks / _detectedPeaks;
                    Log($"Mean peak detection time: {_meanElapsedTicks} ticks", ConsoleColor.Green, clearPreviousContent: true);
                    _watch.Reset();
#endif
                }
            }
        }
    }
}