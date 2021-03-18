using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MistyDemo.Classes
{
    public class MHubService : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                await this.Setup();
            }
        }

        private async Task Setup()
        {
            Console.WriteLine("Hub Ready...");
        }

    }
}
