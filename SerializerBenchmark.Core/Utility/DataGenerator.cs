using SerializerBenchmark.Core.Models;

namespace SerializerBenchmark.Core.Utility;

public static class DataGenerator
{
    private static readonly Random _rnd = new(42);

    private static readonly string[] Levels = ["DEBUG", "INFO", "WARN", "ERROR", "FATAL"];
    private static readonly string[] Services = ["api-gateway", "auth-service", "db-proxy", "cache", "worker"];
    private static readonly string[] Environments = ["production", "staging", "development"];
    private static readonly string[] Cities = ["Milano", "Roma", "Torino", "Napoli", "Bologna", "Firenze"];
    private static readonly string[] Countries = ["Italy", "Germany", "France", "Spain", "UK"];
    private static readonly string[] Tags = ["vip", "premium", "trial", "admin", "guest", "beta", "verified"];
    private static readonly int[] int32Array = [200, 200, 200, 201, 400, 404, 500];

    public static IntegerPayload GenerateIntegers(int count) => new()
    {
        Values = Enumerable.Range(0, count).Select(_ => _rnd.Next()).ToArray(),
        LongValues = Enumerable.Range(0, count).Select(_ => _rnd.NextInt64()).ToArray(),
    };

    public static DecimalPayload GenerateDecimal(int count) => new()
    {
        Price = Enumerable.Range(0, count).Select(_ => (_rnd.Next(10) - 5) * 1_000_000m).ToArray()
    };

    public static FloatPayload GenerateFloats(int count) => new()
    {
        Values = Enumerable.Range(0, count).Select(_ => _rnd.NextDouble() * 1_000_000).ToArray(),
        FloatValues = Enumerable.Range(0, count).Select(_ => (float)(_rnd.NextDouble() * 1000)).ToArray(),
    };

    public static StringPayload GenerateStrings(int count) => new()
    {
        Values = Enumerable.Range(0, count)
            .Select(_ => GenerateRandomString(_rnd.Next(8, 64)))
            .ToArray(),
    };

    public static NestedPayload GenerateNested(int count) => new()
    {
        People = Enumerable.Range(0, count).Select(i => new NestedPerson
        {
            Id = i,
            Name = $"User_{GenerateRandomString(8)}",
            Age = _rnd.Next(18, 80),
            Contact = new ContactInfo
            {
                Email = $"user{i}@example.com",
                Phone = $"+39{_rnd.Next(300, 399)}{_rnd.Next(1000000, 9999999)}",
                HomeAddress = new Address
                {
                    Street = $"Via {GenerateRandomString(8)} {_rnd.Next(1, 200)}",
                    City = Cities[_rnd.Next(Cities.Length)],
                    ZipCode = _rnd.Next(10000, 99999).ToString(),
                    Country = "Italy",
                },
                WorkAddress = new Address
                {
                    Street = $"Corso {GenerateRandomString(6)} {_rnd.Next(1, 100)}",
                    City = Cities[_rnd.Next(Cities.Length)],
                    ZipCode = _rnd.Next(10000, 99999).ToString(),
                    Country = Countries[_rnd.Next(Countries.Length)],
                },
            },
            Tags = Enumerable.Range(0, _rnd.Next(1, 5)).Select(_ => Tags[_rnd.Next(Tags.Length)]).ToArray(),
            Metadata = new Dictionary<string, string>
            {
                ["created_by"] = "system",
                ["source"] = "import",
                ["region"] = "eu-west",
            },
        }).ToArray(),
    };

    public static TemporalPayload GenerateTemporal(int count)
    {
        var now = DateTime.UtcNow;
        return new TemporalPayload
        {
            Records = Enumerable.Range(0, count).Select(i => new TemporalRecord
            {
                Id = Guid.NewGuid(),
                CreatedAt = now.AddDays(-_rnd.Next(0, 365)),
                UpdatedAt = now.AddMinutes(-_rnd.Next(0, 1440)),
                Name = $"Record_{GenerateRandomString(10)}",
                Version = _rnd.NextInt64(1, 999),
            }).ToArray(),
        };
    }

    public static LogPayload GenerateLogs(int count) => new()
    {
        Entries = Enumerable.Range(0, count).Select(_ => new LogEntry
        {
            Level = Levels[_rnd.Next(Levels.Length)],
            Service = Services[_rnd.Next(Services.Length)],
            Environment = Environments[_rnd.Next(Environments.Length)],
            StatusCode = int32Array[_rnd.Next(7)],
            Message = $"Request processed in {_rnd.Next(1, 2000)}ms",
            Timestamp = DateTimeOffset.UtcNow.AddMinutes(-_rnd.Next(0, 10000)).ToUnixTimeMilliseconds(),
        }).ToArray(),
    };

    public static BulkArrayPayload GenerateBulkArray(int count) => new()
    {
        IntArray = Enumerable.Range(0, count).Select(_ => _rnd.Next()).ToArray(),
        DoubleArray = Enumerable.Range(0, count).Select(_ => _rnd.NextDouble() * 100000).ToArray(),
        ByteArray = GenerateRandomBytes(count),
    };

    private static byte[] GenerateRandomBytes(int count)
    {
        var bytes = new byte[count];
        _rnd.NextBytes(bytes);
        return bytes;
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, length).Select(_ => chars[_rnd.Next(chars.Length)]).ToArray());
    }
}
