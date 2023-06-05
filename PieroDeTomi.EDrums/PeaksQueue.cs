using PieroDeTomi.EDrums.Models;

namespace PieroDeTomi.EDrums
{
    public class PeaksQueue
    {
        private WaveValue? _previousPeak;

        private bool _isWaveGrowing;

        private DateTime _lastAdd;

        private Action<WaveValue> _truePeakDetected;

        private Task _emitter;

        private CancellationTokenSource _emitterCancellationTokenSource;

        public PeaksQueue(Action<WaveValue> truePeakDetected)
        {
            _truePeakDetected = truePeakDetected;
        }

        public void Add(WaveValue peak)
        {
            var elapsed = (DateTime.Now - _lastAdd).Ticks / 10000;

            _lastAdd = DateTime.Now;

            if (elapsed > 10 || !_previousPeak.HasValue)
            {
                _previousPeak = peak;
                Emit(_previousPeak.Value);

                return;
            }

            System.Console.WriteLine($"Delay: {elapsed}ms");

            if (peak.UnderlyingValue >= _previousPeak.Value.UnderlyingValue)
            {
                _previousPeak = peak;
                _isWaveGrowing = true;

                return;
            }

            if (peak.UnderlyingValue < _previousPeak.Value.UnderlyingValue && _isWaveGrowing)
            {
                Emit(_previousPeak.Value);
                
                _previousPeak = null;
                _isWaveGrowing = false;

                return;
            }
        }

        public void Emit(WaveValue peak)
        {
            _emitterCancellationTokenSource?.Cancel();
            _emitterCancellationTokenSource?.Dispose();
            _emitterCancellationTokenSource = new CancellationTokenSource();

            try
            {
                _emitter = Task
                    .Delay(10, _emitterCancellationTokenSource.Token)
                    .ContinueWith(_ => _truePeakDetected(peak), _emitterCancellationTokenSource.Token);
            }
            catch (TaskCanceledException exception) when (exception.CancellationToken == _emitterCancellationTokenSource.Token)
            {
            }
        }
    }
}
