using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NreKnowledgebase;
using Serilog;

namespace NreKnowledgebaseDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args)
                .Build();
            
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
  
            var user = configuration.GetValue<string>("user");
            var password = configuration.GetValue<string>("password");

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                Log.Fatal("User or password not provided in commandline");
                Log.Information("Usage:  NreKnowledgebaseDownloader user=<user> password=<password>");
                return;
            }
            
            using(var source = new NationalRailEnquiriesSource(new HttpClient(), Log.Logger))
            {
                await source.Initiate(user, password, CancellationToken.None);
                Log.Information("Initialised");
                
                var outputFolder = CreateOutputFolder();
                Log.Information("Created directory");
                var tasks = new List<Task>();
                var token = CancellationToken.None;

                foreach (var subject in (KnowedgebaseSubjects[]) Enum.GetValues(typeof(KnowedgebaseSubjects)))
                {
                    var outputTask = CreateOutputTask(subject, source, outputFolder, token);
                    tasks.Add(outputTask);
                }
                Log.Information("Waiting");
                if(!Task.WaitAll(tasks.ToArray(), 30000))
                    Log.Error("Did not get all of the knowledgebase before timeout.");
                
                Log.Information("Done");
            }
        }

        private static string CreateOutputFolder()
        {
            var folderName = Path.Combine(".", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(folderName);
            return folderName;
        }
        
        
        private static Task CreateOutputTask(KnowedgebaseSubjects subject, NationalRailEnquiriesSource source, string outputFolder, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var subjectName = Enum.GetName(typeof(KnowedgebaseSubjects), subject);
                try
                {
                    using (var reader = new StreamReader(await source.GetKnowledgebaseStream(subject, token)))
                    {
                        var fileName = Path.Combine(outputFolder, $"{subjectName}.xml");
                        var xml = reader.ReadToEnd();
                        await File.WriteAllTextAsync(fileName, xml, token);
                        Log.Information($"Downloaded {subjectName}", subjectName);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e,$"Error retrieving {subjectName}", subjectName);
                    throw;
                }
            }, token);
        }
    }
}