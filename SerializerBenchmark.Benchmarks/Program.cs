using BenchmarkDotNet.Running;
using SerializerBenchmark.Benchmarks;

BenchmarkRunner.Run<AllSerializerBenchmarks>(args: args);