using BenchmarkDotNet.Attributes;
using MessagePack;
using ProtoBuf;
using SerializerBenchmark.Core.Models;
using System.Text.Json;

namespace SerializerBenchmark.Benchmarks;

public partial class AllSerializerBenchmarks
{
    // ─────────────────────────────────────────
    // Serialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Serialization Log"), Benchmark(Baseline = true)]
    public string Json_Log_Serialize() => JsonSerializer.Serialize(_logs, MyJsonSerializerContext.Default.LogPayload);

    [BenchmarkCategory("Serialization Log"), Benchmark()]
    public byte[] Protobuf_Log_Serialize()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _logs);
        return ms.ToArray();
    }

    [BenchmarkCategory("Serialization Log"), Benchmark()]
    public byte[] MessagePack_Log_Serialize() => MessagePackSerializer.Serialize(_logs, MsgPackOptions);

    [BenchmarkCategory("Serialization Log"), Benchmark()]
    public byte[] MemoryPack_Log_Serialize() => MemoryPack.MemoryPackSerializer.Serialize(_logs);

    // ─────────────────────────────────────────
    // Deserialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Deserialization Log"), Benchmark(Baseline = true)]
    public LogPayload? Json_Log_Deserialize() => JsonSerializer.Deserialize(_jsonLogString, MyJsonSerializerContext.Default.LogPayload);

    [BenchmarkCategory("Deserialization Log"), Benchmark()]
    public LogPayload Protobuf_Log_Deserialize()
    {
        using var ms = new MemoryStream(_protobufLogBytes);
        return Serializer.Deserialize<LogPayload>(ms);
    }

    [BenchmarkCategory("Deserialization Log"), Benchmark()]
    public LogPayload MessagePack_Log_Deserialize() => MessagePackSerializer.Deserialize<LogPayload>(_msgPackLogBytes, MsgPackOptions);

    [BenchmarkCategory("Deserialization Log"), Benchmark()]
    public LogPayload? MemoryPack_Log_Deserialize() => MemoryPack.MemoryPackSerializer.Deserialize<LogPayload>(_memPackLogBytes);
}
