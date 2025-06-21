using Microsoft.Extensions.Logging;

namespace CryptoInvestmentAPI.CustomExtensions
{
    public class ColorConsoleLogger : ILogger
    {
        private readonly string _name;

        public ColorConsoleLogger(string name)
        {
            _name = name;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            // Bật tắt theo mức log cần thiết
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            // Đặt màu theo LogLevel
            switch (logLevel)
            {
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }

            Console.WriteLine($"{DateTime.Now} [{logLevel}] {_name}: {formatter(state, exception)}");

            Console.ResetColor();
        }
    }
}
