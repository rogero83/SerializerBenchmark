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

    [BenchmarkCategory("Serialization BulkArray"), Benchmark(Baseline = true)]
    public byte[] Json_BulkArray_Serialize() => JsonSerializer.SerializeToUtf8Bytes(_bulkArray, MyJsonSerializerContext.Default.BulkArrayPayload);

    [BenchmarkCategory("Serialization BulkArray"), Benchmark()]
    public byte[] Protobuf_BulkArray_Serialize()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _bulkArray);
        return ms.ToArray();
    }

    [BenchmarkCategory("Serialization BulkArray"), Benchmark()]
    public byte[] MessagePack_BulkArray_Serialize() => MessagePackSerializer.Serialize(_bulkArray, MsgPackOptions);

    [BenchmarkCategory("Serialization BulkArray"), Benchmark()]
    public byte[] MemoryPack_BulkArray_Serialize() => MemoryPack.MemoryPackSerializer.Serialize(_bulkArray);

    // ─────────────────────────────────────────
    // Deserialization
    // ─────────────────────────────────────────

    [BenchmarkCategory("Deserialization BulkArray"), Benchmark(Baseline = true)]
    public BulkArrayPayload? Json_BulkArray_Deserialize() => JsonSerializer.Deserialize(_jsonBulkArrayString, MyJsonSerializerContext.Default.BulkArrayPayload);

    [BenchmarkCategory("Deserialization BulkArray"), Benchmark()]
    public BulkArrayPayload Protobuf_BulkArray_Deserialize()
    {
        using var ms = new MemoryStream(_protobufBulkArrayBytes);
        return Serializer.Deserialize<BulkArrayPayload>(ms);
    }

    [BenchmarkCategory("Deserialization BulkArray"), Benchmark()]
    public BulkArrayPayload MessagePack_BulkArray_Deserialize() => MessagePackSerializer.Deserialize<BulkArrayPayload>(_msgPackBulkArrayBytes, MsgPackOptions);

    [BenchmarkCategory("Deserialization BulkArray"), Benchmark()]
    public BulkArrayPayload? MemoryPack_BulkArray_Deserialize() => MemoryPack.MemoryPackSerializer.Deserialize<BulkArrayPayload>(_memPackBulkArrayBytes);
}
