using SerializerBenchmark.Core.Models;

namespace SerializerBenchmark.Core.Interfaces;

public interface ISerializerService
{
    public string Name { get; }

    MeasureResult Measure<T>(T data, int iterations);
}
