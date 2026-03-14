namespace SerializerBenchmark.Core.Models;

public class SerialializerModel
{
    public required string Id { get; set; }
    public required string Label { get; set; }
    public required string Version { get; set; }
    public required string Color { get; set; }

    public static SerialializerModel[] SerialializerModels => new SerialializerModel[]
        {
            new() {
                Id = SerializerName.SystemTextJson,
                Label = "System.Text.Json",
                Version = "10.0",
                Color = "#f97316"
            },
            new() {
                Id = SerializerName.SystemTextJsonUtf8,
                Label = "System.Text.Json Utf8",
                Version = "10.0",
                Color = "#527316"
            },
            new(){
                Id = SerializerName.Protobuf,
                Label = "protobuf-net",
                Version = "3.2.56",
                Color = "#3b82f6"
            },
            new(){
                Id = SerializerName.MessagePack,
                Label = "MessagePack",
                Version = "3.1.4",
                Color = "#a855f7"
            },
            new(){
                Id = SerializerName.MemoryPack,
                Label= "MemoryPack",
                Version = "1.21.4",
                Color = "#22c55e"
            },
        };

    public static string[] Serializers => [.. SerialializerModels.Select(s => s.Id)];
}