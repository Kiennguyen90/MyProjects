namespace CryptoInvestmentAPI.CustomExtensions
{
    public class ColorConsoleLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ColorConsoleLogger(categoryName);
        }

        public void Dispose()
        {
            // Cleanup nếu cần
        }
    }
}
