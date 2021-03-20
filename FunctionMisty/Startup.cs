using FunctionMisty;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

[assembly: FunctionsStartup(typeof(Startup))]

namespace FunctionMisty
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<MReadings>();
            builder.Services.AddLogging();
        }
    }
}
