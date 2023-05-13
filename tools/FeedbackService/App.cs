using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Mnk.Library.Common.Log;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Tools.FeedbackService
{
    internal class App
    {
        public static void Run(string[] args)
        {
            LogManager.Init(new MultiplyLog(new ConsoleLog(), new FileLog(Path.Combine(Folders.UserLogsFolder, "TBox.Feedback.Service.log"))));
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();
            //app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
