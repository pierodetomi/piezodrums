namespace PiezoDrums.Base
{
    public class LoggingComponentBase
    {
        public void LogError(string message, bool clearPreviousContent = false)
        {
            Log($"ERR: {message}", ConsoleColor.Red, clearPreviousContent);
        }

        public void Log(string message, ConsoleColor? color = null, bool clearPreviousContent = false)
        {
            if (clearPreviousContent)
                System.Console.Clear();

            if (!color.HasValue)
            {
                System.Console.WriteLine(message);
                return;
            }

            var colorBackup = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color.Value;
            System.Console.WriteLine(message);
            System.Console.ForegroundColor = colorBackup;
        }
    }
}