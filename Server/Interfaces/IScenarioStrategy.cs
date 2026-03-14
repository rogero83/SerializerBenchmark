using SerializerBenchmark.Core.Models;

namespace SerializerBenchmark.Server.Interfaces;

public interface IScenarioStrategy
{
    string Name { get; }
    Task<BenchmarkResult> RunAsync(string serializerName, int count, int iterations);
}
