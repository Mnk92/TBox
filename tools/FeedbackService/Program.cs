using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mnk.Library.Common.Log;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Tools.FeedbackService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Fix  to load dependencies correctly
            AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
            {
                return (from dir in new[] { "Libraries", "Localization" }
                        select Path.GetFullPath(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\" + dir + "\\", new AssemblyName(a.Name).Name + ".dll"))
                    into assemblyPath
                        where File.Exists(assemblyPath)
                        select Assembly.LoadFrom(assemblyPath)).FirstOrDefault();
            };

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
