using BenchmarkDotNet.Attributes;
using MessagePack;
using ProtoBuf;
using SerializerBenchmark.Core.Models;
using System.Text.Json;

namespace SerializerBenchmark.Benchmarks;

public partial class AllSerializerBenchmarks
{
    // ─────────────────────────────────────────
    // Floats - Serialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Serialization Floats"), Benchmark(Baseline = true)]
    public string Json_Floats_Serialize() => JsonSerializer.Serialize(_floats, MyJsonSerializerContext.Default.FloatPayload);

    [BenchmarkCategory("Serialization Floats"), Benchmark()]
    public byte[] Protobuf_Floats_Serialize()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _floats);
        return ms.ToArray();
    }

    [BenchmarkCategory("Serialization Floats"), Benchmark()]
    public byte[] MessagePack_Floats_Serialize() => MessagePackSerializer.Serialize(_floats, MsgPackOptions);

    [BenchmarkCategory("Serialization Floats"), Benchmark()]
    public byte[] MemoryPack_Floats_Serialize() => MemoryPack.MemoryPackSerializer.Serialize(_floats);

    // ─────────────────────────────────────────
    // Floats - Deserialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Deserialization Floats"), Benchmark(Baseline = true)]
    public FloatPayload? Json_Floats_Deserialize() => JsonSerializer.Deserialize(_jsonFloatsString, MyJsonSerializerContext.Default.FloatPayload);

    [BenchmarkCategory("Deserialization Floats"), Benchmark()]
    public FloatPayload Protobuf_Floats_Deserialize()
    {
        using var ms = new MemoryStream(_protobufFloatsBytes);
        return Serializer.Deserialize<FloatPayload>(ms);
    }

    [BenchmarkCategory("Deserialization Floats"), Benchmark()]
    public FloatPayload MessagePack_Floats_Deserialize() => MessagePackSerializer.Deserialize<FloatPayload>(_msgPackFloatsBytes, MsgPackOptions);

    [BenchmarkCategory("Deserialization Floats"), Benchmark()]
    public FloatPayload? MemoryPack_Floats_Deserialize() => MemoryPack.MemoryPackSerializer.Deserialize<FloatPayload>(_memPackFloatsBytes);
}
