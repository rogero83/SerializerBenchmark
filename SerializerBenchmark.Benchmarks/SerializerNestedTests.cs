using BenchmarkDotNet.Attributes;
using MessagePack;
using ProtoBuf;
using SerializerBenchmark.Core.Models;
using System.Text.Json;

namespace SerializerBenchmark.Benchmarks;

public partial class AllSerializerBenchmarks
{
    // ─────────────────────────────────────────
    // Nested - Serialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Serialization Nested"), Benchmark(Baseline = true)]
    public string Json_Nested_Serialize() => JsonSerializer.Serialize(_nested, MyJsonSerializerContext.Default.NestedPayload);

    [BenchmarkCategory("Serialization Nested"), Benchmark()]
    public byte[] Protobuf_Nested_Serialize()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _nested);
        return ms.ToArray();
    }

    [BenchmarkCategory("Serialization Nested"), Benchmark()]
    public byte[] MessagePack_Nested_Serialize() => MessagePackSerializer.Serialize(_nested, MsgPackOptions);

    [BenchmarkCategory("Serialization Nested"), Benchmark()]
    public byte[] MemoryPack_Nested_Serialize() => MemoryPack.MemoryPackSerializer.Serialize(_nested);

    // ─────────────────────────────────────────
    // Nested - Deserialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Deserialization Nested"), Benchmark(Baseline = true)]
    public NestedPayload? Json_Nested_Deserialize() => JsonSerializer.Deserialize(_jsonNestedString, MyJsonSerializerContext.Default.NestedPayload);

    [BenchmarkCategory("Deserialization Nested"), Benchmark()]
    public NestedPayload Protobuf_Nested_Deserialize()
    {
        using var ms = new MemoryStream(_protobufNestedBytes);
        return Serializer.Deserialize<NestedPayload>(ms);
    }

    [BenchmarkCategory("Deserialization Nested"), Benchmark()]
    public NestedPayload MessagePack_Nested_Deserialize() => MessagePackSerializer.Deserialize<NestedPayload>(_msgPackNestedBytes, MsgPackOptions);

    [BenchmarkCategory("Deserialization Nested"), Benchmark()]
    public NestedPayload? MemoryPack_Nested_Deserialize() => MemoryPack.MemoryPackSerializer.Deserialize<NestedPayload>(_memPackNestedBytes);
}
