using MemoryPack;
using MessagePack;
using ProtoBuf;
using System.Text.Json.Serialization;

namespace SerializerBenchmark.Core.Models;

// ─────────────────────────────────────────────
// Scenario: Interi
// ─────────────────────────────────────────────
[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class IntegerPayload
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public int[] Values { get; set; } = [];
    [MemoryPackOrder(2)][ProtoMember(2)][Key(1)] public long[] LongValues { get; set; } = [];
}

// ─────────────────────────────────────────────
// Scenario: Decimal
// ─────────────────────────────────────────────
[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class DecimalPayload
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public decimal[] Price { get; set; } = [];
}

// ─────────────────────────────────────────────
// Scenario: Float / Double
// ─────────────────────────────────────────────
[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class FloatPayload
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public double[] Values { get; set; } = [];
    [MemoryPackOrder(2)][ProtoMember(2)][Key(1)] public float[] FloatValues { get; set; } = [];
}

// ─────────────────────────────────────────────
// Scenario: Stringhe
// ─────────────────────────────────────────────
[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class StringPayload
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public string[] Values { get; set; } = [];
}

// ─────────────────────────────────────────────
// Scenario: Oggetti annidati
// ─────────────────────────────────────────────
[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class Address
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public string Street { get; set; } = "";
    [MemoryPackOrder(1)][ProtoMember(2)][Key(1)] public string City { get; set; } = "";
    [MemoryPackOrder(2)][ProtoMember(3)][Key(2)] public string ZipCode { get; set; } = "";
    [MemoryPackOrder(3)][ProtoMember(4)][Key(3)] public string Country { get; set; } = "";
}

[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class ContactInfo
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public string Email { get; set; } = "";
    [MemoryPackOrder(1)][ProtoMember(2)][Key(1)] public string Phone { get; set; } = "";
    [MemoryPackOrder(3)][ProtoMember(3)][Key(2)] public Address? HomeAddress { get; set; }
    [MemoryPackOrder(4)][ProtoMember(4)][Key(3)] public Address? WorkAddress { get; set; }
}

[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class NestedPerson
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public int Id { get; set; }
    [MemoryPackOrder(1)][ProtoMember(2)][Key(1)] public string Name { get; set; } = "";
    [MemoryPackOrder(2)][ProtoMember(3)][Key(2)] public int Age { get; set; }
    [MemoryPackOrder(3)][ProtoMember(4)][Key(3)] public ContactInfo? Contact { get; set; }
    [MemoryPackOrder(4)][ProtoMember(5)][Key(4)] public string[] Tags { get; set; } = [];
    [MemoryPackOrder(5)][ProtoMember(6)][Key(5)] public Dictionary<string, string> Metadata { get; set; } = [];
}

[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class NestedPayload
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public NestedPerson[] People { get; set; } = [];
}

// ─────────────────────────────────────────────
// Scenario: DateTime / GUID
// ─────────────────────────────────────────────
[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class TemporalRecord
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public Guid Id { get; set; }
    [MemoryPackOrder(1)][ProtoMember(2)][Key(1)] public DateTime CreatedAt { get; set; }
    [MemoryPackOrder(2)][ProtoMember(3)][Key(2)] public DateTime UpdatedAt { get; set; }
    [MemoryPackOrder(3)][ProtoMember(4)][Key(3)] public string Name { get; set; } = "";
    [MemoryPackOrder(4)][ProtoMember(5)][Key(4)] public long Version { get; set; }
}

[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class TemporalPayload
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public TemporalRecord[] Records { get; set; } = [];
}

// ─────────────────────────────────────────────
// Scenario: Dati ripetuti
// ─────────────────────────────────────────────
[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class LogEntry
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public string Level { get; set; } = "";        // "INFO", "WARN", etc.
    [MemoryPackOrder(1)][ProtoMember(2)][Key(1)] public string Service { get; set; } = "";      // "api", "db", etc.
    [MemoryPackOrder(2)][ProtoMember(3)][Key(2)] public string Environment { get; set; } = "";  // "production"
    [MemoryPackOrder(3)][ProtoMember(4)][Key(3)] public int StatusCode { get; set; }
    [MemoryPackOrder(4)][ProtoMember(5)][Key(4)] public string Message { get; set; } = "";
    [MemoryPackOrder(5)][ProtoMember(6)][Key(5)] public long Timestamp { get; set; }
}

[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class LogPayload
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public LogEntry[] Entries { get; set; } = [];
}

// ─────────────────────────────────────────────
// Scenario: Array primitivi bulk
// ─────────────────────────────────────────────
[ProtoContract]
[MessagePackObject]
[MemoryPackable]
public partial class BulkArrayPayload
{
    [MemoryPackOrder(0)][ProtoMember(1)][Key(0)] public int[] IntArray { get; set; } = [];
    [MemoryPackOrder(1)][ProtoMember(2)][Key(1)] public double[] DoubleArray { get; set; } = [];
    [MemoryPackOrder(2)][ProtoMember(3)][Key(2)] public byte[] ByteArray { get; set; } = [];
}

// ─────────────────────────────────────────────
// Risultato benchmark
// ─────────────────────────────────────────────
public class BenchmarkResult
{
    public string Serializer { get; set; } = "";
    public string Scenario { get; set; } = "";
    public long SizeBytes { get; set; }
    public double SerializeMs { get; set; }
    public double DeserializeMs { get; set; }
    public int Iterations { get; set; }
    public string? Error { get; set; }
}

public class BenchmarkRequest
{
    public string Scenario { get; set; } = "integers";
    public int ItemCount { get; set; } = 1000;
    public int Iterations { get; set; } = 100;
    public string[] Serializers { get; set; } = SerialializerModel.Serializers;
}

/// <summary>
/// Provides a source-generated context for JSON serialization and deserialization of supported payload types.
/// </summary>
/// <remarks>This context enables efficient serialization and deserialization operations for the specified types
/// using System.Text.Json. It is intended to be used with source generation to improve performance and reduce runtime
/// reflection overhead when working with JSON data.</remarks>
[JsonSerializable(typeof(IntegerPayload))]
[JsonSerializable(typeof(DecimalPayload))]
[JsonSerializable(typeof(FloatPayload))]
[JsonSerializable(typeof(StringPayload))]
[JsonSerializable(typeof(Address))]
[JsonSerializable(typeof(ContactInfo))]
[JsonSerializable(typeof(NestedPerson))]
[JsonSerializable(typeof(NestedPayload))]
[JsonSerializable(typeof(TemporalRecord))]
[JsonSerializable(typeof(TemporalPayload))]
[JsonSerializable(typeof(LogEntry))]
[JsonSerializable(typeof(LogPayload))]
[JsonSerializable(typeof(BulkArrayPayload))]
public partial class MyJsonSerializerContext : JsonSerializerContext { }