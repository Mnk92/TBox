using Mnk.Library.Common.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mnk.TBox.Plugins.Market.Service
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger<Program>();
        static void Main(string[] args)
        {
            LogManager.Init(new MultiplyLog(new FileLog("log.log"), new ConsoleLog()));
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddGrpc();
            builder.Services.AddDbContext<Context.Context>(x => x.UseSqlite(builder.Configuration.GetConnectionString("AppDbContext")));
            var app = builder.Build();
            app.MapGrpcService<MarketService>();
            app.Run();
        }
    }
}
