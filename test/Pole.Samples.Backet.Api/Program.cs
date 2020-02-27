using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Npgsql;
using Pole.Core.EventBus.EventStorage;
using Pole.Samples.Backet.Api.Benchmarks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Samples.Backet.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //GrainWithEntityframeworkCoreAndPgTest grainWithEntityframeworkCoreAndPgTest = new GrainWithEntityframeworkCoreAndPgTest();
            //await grainWithEntityframeworkCoreAndPgTest.SingleOrDefaultAsync();
            //Summary summary = BenchmarkRunner.Run<GrainWithEntityframeworkCoreAndPgTest>();
            //Console.ReadLine();
            //using ( var connection = new NpgsqlConnection("Server=192.168.0.248;Port=5432;Username=postgres;Password=comteck2020!@#;Database=Pole-Backet;Enlist=True;Timeout=0;Command Timeout=600;MinPoolSize=20;MaxPoolSize=500;"))
            //{
            //    var uploader = new Pole.EventStorage.PostgreSql.PoleNpgsqlBulkUploader(connection);
            //    var events = new List<EventEntity>();
            //    events.Add(new EventEntity { Id = "111", Retries = 20, ExpiresAt = DateTime.Now, StatusName = "333" });
            //    await uploader.UpdateAsync("\"pole\".\"Events\"", events);
            //}
            // Queue the task.
            for (var i = 0; i < 100; i++)
            {
                ThreadPool.QueueUserWorkItem(ThreadProc, i);
            }

            Console.WriteLine("Main thread does some work, then sleeps.");
            Thread.Sleep(1000);

            Console.WriteLine("Main thread exits.");
        }

        // This thread procedure performs the task.
        static void ThreadProc(Object stateInfo)
        {
            var i = Convert.ToInt32(stateInfo);
            // No state object was passed to QueueUserWorkItem, so stateInfo is null.
            Console.WriteLine($"Hello from the thread pool.{i}");
        }
    }
}
