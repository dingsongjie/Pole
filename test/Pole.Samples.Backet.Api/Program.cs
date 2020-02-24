using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Pole.Samples.Backet.Api.Benchmarks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Samples.Backet.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //GrainWithEntityframeworkCoreAndPgTest grainWithEntityframeworkCoreAndPgTest = new GrainWithEntityframeworkCoreAndPgTest();
            //await grainWithEntityframeworkCoreAndPgTest.SingleOrDefaultAsync();
            Summary summary = BenchmarkRunner.Run<GrainWithEntityframeworkCoreAndPgTest>();
            Console.ReadLine();
        }
    }
}
