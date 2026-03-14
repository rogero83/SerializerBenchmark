using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Core.Utility;
using SerializerBenchmark.Server.Interfaces;

namespace SerializerBenchmark.Server.Scenarios;

public class DateTimeScenarioStrategy(IEnumerable<ISerializerService> serializerServices) : BaseScenarioStrategy(serializerServices)
{
    public override string Name => ScenarioName.DateTime;

    public override Task<BenchmarkResult> RunAsync(string serializerName, int count, int iterations)
    {
        var data = DataGenerator.GenerateTemporal(count);
        return BenchmarkGeneric(serializerName, data, iterations);
    }
}
