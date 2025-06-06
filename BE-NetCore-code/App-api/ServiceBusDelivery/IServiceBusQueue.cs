using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusDelivery
{
    public interface IServiceBusQueue
    {
        //Send
        Task SendMesssage(string queueName, string message);

        // Receive
        Task <string> ReceiveMasssage(string queueName);
    }
}
