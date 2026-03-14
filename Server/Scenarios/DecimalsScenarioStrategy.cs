using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Core.Utility;
using SerializerBenchmark.Server.Interfaces;

namespace SerializerBenchmark.Server.Scenarios;

public class DecimalsScenarioStrategy(IEnumerable<ISerializerService> serializerServices) : BaseScenarioStrategy(serializerServices)
{
    public override string Name => ScenarioName.Decimals;

    public override Task<BenchmarkResult> RunAsync(string serializerName, int count, int iterations)
    {
        var data = DataGenerator.GenerateDecimal(count);
        return BenchmarkGeneric(serializerName, data, iterations);
    }
}
