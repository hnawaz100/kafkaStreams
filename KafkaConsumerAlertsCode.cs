using System;
using System.Net.Mail;
using System.Threading;
using Confluent.Kafka;
using Newtonsoft.Json;
using Slack.Webhooks;

namespace largetransactionalert
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                GroupId = "1003",
                BootstrapServers = "ipadress:9092",
                AutoOffsetReset = AutoOffsetReset.Latest
            };


            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("YourKafkaTopic"); //

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            var obj = JsonConvert.DeserializeObject<sales>(cr.Value);

                            Console.WriteLine("Order ID: " + obj.OD_ORDERID + "\t country" + obj.SHIPCOUNTRY + "\t Sales Amount" + obj.SALESAMOUNT);
                            var message = "The following Order ID " + obj.OD_ORDERID + " with;" + " Location: " + obj.SHIPCOUNTRY + " has a sales amount of " + obj.SALESAMOUNT + ". Please review and verify.";

                            var url = ""; //slack webhook url

                            var slackClient = new SlackClient(url);

                            var slackMessage = new SlackMessage
                            {
                                Channel = "#slackChannel",
                                Text = message,
                                IconEmoji = Emoji.Collision,
                                Username = "Kafka Alerts"
                            };
                            slackClient.Post(slackMessage);
                            sendMail(message);
                            Console.WriteLine(obj.ToString());
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }

        }

        //class to append sales data
        class sales
        {
            public string SHIPCOUNTRY { get; set; }
            public string ORDERDATE { get; set; }
            public string OD_ORDERID { get; set; }
            public string SALESAMOUNT { get; set; }
        }

        //send mail alerts
        private static void sendMail(string message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("127.0.0.1"); //smtp server 

                mail.From = new MailAddress("youremailaddres");
                mail.To.Add("youremailaddres");
                mail.Subject = "Kafka Alerts";
                mail.Body = message;

                SmtpServer.Port = 25; //mail server port
                SmtpServer.Credentials = new System.Net.NetworkCredential("user", "password");
                //SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                Console.WriteLine("mail Sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
