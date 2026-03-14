using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Server.Interfaces;

namespace SerializerBenchmark.Server.Scenarios;

public abstract class BaseScenarioStrategy(IEnumerable<ISerializerService> serializerServices)
    : IScenarioStrategy
{
    private readonly Dictionary<string, ISerializerService> _serializers = serializerServices.ToDictionary(k => k.Name, v => v);

    public abstract string Name { get; }

    public abstract Task<BenchmarkResult> RunAsync(string serializerName, int count, int iterations);

    protected Task<BenchmarkResult> BenchmarkGeneric<T>(string serializerName, T data, int iterations)
    {
        if (!_serializers.TryGetValue(serializerName, out var service))
            throw new ArgumentException($"Unknown serializer: {serializerName}");

        var measureResult = service.Measure(data, iterations);

        return Task.FromResult(new BenchmarkResult
        {
            Serializer = serializerName,
            Scenario = Name,
            SizeBytes = measureResult.Size,
            SerializeMs = Math.Round(measureResult.SerMs, 4),
            DeserializeMs = Math.Round(measureResult.DeserMs, 4),
            Iterations = iterations,
        });
    }
}
