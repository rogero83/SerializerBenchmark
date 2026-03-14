using SerializerBenchmark.Server.Models;

namespace SerializerBenchmark.Server.Interfaces;

public interface ISerializerService
{
    public string Name { get; }

    MeasureResult Measure<T>(T data, int iterations);
}
