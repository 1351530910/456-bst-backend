﻿
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace bst
{
    public class Program
    {
        public const bool DEBUG = true;
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
