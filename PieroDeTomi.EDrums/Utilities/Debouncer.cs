namespace PieroDeTomi.EDrums.Utilities
{
    public class Debouncer
    {
        private int _milliseconds;

        private CancellationTokenSource _cancelTokenSource = null;

        private Func<Task> _lastMethod = null;

        public Debouncer(int milliseconds)
        {
            _milliseconds = milliseconds;
        }

        public Task Debounce(Func<Task> method)
        {
            _cancelTokenSource?.Cancel();
            _cancelTokenSource?.Dispose();
            _cancelTokenSource = new CancellationTokenSource();

            try
            {
                return Task.Delay(_milliseconds, _cancelTokenSource.Token)
                    .ContinueWith(_ => method(), _cancelTokenSource.Token);
            }
            catch (TaskCanceledException exception) when (exception.CancellationToken == _cancelTokenSource.Token)
            {
            }

            return Task.CompletedTask;
        }

        public Task DebounceIf(Func<bool> renewalCondition, Func<Task> method)
        {
            _cancelTokenSource?.Cancel();
            _cancelTokenSource?.Dispose();
            _cancelTokenSource = new CancellationTokenSource();

            if (renewalCondition())
                method = _lastMethod;

            try
            {
                _lastMethod = method;

                return Task.Delay(_milliseconds, _cancelTokenSource.Token)
                    .ContinueWith(_ => method(), _cancelTokenSource.Token);
            }
            catch (TaskCanceledException exception) when (exception.CancellationToken == _cancelTokenSource.Token)
            {
            }

            return Task.CompletedTask;
        }
    }
}
