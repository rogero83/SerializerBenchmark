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

    [BenchmarkCategory("Serialization Strings"), Benchmark(Baseline = true)]
    public string Json_Strings_Serialize() => JsonSerializer.Serialize(_strings, MyJsonSerializerContext.Default.StringPayload);

    [BenchmarkCategory("Serialization Strings"), Benchmark()]
    public byte[] Protobuf_Strings_Serialize()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _strings);
        return ms.ToArray();
    }

    [BenchmarkCategory("Serialization Strings"), Benchmark()]
    public byte[] MessagePack_Strings_Serialize() => MessagePackSerializer.Serialize(_strings, MsgPackOptions);

    [BenchmarkCategory("Serialization Strings"), Benchmark()]
    public byte[] MemoryPack_Strings_Serialize() => MemoryPack.MemoryPackSerializer.Serialize(_strings);

    // ─────────────────────────────────────────
    // Deserialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Deserialization Strings"), Benchmark(Baseline = true)]
    public StringPayload? Json_Strings_Deserialize() => JsonSerializer.Deserialize(_jsonStringsString, MyJsonSerializerContext.Default.StringPayload);

    [BenchmarkCategory("Deserialization Strings"), Benchmark()]
    public StringPayload Protobuf_Strings_Deserialize()
    {
        using var ms = new MemoryStream(_protobufStringsBytes);
        return Serializer.Deserialize<StringPayload>(ms);
    }

    [BenchmarkCategory("Deserialization Strings"), Benchmark()]
    public StringPayload MessagePack_Strings_Deserialize() => MessagePackSerializer.Deserialize<StringPayload>(_msgPackStringsBytes, MsgPackOptions);

    [BenchmarkCategory("Deserialization Strings"), Benchmark()]
    public StringPayload? MemoryPack_Strings_Deserialize() => MemoryPack.MemoryPackSerializer.Deserialize<StringPayload>(_memPackStringsBytes);
}
