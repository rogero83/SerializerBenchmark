namespace SerializerBenchmark.Server.Models;

public record MeasureResult(long Size, double SerMs, double DeserMs);
