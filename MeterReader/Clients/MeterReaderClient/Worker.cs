using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using MeterReaderWeb.Protos;
using MeterReaderWeb.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeterReaderClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration configuration;
        private readonly ReadingFactory readingFactory;
        private MeterReadingService.MeterReadingServiceClient client = null;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, ReadingFactory readingFactory)
        {
            _logger = logger;
            this.configuration = configuration;
            this.readingFactory = readingFactory;
        }

        protected MeterReadingService.MeterReadingServiceClient Client
        {
            get
            {
                if (client == null)
                {
                    var channel = GrpcChannel.ForAddress(configuration["Service:ServerUrl"]);
                    client = new MeterReadingService.MeterReadingServiceClient(channel);
                }

                return client;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var customerId = configuration.GetValue<int>("Service:CustomerId");

                var pkt = new ReadingPacket()
                {
                    Successful = ReadingStatus.Success,
                    Notes = "This is our test"
                };

                for (int i = 0; i < 5; ++i)
                {
                    pkt.Readings.Add(await readingFactory.Generate(customerId));
                }

                var result = await Client.AddReadingAsync(pkt);
                if (result.Success == ReadingStatus.Success)
                {
                    _logger.LogInformation("Successfully sent");
                }
                else
                {
                    _logger.LogInformation("Failed to sent");
                }

                await Task.Delay(configuration.GetValue<int>("Service:DelayInterval"), stoppingToken);
            }
        }
    }
}
