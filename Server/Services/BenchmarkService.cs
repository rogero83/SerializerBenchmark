using SerializerBenchmark.Core.Interfaces;
using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Core.Services;

namespace SerializerBenchmark.Server.Services;

public class BenchmarkService(IEnumerable<ISerializerService> serializerServices)
{
    private readonly Dictionary<string, ISerializerService> serializers = serializerServices.ToDictionary(k => k.Name, v => v);

    public async Task<List<BenchmarkResult>> RunAsync(BenchmarkRequest request)
    {
        var results = new List<BenchmarkResult>();

        foreach (var serializer in request.Serializers)
        {
            try
            {
                var result = await RunScenario(serializer, request.Scenario, request.ItemCount, request.Iterations);
                results.Add(result);
            }
            catch (Exception ex)
            {
                results.Add(new BenchmarkResult
                {
                    Serializer = serializer,
                    Scenario = request.Scenario,
                    Error = ex.Message,
                });
            }
        }

        return results;
    }

    private Task<BenchmarkResult> RunScenario(string serializer, string scenario, int count, int iterations)
    {
        return scenario switch
        {
            "integers" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateIntegers(count), iterations),
            "decimals" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateDecimal(count), iterations),
            "floats" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateFloats(count), iterations),
            "strings" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateStrings(count), iterations),
            "nested" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateNested(count), iterations),
            "datetime" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateTemporal(count), iterations),
            "repeated" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateLogs(count), iterations),
            "bulkarray" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateBulkArray(count), iterations),
            _ => throw new ArgumentException($"Unknown scenario: {scenario}"),
        };
    }

    private Task<BenchmarkResult> BenchmarkGeneric<T>(
        string serializer, string scenario, T data, int iterations)
    {
        if (!serializers.TryGetValue(serializer, out var service))
            throw new ArgumentException($"Unknown serializer: {serializer}");

        var measureResult = service.Measure(data, iterations);

        return Task.FromResult(new BenchmarkResult
        {
            Serializer = serializer,
            Scenario = scenario,
            SizeBytes = measureResult.Size,
            SerializeMs = Math.Round(measureResult.SerMs, 4),
            DeserializeMs = Math.Round(measureResult.DeserMs, 4),
            Iterations = iterations,
        });
    }
}
