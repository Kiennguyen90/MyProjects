using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceBusDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCore.BackgroundServices
{
    public class AzureServiceBusListener : BackgroundService
    {
        private readonly ILogger<AzureServiceBusListener> _logger;
        private readonly IServiceBusQueue _queue;

        public AzureServiceBusListener (ILogger<AzureServiceBusListener> logger, IServiceBusQueue queue)
        {
            _logger = logger;
            _queue = queue;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Listener Started");
            while (!stoppingToken.IsCancellationRequested) { 
                var msg = await _queue.ReceiveMasssage("cryptoservice");
                Console.WriteLine(msg);
            }
        }
    }
}
