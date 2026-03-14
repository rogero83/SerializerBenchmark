using ProtoBuf;
using SerializerBenchmark.Core.Interfaces;
using SerializerBenchmark.Core.Models;
using System.Diagnostics;

namespace SerializerBenchmark.Server.Services;

public class ProtobufService : ISerializerService
{
    public string Name => SerializerName.Protobuf;

    public MeasureResult Measure<T>(T data, int iterations)
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

        return new MeasureResult(bytes!.Length, serMs, sw.Elapsed.TotalMilliseconds);
    }
}
