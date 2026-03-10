using MemoryPack;
using MessagePack;
using ProtoBuf;
using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Core.Services;
using System.Diagnostics;
using System.Text.Json;

namespace SerializerBenchmark.Server.Services;

public class BenchmarkService
{
    private static readonly MessagePackSerializerOptions MsgPackOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.None);

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
            "integers" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateIntegers(count), iterations, RunIntegerBenchmark),
            "decimals" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateDecimal(count), iterations, RunDecimalBenchmark),
            "floats" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateFloats(count), iterations, RunFloatBenchmark),
            "strings" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateStrings(count), iterations, RunStringBenchmark),
            "nested" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateNested(count), iterations, RunNestedBenchmark),
            "datetime" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateTemporal(count), iterations, RunTemporalBenchmark),
            "repeated" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateLogs(count), iterations, RunLogBenchmark),
            "bulkarray" => BenchmarkGeneric(serializer, scenario, DataGenerator.GenerateBulkArray(count), iterations, RunBulkArrayBenchmark),
            _ => throw new ArgumentException($"Unknown scenario: {scenario}"),
        };
    }

    private Task<BenchmarkResult> BenchmarkGeneric<T>(
        string serializer, string scenario, T data, int iterations,
        Func<string, T, int, (long size, double serMs, double deserMs)> runner)
    {
        var (size, serMs, deserMs) = runner(serializer, data, iterations);
        return Task.FromResult(new BenchmarkResult
        {
            Serializer = serializer,
            Scenario = scenario,
            SizeBytes = size,
            SerializeMs = Math.Round(serMs, 4),
            DeserializeMs = Math.Round(deserMs, 4),
            Iterations = iterations,
        });
    }

    // ─────────────────────────────────────────
    // INTEGERS
    // ─────────────────────────────────────────
    private (long, double, double) RunIntegerBenchmark(string serializer, IntegerPayload data, int iterations)
        => serializer switch
        {
            SerializerName.SystemTextJson => MeasureJson(data, iterations),
            SerializerName.Protobuf => MeasureProtobuf(data, iterations),
            SerializerName.MessagePack => MeasureMsgPack(data, iterations),
            SerializerName.MemoryPack => MeasureMemoryPack(data, iterations),
            _ => throw new ArgumentException(serializer),
        };

    // ─────────────────────────────────────────
    // INTEGERS
    // ─────────────────────────────────────────
    private (long, double, double) RunDecimalBenchmark(string serializer, DecimalPayload data, int iterations)
        => serializer switch
        {
            SerializerName.SystemTextJson => MeasureJson(data, iterations),
            SerializerName.Protobuf => MeasureProtobuf(data, iterations),
            SerializerName.MessagePack => MeasureMsgPack(data, iterations),
            SerializerName.MemoryPack => MeasureMemoryPack(data, iterations),
            _ => throw new ArgumentException(serializer),
        };

    // ─────────────────────────────────────────
    // FLOATS
    // ─────────────────────────────────────────
    private (long, double, double) RunFloatBenchmark(string serializer, FloatPayload data, int iterations)
        => serializer switch
        {
            SerializerName.SystemTextJson => MeasureJson(data, iterations),
            SerializerName.Protobuf => MeasureProtobuf(data, iterations),
            SerializerName.MessagePack => MeasureMsgPack(data, iterations),
            SerializerName.MemoryPack => MeasureMemoryPack(data, iterations),
            _ => throw new ArgumentException(serializer),
        };

    // ─────────────────────────────────────────
    // STRINGS
    // ─────────────────────────────────────────
    private (long, double, double) RunStringBenchmark(string serializer, StringPayload data, int iterations)
        => serializer switch
        {
            SerializerName.SystemTextJson => MeasureJson(data, iterations),
            SerializerName.Protobuf => MeasureProtobuf(data, iterations),
            SerializerName.MessagePack => MeasureMsgPack(data, iterations),
            SerializerName.MemoryPack => MeasureMemoryPack(data, iterations),
            _ => throw new ArgumentException(serializer),
        };

    // ─────────────────────────────────────────
    // NESTED
    // ─────────────────────────────────────────
    private (long, double, double) RunNestedBenchmark(string serializer, NestedPayload data, int iterations)
        => serializer switch
        {
            SerializerName.SystemTextJson => MeasureJson(data, iterations),
            SerializerName.Protobuf => MeasureProtobuf(data, iterations),
            SerializerName.MessagePack => MeasureMsgPack(data, iterations),
            SerializerName.MemoryPack => MeasureMemoryPack(data, iterations),
            _ => throw new ArgumentException(serializer),
        };

    // ─────────────────────────────────────────
    // TEMPORAL
    // ─────────────────────────────────────────
    private (long, double, double) RunTemporalBenchmark(string serializer, TemporalPayload data, int iterations)
        => serializer switch
        {
            SerializerName.SystemTextJson => MeasureJson(data, iterations),
            SerializerName.Protobuf => MeasureProtobuf(data, iterations),
            SerializerName.MessagePack => MeasureMsgPack(data, iterations),
            SerializerName.MemoryPack => MeasureMemoryPack(data, iterations),
            _ => throw new ArgumentException(serializer),
        };

    // ─────────────────────────────────────────
    // LOGS (repeated)
    // ─────────────────────────────────────────
    private (long, double, double) RunLogBenchmark(string serializer, LogPayload data, int iterations)
        => serializer switch
        {
            SerializerName.SystemTextJson => MeasureJson(data, iterations),
            SerializerName.Protobuf => MeasureProtobuf(data, iterations),
            SerializerName.MessagePack => MeasureMsgPack(data, iterations),
            SerializerName.MemoryPack => MeasureMemoryPack(data, iterations),
            _ => throw new ArgumentException(serializer),
        };

    // ─────────────────────────────────────────
    // BULK ARRAY
    // ─────────────────────────────────────────
    private (long, double, double) RunBulkArrayBenchmark(string serializer, BulkArrayPayload data, int iterations)
        => serializer switch
        {
            SerializerName.SystemTextJson => MeasureJson(data, iterations),
            SerializerName.Protobuf => MeasureProtobuf(data, iterations),
            SerializerName.MessagePack => MeasureMsgPack(data, iterations),
            SerializerName.MemoryPack => MeasureMemoryPack(data, iterations),
            _ => throw new ArgumentException(serializer),
        };

    // ═════════════════════════════════════════
    // MEASUREMENT HELPERS
    // ═════════════════════════════════════════

    private static (long size, double serMs, double deserMs) MeasureJson<T>(T data, int iterations)
    {
        var typeInfo = MyJsonSerializerContext.Default.GetTypeInfo(typeof(T));

        // Warmup
        var warmup = JsonSerializer.Serialize(data, typeInfo!);
        JsonSerializer.Deserialize<T>(warmup);

        var sw = Stopwatch.StartNew();
        string? str = null;
        for (int i = 0; i < iterations; i++)
            str = JsonSerializer.Serialize(data, typeInfo!);
        sw.Stop();
        double serMs = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        for (int i = 0; i < iterations; i++)
            JsonSerializer.Deserialize(str!, typeInfo!);
        sw.Stop();

        return (str!.Length, serMs, sw.Elapsed.TotalMilliseconds);
    }

    private static (long size, double serMs, double deserMs) MeasureProtobuf<T>(T data, int iterations)
    {
        // Warmup
        using var warmupMs = new MemoryStream();
        Serializer.Serialize(warmupMs, data);
        warmupMs.Position = 0;
        Serializer.Deserialize<T>(warmupMs);

        var sw = Stopwatch.StartNew();
        byte[]? bytes = null;
        for (int i = 0; i < iterations; i++)
        {
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, data);
            bytes = ms.ToArray();
        }
        sw.Stop();
        double serMs = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            using var ms = new MemoryStream(bytes!);
            Serializer.Deserialize<T>(ms);
        }
        sw.Stop();

        return (bytes!.Length, serMs, sw.Elapsed.TotalMilliseconds);
    }

    private static (long size, double serMs, double deserMs) MeasureMsgPack<T>(T data, int iterations)
    {
        // Warmup
        var warmup = MessagePackSerializer.Serialize(data, MsgPackOptions);
        MessagePackSerializer.Deserialize<T>(warmup, MsgPackOptions);

        var sw = Stopwatch.StartNew();
        byte[]? bytes = null;
        for (int i = 0; i < iterations; i++)
            bytes = MessagePackSerializer.Serialize(data, MsgPackOptions);
        sw.Stop();
        double serMs = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        for (int i = 0; i < iterations; i++)
            MessagePackSerializer.Deserialize<T>(bytes!, MsgPackOptions);
        sw.Stop();

        return (bytes!.Length, serMs, sw.Elapsed.TotalMilliseconds);
    }

    private static (long size, double serMs, double deserMs) MeasureMemoryPack<T>(T data, int iterations)
    {
        // Warmup
        var warmup = MemoryPackSerializer.Serialize(data);
        MemoryPackSerializer.Deserialize<T>(warmup);

        var sw = Stopwatch.StartNew();
        byte[]? bytes = null;
        for (int i = 0; i < iterations; i++)
            bytes = MemoryPackSerializer.Serialize(data);
        sw.Stop();
        double serMs = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        for (int i = 0; i < iterations; i++)
            MemoryPackSerializer.Deserialize<T>(bytes!);
        sw.Stop();

        return (bytes!.Length, serMs, sw.Elapsed.TotalMilliseconds);
    }
}
