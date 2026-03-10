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

    [BenchmarkCategory("Serialization Decimals"), Benchmark(Baseline = true)]
    public string Json_Decimals_Serialize() => JsonSerializer.Serialize(_decimal, MyJsonSerializerContext.Default.DecimalPayload);

    [BenchmarkCategory("Serialization Decimals"), Benchmark()]
    public byte[] Protobuf_Decimals_Serialize()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _decimal);
        return ms.ToArray();
    }

    [BenchmarkCategory("Serialization Decimals"), Benchmark()]
    public byte[] MessagePack_Decimals_Serialize() => MessagePackSerializer.Serialize(_decimal, MsgPackOptions);

    [BenchmarkCategory("Serialization Decimals"), Benchmark()]
    public byte[] MemoryPack_Decimals_Serialize() => MemoryPack.MemoryPackSerializer.Serialize(_decimal);

    // ─────────────────────────────────────────
    // Deserialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Deserialization Decimals"), Benchmark(Baseline = true)]
    public DecimalPayload? Json_Decimals_Deserialize() => JsonSerializer.Deserialize(_jsonDecimalsString, MyJsonSerializerContext.Default.DecimalPayload);

    [BenchmarkCategory("Deserialization Decimals"), Benchmark()]
    public DecimalPayload Protobuf_Decimals_Deserialize()
    {
        using var ms = new MemoryStream(_protobufDecimalsBytes);
        return Serializer.Deserialize<DecimalPayload>(ms);
    }

    [BenchmarkCategory("Deserialization Decimals"), Benchmark()]
    public DecimalPayload MessagePack_Decimals_Deserialize() => MessagePackSerializer.Deserialize<DecimalPayload>(_msgPackDecimalsBytes, MsgPackOptions);

    [BenchmarkCategory("Deserialization Decimals"), Benchmark()]
    public DecimalPayload? MemoryPack_Decimals_Deserialize() => MemoryPack.MemoryPackSerializer.Deserialize<DecimalPayload>(_memPackDecimalsBytes);
}
