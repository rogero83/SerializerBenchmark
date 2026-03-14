using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Server.Interfaces;

namespace SerializerBenchmark.Server.Services;

public class BenchmarkService(IEnumerable<IScenarioStrategy> scenarioStrategies)
{
    private readonly Dictionary<string, IScenarioStrategy> _scenarios
        = scenarioStrategies.ToDictionary(k => k.Name, v => v);

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
        if (!_scenarios.TryGetValue(scenario, out var strategy))
            throw new ArgumentException($"Unknown scenario: {scenario}");

        return strategy.RunAsync(serializer, count, iterations);
    }
}
