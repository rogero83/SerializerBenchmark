using BenchmarkDotNet.Attributes;
using MessagePack;
using ProtoBuf;
using SerializerBenchmark.Core.Models;
using System.Text.Json;

namespace SerializerBenchmark.Benchmarks;

public partial class AllSerializerBenchmarks
{
    // ─────────────────────────────────────────
    // Integers - Serialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Serialization Integers"), Benchmark(Baseline = true)]
    public string Json_Integers_Serialize() => JsonSerializer.Serialize(_integers, MyJsonSerializerContext.Default.IntegerPayload);

    [BenchmarkCategory("Serialization Integers"), Benchmark()]
    public byte[] Protobuf_Integers_Serialize()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _integers);
        return ms.ToArray();
    }

    [BenchmarkCategory("Serialization Integers"), Benchmark()]
    public byte[] MessagePack_Integers_Serialize() => MessagePackSerializer.Serialize(_integers, MsgPackOptions);

    [BenchmarkCategory("Serialization Integers"), Benchmark()]
    public byte[] MemoryPack_Integers_Serialize() => MemoryPack.MemoryPackSerializer.Serialize(_integers);

    // ─────────────────────────────────────────
    // Integers - Deserialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Deserialization Integers"), Benchmark(Baseline = true)]
    public IntegerPayload? Json_Integers_Deserialize() => JsonSerializer.Deserialize(_jsonIntegersString, MyJsonSerializerContext.Default.IntegerPayload);

    [BenchmarkCategory("Deserialization Integers"), Benchmark()]
    public IntegerPayload Protobuf_Integers_Deserialize()
    {
        using var ms = new MemoryStream(_protobufIntegersBytes);
        return Serializer.Deserialize<IntegerPayload>(ms);
    }

    [BenchmarkCategory("Deserialization Integers"), Benchmark()]
    public IntegerPayload MessagePack_Integers_Deserialize() => MessagePackSerializer.Deserialize<IntegerPayload>(_msgPackIntegersBytes, MsgPackOptions);

    [BenchmarkCategory("Deserialization Integers"), Benchmark()]
    public IntegerPayload? MemoryPack_Integers_Deserialize() => MemoryPack.MemoryPackSerializer.Deserialize<IntegerPayload>(_memPackIntegersBytes);
}
