using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Core.Utility;
using SerializerBenchmark.Server.Interfaces;
using SerializerBenchmark.Server.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace SerializerBenchmark.Server.Services;

public class TextJsonService : ISerializerService
{
    public string Name => SerializerName.SystemTextJson;

    public MeasureResult Measure<T>(T data, int iterations)
    {
        var typeInfo = MyJsonSerializerContext.Default.GetTypeInfo(typeof(T)) as JsonTypeInfo<T>
            ?? throw new InvalidOperationException(
                $"Tipo {typeof(T).Name} non registrato in MyJsonSerializerContext. " +
                $"Aggiungi [JsonSerializable(typeof({typeof(T).Name}))] al context.");

        // Warmup
        var warmup = JsonSerializer.Serialize(data, typeInfo);
        JsonSerializer.Deserialize(warmup, typeInfo);

        var sw = Stopwatch.StartNew();
        string? str = null;
        for (int i = 0; i < iterations; i++)
            str = JsonSerializer.Serialize(data, typeInfo);
        sw.Stop();
        double serMs = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        for (int i = 0; i < iterations; i++)
            JsonSerializer.Deserialize(str!, typeInfo);
        sw.Stop();

        return new MeasureResult(str!.Length, serMs, sw.Elapsed.TotalMilliseconds);
    }
}
