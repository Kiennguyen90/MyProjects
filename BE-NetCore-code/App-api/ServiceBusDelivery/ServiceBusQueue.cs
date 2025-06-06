
using Azure.Messaging.ServiceBus;
using System.Text;

namespace ServiceBusDelivery
{
    public class ServiceBusQueue : IServiceBusQueue
    {
        private string _conn;
        
        public ServiceBusQueue(string conn)
        {
            _conn = conn;
        }
        public async Task<string> ReceiveMasssage(string queueName)
        {
            try
            {
                var sbClient = new ServiceBusClient(_conn, new ServiceBusClientOptions()
                {
                    TransportType = ServiceBusTransportType.AmqpWebSockets,
                });

                var sbReceiver = sbClient.CreateReceiver(queueName);
                var msg = await sbReceiver.ReceiveMessageAsync();
                var messageBody = Encoding.UTF8.GetString(msg.Body);
                return messageBody;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task SendMesssage(string queueName, string message)
        {
            try
            {
                var sbClient = new ServiceBusClient(_conn, new ServiceBusClientOptions()
                {
                    TransportType = ServiceBusTransportType.AmqpWebSockets
                });

                var sbSender = sbClient.CreateSender(queueName);
                await sbSender.SendMessageAsync(new ServiceBusMessage(message));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
