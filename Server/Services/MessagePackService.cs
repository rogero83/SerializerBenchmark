using MessagePack;
using SerializerBenchmark.Core.Interfaces;
using SerializerBenchmark.Core.Models;
using System.Diagnostics;

namespace SerializerBenchmark.Server.Services;

public class MessagePackService : ISerializerService
{
    private static readonly MessagePackSerializerOptions MsgPackOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.None);

    public string Name => SerializerName.MessagePack;

    public MeasureResult Measure<T>(T data, int iterations)
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

        return new MeasureResult(bytes!.Length, serMs, sw.Elapsed.TotalMilliseconds);
    }
}
