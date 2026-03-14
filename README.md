# Serializer Benchmark — .NET 10 + React

Progetto completo per testare in **tempo reale** le performance di:

| Serializer | Tipo | NuGet |
|---|---|---|
| `System.Text.Json` | Testo (JSON) | built-in .NET 10 |
| `protobuf-net` | Binario (Protobuf) | `protobuf-net 3.2.56` |
| `MessagePack` | Binario compatto | `MessagePack 3.1.4` |
| `MemoryPack` | Binario ultra-veloce | `MemoryPack 1.21.4` |


## 🚀 Avvio rapido

### Prerequisiti
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 18+](https://nodejs.org)
- [Aspire](https://aspire.dev/get-started/install-cli/)

### Avvia con Aspire

```bash
aspire run
```

### Avvio senza Aspire

#### 1. Avvia il Server (terminale 1)

```bash
cd Server
dotnet restore
dotnet run
```

Il server sarà disponibile su:
- **API**: htts://localhost:5000/api/benchmark
- **Swagger UI**: https://localhost:5000/swagger

#### 2. Avvia il Client (terminale 2)

```bash
cd Client
npm install
npm run dev
```

Il client React sarà disponibile su: **http://localhost:5173**

---

## Come usare l'interfaccia

1. **Seleziona uno scenario** (Interi, Decimali, Float, Stringhe, Oggetti annidati, ecc.)
2. **Scegli i serializzatori** da confrontare (puoi deselezionarne alcuni)
3. **Configura i parametri** con gli slider:
   - **Elementi**: quanti item generare per il test (100 → 10.000)
   - **Iterazioni**: quante volte ripetere l'operazione per la media (10 → 1.000)
4. Premi **▶ Esegui Benchmark**
5. Analizza i risultati con:
   - **Bar chart** — confronto diretto
   - **Radar** — score complessivo multi-metrica
   - **Storia** — andamento tra più run consecutivi

---

## Scenari disponibili

| ID | Scenario | Cosa misura |
|---|---|---|
| `integers` | Interi (int/long) | Array di int32 e int64 |
| `decimals` | Decimali (decimal) | Array di decimal |
| `floats` | Float / Double | Array di valori IEEE 754 |
| `strings` | Stringhe | Lista di stringhe UTF-8 variabili |
| `nested` | Oggetti annidati | Grafi a 5+ livelli (Person → Contact → Address) |
| `datetime` | DateTime / GUID | Record con timestamp e Guid |
| `repeated` | Dati ripetuti | Log entries con enum/valori ripetuti |
| `bulkarray` | Array primitivi bulk | Grandi array int[], double[], byte[] |

---

## API Reference

### `POST /api/benchmark/run`

```json
{
  "scenario": "integers",
  "itemCount": 1000,
  "iterations": 200,
  "serializers": ["SystemTextJson", "SystemTextJsonUtf8", "Protobuf", "MessagePack", "MemoryPack"]
}
```

**Response:**
```json
[
  {
    "serializer": "MemoryPack",
    "scenario": "integers",
    "sizeBytes": 4004,
    "serializeMs": 0.09,
    "deserializeMs": 0.07,
    "iterations": 200
  }
]
```

### `GET /api/benchmark/scenarios` — Lista scenari
### `GET /api/benchmark/serializers` — Lista serializzatori con colori
### `GET /api/benchmark/health` — Health check

---

## Note tecniche

- La misurazione usa `System.Diagnostics.Stopwatch` ad alta risoluzione
- Ogni benchmark include un **warmup** prima della misurazione reale
- I dati sono generati deterministicamente (`Random seed = 42`)
- I modelli sono annotati con tutti e quattro i sistemi di attributi:
  - `[ProtoContract]` / `[ProtoMember]` per protobuf-net
  - `[MessagePackObject]` / `[Key]` per MessagePack
  - `[MemoryPackable]` / `[MemoryPackOrder]` per MemoryPack
  - System.Text.Json funziona con le property pubbliche senza attributi, ma con source-generated.
    - Nel client ho aggiunto anche SystemTextJsonUtf8 che usa JsonSerializer.SerializeToUtf8Bytes e JsonSerializer.Deserialize(byte[]) invece di JsonSerializer.Serialize(string) e JsonSerializer.Deserialize(string).
    - Nei benchmark c'è solo SystemTextJsonUtf8.

---