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

    [BenchmarkCategory("Serialization Temporal"), Benchmark(Baseline = true)]
    public string Json_Temporal_Serialize() => JsonSerializer.Serialize(_temporal, MyJsonSerializerContext.Default.TemporalPayload);

    [BenchmarkCategory("Serialization Temporal"), Benchmark()]
    public byte[] Protobuf_Temporal_Serialize()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _temporal);
        return ms.ToArray();
    }

    [BenchmarkCategory("Serialization Temporal"), Benchmark()]
    public byte[] MessagePack_Temporal_Serialize() => MessagePackSerializer.Serialize(_temporal, MsgPackOptions);

    [BenchmarkCategory("Serialization Temporal"), Benchmark()]
    public byte[] MemoryPack_Temporal_Serialize() => MemoryPack.MemoryPackSerializer.Serialize(_temporal);

    // ─────────────────────────────────────────
    // Deserialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Deserialization Temporal"), Benchmark(Baseline = true)]
    public TemporalPayload? Json_Temporal_Deserialize() => JsonSerializer.Deserialize(_jsonTemporalString, MyJsonSerializerContext.Default.TemporalPayload);

    [BenchmarkCategory("Deserialization Temporal"), Benchmark()]
    public TemporalPayload Protobuf_Temporal_Deserialize()
    {
        using var ms = new MemoryStream(_protobufTemporalBytes);
        return Serializer.Deserialize<TemporalPayload>(ms);
    }

    [BenchmarkCategory("Deserialization Temporal"), Benchmark()]
    public TemporalPayload MessagePack_Temporal_Deserialize() => MessagePackSerializer.Deserialize<TemporalPayload>(_msgPackTemporalBytes, MsgPackOptions);

    [BenchmarkCategory("Deserialization Temporal"), Benchmark()]
    public TemporalPayload? MemoryPack_Temporal_Deserialize() => MemoryPack.MemoryPackSerializer.Deserialize<TemporalPayload>(_memPackTemporalBytes);
}
