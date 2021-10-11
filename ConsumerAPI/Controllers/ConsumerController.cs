using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsumerController : Controller
    {
        
        [HttpGet("consume/all")]
        public ActionResult<Message> GetAll()
        {
            Message msg = new Message();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    msg.messages = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", msg.messages);
                    Trace.WriteLine(" [x] Received {0}", msg.messages);
                };
                channel.BasicConsume(queue: "TaskQueue",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

            return msg;

        }
    }
}
