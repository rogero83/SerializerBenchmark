using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using MemoryPack;
using MessagePack;
using ProtoBuf;
using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Core.Services;
using System.Text.Json;

namespace SerializerBenchmark.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public partial class AllSerializerBenchmarks
{
    protected static readonly MessagePackSerializerOptions MsgPackOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.None);

    [Params(100/*, 1000*/)]
    public int ItemCount { get; set; }

    // Data for Integers
    private IntegerPayload _integers = null!;
    private byte[] _protobufIntegersBytes = null!;
    private byte[] _msgPackIntegersBytes = null!;
    private byte[] _memPackIntegersBytes = null!;
    private string _jsonIntegersString = null!;

    // Data for Decimal
    private DecimalPayload _decimal = null!;
    private byte[] _protobufDecimalsBytes = null!;
    private byte[] _msgPackDecimalsBytes = null!;
    private byte[] _memPackDecimalsBytes = null!;
    private string _jsonDecimalsString = null!;

    // Data for Floats
    private FloatPayload _floats = null!;
    private byte[] _protobufFloatsBytes = null!;
    private byte[] _msgPackFloatsBytes = null!;
    private byte[] _memPackFloatsBytes = null!;
    private string _jsonFloatsString = null!;

    // Data for Strings
    private StringPayload _strings = null!;
    private byte[] _protobufStringsBytes = null!;
    private byte[] _msgPackStringsBytes = null!;
    private byte[] _memPackStringsBytes = null!;
    private string _jsonStringsString = null!;

    // Data for Nested
    private NestedPayload _nested = null!;
    private byte[] _protobufNestedBytes = null!;
    private byte[] _msgPackNestedBytes = null!;
    private byte[] _memPackNestedBytes = null!;
    private string _jsonNestedString = null!;

    // Data for Temporal
    private TemporalPayload _temporal = null!;
    private byte[] _protobufTemporalBytes = null!;
    private byte[] _msgPackTemporalBytes = null!;
    private byte[] _memPackTemporalBytes = null!;
    private string _jsonTemporalString = null!;

    // Data for Log
    private LogPayload _logs = null!;
    private byte[] _protobufLogBytes = null!;
    private byte[] _msgPackLogBytes = null!;
    private byte[] _memPackLogBytes = null!;
    private string _jsonLogString = null!;

    // Data for BulkArray
    private BulkArrayPayload _bulkArray = null!;
    private byte[] _protobufBulkArrayBytes = null!;
    private byte[] _msgPackBulkArrayBytes = null!;
    private byte[] _memPackBulkArrayBytes = null!;
    private string _jsonBulkArrayString = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Integers Setup
        _integers = DataGenerator.GenerateIntegers(ItemCount);
        _jsonIntegersString = JsonSerializer.Serialize(_integers, MyJsonSerializerContext.Default.IntegerPayload);
        _msgPackIntegersBytes = MessagePackSerializer.Serialize(_integers, MsgPackOptions);
        _memPackIntegersBytes = MemoryPackSerializer.Serialize(_integers);
        _protobufIntegersBytes = SerializeProtobuf(_integers);

        // Decimals Setup
        _decimal = DataGenerator.GenerateDecimal(ItemCount);
        _jsonDecimalsString = JsonSerializer.Serialize(_decimal, MyJsonSerializerContext.Default.DecimalPayload);
        _msgPackDecimalsBytes = MessagePackSerializer.Serialize(_decimal, MsgPackOptions);
        _memPackDecimalsBytes = MemoryPackSerializer.Serialize(_decimal);
        _protobufDecimalsBytes = SerializeProtobuf(_decimal);

        // Floats Setup
        _floats = DataGenerator.GenerateFloats(ItemCount);
        _jsonFloatsString = JsonSerializer.Serialize(_floats, MyJsonSerializerContext.Default.FloatPayload);
        _msgPackFloatsBytes = MessagePackSerializer.Serialize(_floats, MsgPackOptions);
        _memPackFloatsBytes = MemoryPackSerializer.Serialize(_floats);
        _protobufFloatsBytes = SerializeProtobuf(_floats);

        // Strings Setup
        _strings = DataGenerator.GenerateStrings(ItemCount);
        _jsonStringsString = JsonSerializer.Serialize(_strings, MyJsonSerializerContext.Default.StringPayload);
        _msgPackStringsBytes = MessagePackSerializer.Serialize(_strings, MsgPackOptions);
        _memPackStringsBytes = MemoryPackSerializer.Serialize(_strings);
        _protobufStringsBytes = SerializeProtobuf(_strings);

        // Nested Setup
        _nested = DataGenerator.GenerateNested(ItemCount);
        _jsonNestedString = JsonSerializer.Serialize(_nested, MyJsonSerializerContext.Default.NestedPayload);
        _msgPackNestedBytes = MessagePackSerializer.Serialize(_nested, MsgPackOptions);
        _memPackNestedBytes = MemoryPackSerializer.Serialize(_nested);
        _protobufNestedBytes = SerializeProtobuf(_nested);

        // Temporal Setup
        _temporal = DataGenerator.GenerateTemporal(ItemCount);
        _jsonTemporalString = JsonSerializer.Serialize(_temporal, MyJsonSerializerContext.Default.TemporalPayload);
        _msgPackTemporalBytes = MessagePackSerializer.Serialize(_temporal, MsgPackOptions);
        _memPackTemporalBytes = MemoryPackSerializer.Serialize(_temporal);
        _protobufTemporalBytes = SerializeProtobuf(_temporal);

        // Log Setup
        _logs = DataGenerator.GenerateLogs(ItemCount);
        _jsonLogString = JsonSerializer.Serialize(_logs, MyJsonSerializerContext.Default.LogPayload);
        _msgPackLogBytes = MessagePackSerializer.Serialize(_logs, MsgPackOptions);
        _memPackLogBytes = MemoryPackSerializer.Serialize(_logs);
        _protobufLogBytes = SerializeProtobuf(_logs);

        // BulkArray Setup
        _bulkArray = DataGenerator.GenerateBulkArray(ItemCount);
        _jsonBulkArrayString = JsonSerializer.Serialize(_bulkArray, MyJsonSerializerContext.Default.BulkArrayPayload);
        _msgPackBulkArrayBytes = MessagePackSerializer.Serialize(_bulkArray, MsgPackOptions);
        _memPackBulkArrayBytes = MemoryPackSerializer.Serialize(_bulkArray);
        _protobufBulkArrayBytes = SerializeProtobuf(_bulkArray);
    }

    private static byte[] SerializeProtobuf<T>(T data) where T : class
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, data);
        return ms.ToArray();
    }
}
