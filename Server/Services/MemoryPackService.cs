using MemoryPack;
using SerializerBenchmark.Core.Utility;
using SerializerBenchmark.Server.Interfaces;
using SerializerBenchmark.Server.Models;
using System.Diagnostics;

namespace SerializerBenchmark.Server.Services;

public class MemoryPackService : ISerializerService
{
    public string Name => SerializerName.MemoryPack;

    public MeasureResult Measure<T>(T data, int iterations)
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

        return new MeasureResult(bytes!.Length, serMs, sw.Elapsed.TotalMilliseconds);
    }
}
