using Microsoft.AspNetCore.Mvc;
using SerializerBenchmark.Core.Models;
using SerializerBenchmark.Core.Utility;
using SerializerBenchmark.Server.Services;

namespace SerializerBenchmark.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BenchmarkController(BenchmarkService benchmarkService,
    ILogger<BenchmarkController> logger) : ControllerBase
{
    /// <summary>
    /// Esegue il benchmark per lo scenario specificato
    /// </summary>
    [HttpPost("run")]
    [ProducesResponseType(typeof(List<BenchmarkResult>), 200)]
    public async Task<IActionResult> Run([FromBody] BenchmarkRequest request)
    {
        if (request.ItemCount is < 1 or > 100_000)
            return BadRequest("ItemCount deve essere tra 1 e 100.000");

        if (request.Iterations is < 1 or > 10_000)
            return BadRequest("Iterations deve essere tra 1 e 10.000");

        logger.LogInformation("Benchmark: scenario={Scenario}, items={Items}, iterations={Iterations}",
            request.Scenario, request.ItemCount, request.Iterations);

        var results = await benchmarkService.RunAsync(request);
        return Ok(results);
    }

    /// <summary>
    /// Lista degli scenari disponibili
    /// </summary>
    [HttpGet("scenarios")]
    public IActionResult GetScenarios()
    {
        var scenarios = new[]
        {
            new { id = ScenarioName.Integers, label = "Interi (int/long)", icon = "🔢", description = "Array di interi 32/64 bit" },
            new { id = ScenarioName.Decimals, label = "Decimali (decimal)", icon = "🔣", description = "Array di decimali positivi e negativi" },
            new { id = ScenarioName.Floats, label = "Float / Double", icon = "🔣", description = "Array IEEE 754 float e double" },
            new { id = ScenarioName.Strings, label = "Stringhe", icon = "📝", description = "Array di stringhe UTF-8 variabili" },
            new { id = ScenarioName.Nested, label = "Oggetti annidati", icon = "🏗️", description = "Grafi di oggetti con 5+ livelli" },
            new { id = ScenarioName.DateTime, label = "DateTime / GUID", icon = "📅", description = "Record con timestamp e identificatori unici" },
            new { id = ScenarioName.Repeated, label = "Dati ripetuti", icon = "🔁", description = "Log entries con valori enum ripetuti" },
            new { id = ScenarioName.BulkArray, label = "Array primitivi (bulk)", icon = "📦", description = "Grandi array di int, double e byte" },
        };
        return Ok(scenarios);
    }

    /// <summary>
    /// Lista dei serializzatori disponibili
    /// </summary>
    [HttpGet("serializers")]
    public IActionResult GetSerializers()
    {
        return Ok(SerialializerModel.SerialializerModels);
    }

    /// <summary>
    /// Health check
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });
}
