using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Core.Utility;
using SerializerBenchmark.Server.Interfaces;

namespace SerializerBenchmark.Server.Scenarios;

public class RepeatedScenarioStrategy(IEnumerable<ISerializerService> serializerServices) : BaseScenarioStrategy(serializerServices)
{
    public override string Name => ScenarioName.Repeated;

    public override Task<BenchmarkResult> RunAsync(string serializerName, int count, int iterations)
    {
        var data = DataGenerator.GenerateLogs(count);
        return BenchmarkGeneric(serializerName, data, iterations);
    }
}
