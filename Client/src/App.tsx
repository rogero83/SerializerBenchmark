import { useState } from 'react'
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip,
  ResponsiveContainer, RadarChart, PolarGrid, PolarAngleAxis,
  Radar, Legend, LineChart, Line, PolarRadiusAxis
} from 'recharts'
import { useBenchmark } from './hooks/useBenchmark'
import type { BenchmarkRequest, MetricKey } from './types'

const FALLBACK_COLORS: Record<string, string> = {
  SystemTextJson: '#f97316',
  Protobuf: '#3b82f6',
  MessagePack: '#a855f7',
  MemoryPack: '#22c55e',
}

const METRIC_LABELS: Record<MetricKey, { label: string; unit: string; short: string }> = {
  sizeBytes: { label: 'Dimensione', unit: 'B', short: 'Bytes' },
  serializeMs: { label: 'Serializzazione', unit: 'ms', short: 'Ser (ms)' },
  deserializeMs: { label: 'Deserializzazione', unit: 'ms', short: 'Deser (ms)' },
}

const CustomTooltip = ({ active, payload, label }: any) => {
  if (!active || !payload?.length) return null
  return (
    <div style={{ background: '#0f172a', border: '1px solid #1e293b', borderRadius: 8, padding: '12px 16px', fontSize: 13, boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.5)' }}>
      <p style={{ color: '#e2e8f0', fontWeight: 700, marginBottom: 8, borderBottom: '1px solid #1e293b', paddingBottom: 6 }}>{label}</p>
      {payload.map((p: any) => (
        <div key={p.name} style={{ display: 'flex', justifyContent: 'space-between', gap: 16, margin: '4px 0' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
            <span style={{ width: 8, height: 8, borderRadius: '50%', background: p.fill || p.stroke || p.color, display: 'inline-block' }} />
            <span style={{ color: '#cbd5e1' }}>{p.name}</span>
          </div>
          <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end' }}>
            <strong style={{ color: p.fill || p.stroke || p.color }}>
              {typeof p.value === 'number' ? p.value.toLocaleString(undefined, { maximumFractionDigits: 3 }) : p.value}
              {p.unit ? ` ${p.unit}` : ''}
            </strong>
            {p.payload?.originalValue !== undefined && (
              <span style={{ fontSize: 10, color: '#475569' }}>Real: {p.payload.originalValue.toLocaleString()} {p.payload.unit}</span>
            )}
          </div>
        </div>
      ))}
    </div>
  )
}

export default function App() {
  const { results, history, loading, error, scenarios, serializers, serverOnline, run, checkHealth } = useBenchmark()

  const [scenario, setScenario] = useState('integers')
  const [itemCount, setItemCount] = useState(1000)
  const [iterations, setIterations] = useState(200)
  const [selectedSerializers, setSelectedSerializers] = useState<string[]>(['SystemTextJson', 'Protobuf', 'MessagePack', 'MemoryPack'])
  const [metric, setMetric] = useState<MetricKey>('sizeBytes')
  const [activeTab, setActiveTab] = useState<'bar' | 'radar' | 'history'>('bar')
  const [hiddenLines, setHiddenLines] = useState<Record<string, boolean>>({})

  const colors = Object.fromEntries(
    serializers.map(s => [s.id, s.color])
  )
  const getColor = (id: string) => colors[id] ?? FALLBACK_COLORS[id] ?? '#64748b'

  const toggleSerializer = (id: string) => {
    setSelectedSerializers(prev =>
      prev.includes(id) ? prev.filter(s => s !== id) : [...prev, id]
    )
  }

  const handleLegendClick = (e: any) => {
    const dataKey = e.dataKey;
    if (dataKey) {
      setHiddenLines(prev => ({ ...prev, [dataKey]: !prev[dataKey] }));
    }
  }

  const handleRun = () => {
    const req: BenchmarkRequest = {
      scenario,
      itemCount,
      iterations,
      serializers: selectedSerializers,
    }
    run(req)
  }

  // Chart data
  const chartData = results
    .filter(r => !r.error)
    .sort((a, b) => a[metric] - b[metric])
    .map(r => ({
      name: serializers.find(s => s.id === r.serializer)?.label ?? r.serializer,
      id: r.serializer,
      value: r[metric],
      fill: getColor(r.serializer),
    }))

  const radarData = (['sizeBytes', 'serializeMs', 'deserializeMs'] as MetricKey[]).map(m => {
    const vals = results.filter(r => !r.error).map(r => r[m])
    const min = Math.min(...vals)
    const row: Record<string, any> = { 
      metric: METRIC_LABELS[m].short,
      fullMetric: METRIC_LABELS[m].label,
      unit: METRIC_LABELS[m].unit 
    }
    results.filter(r => !r.error).forEach(r => {
      const label = serializers.find(s => s.id === r.serializer)?.label ?? r.serializer
      // Score: 100 is best (lowest value)
      // If current value is 0, we avoid division by zero
      const score = r[m] === 0 ? 100 : (min / r[m]) * 100
      row[label] = Math.round(score)
      row[`${label}_val`] = r[m] // Store real value for tooltip
    })
    return row
  })

  // History line chart data
  const historyChartData = history.slice().reverse().map((h, i) => {
    const row: Record<string, any> = { run: `#${i + 1} ${h.request.scenario}` }
    h.results.filter(r => !r.error).forEach(r => {
      const label = serializers.find(s => s.id === r.serializer)?.label ?? r.serializer
      row[label] = r[metric]
    })
    return row
  })

  const allSerializerLabels = Array.from(new Set(
    history.flatMap(h => h.results.filter(r => !r.error).map(r =>
      serializers.find(s => s.id === r.serializer)?.label ?? r.serializer
    ))
  ))

  const averages: Record<string, number> = {}
  allSerializerLabels.forEach(label => {
    let sum = 0
    let count = 0
    historyChartData.forEach(row => {
      if (row[label] !== undefined) {
        sum += row[label]
        count++
      }
    })
    if (count > 0) averages[label] = sum / count
  })

  // Appendiamo in tutte le righe le medie così Rechart disegna una linea orizzontale
  historyChartData.forEach(row => {
    allSerializerLabels.forEach(label => {
      if (averages[label] !== undefined) {
        row[`Media ${label}`] = averages[label]
      }
    })
  })

  // Averages array calculation finished
  const S = {
    page: {
      minHeight: '100vh',
      background: '#020817',
      color: '#e2e8f0',
      fontFamily: "'IBM Plex Mono', 'Courier New', monospace",
      fontSize: 14,
    } as React.CSSProperties,
    header: {
      borderBottom: '1px solid #1e293b',
      padding: '28px 40px 20px',
      background: 'linear-gradient(135deg, #0f172a 0%, #0c1628 100%)',
    } as React.CSSProperties,
    badge: (online: boolean | null) => ({
      display: 'inline-flex', alignItems: 'center', gap: 6,
      padding: '3px 10px', borderRadius: 20, fontSize: 11,
      background: online === true ? '#22c55e22' : online === false ? '#ef444422' : '#64748b22',
      border: `1px solid ${online === true ? '#22c55e44' : online === false ? '#ef444444' : '#64748b44'}`,
      color: online === true ? '#22c55e' : online === false ? '#ef4444' : '#64748b',
    } as React.CSSProperties),
    card: {
      background: '#0f172a',
      border: '1px solid #1e293b',
      borderRadius: 12,
      padding: 20,
    } as React.CSSProperties,
    label: {
      color: '#64748b', fontSize: 11, letterSpacing: 1.5,
      textTransform: 'uppercase' as const, display: 'block', marginBottom: 8,
    },
    input: {
      background: '#020817', border: '1px solid #1e293b',
      color: '#e2e8f0', padding: '8px 12px', borderRadius: 6,
      fontFamily: 'inherit', fontSize: 13, width: '100%',
    } as React.CSSProperties,
    btn: (active?: boolean, color?: string) => ({
      padding: '8px 16px', borderRadius: 6, cursor: 'pointer',
      border: active ? `1px solid ${color ?? '#3b82f6'}` : '1px solid #1e293b',
      background: active ? `${color ?? '#3b82f6'}22` : '#0f172a',
      color: active ? (color ?? '#93c5fd') : '#64748b',
      fontFamily: 'inherit', fontSize: 12, transition: 'all 0.15s',
    } as React.CSSProperties),
    runBtn: {
      padding: '12px 32px', borderRadius: 8, cursor: loading ? 'not-allowed' : 'pointer',
      border: 'none', background: loading ? '#1e293b' : 'linear-gradient(135deg, #22c55e, #16a34a)',
      color: loading ? '#64748b' : '#020817', fontFamily: 'inherit',
      fontSize: 14, fontWeight: 700, width: '100%', transition: 'all 0.2s',
    } as React.CSSProperties,
  }

  return (
    <div style={S.page}>
      {/* HEADER */}
      <div style={S.header}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'wrap', gap: 12 }}>
            <div>
              <h1 style={{ fontSize: 'clamp(18px, 3vw, 28px)', fontWeight: 700, color: '#f1f5f9', letterSpacing: -0.5, marginBottom: 4 }}>
                🔬 Serializer Benchmark
              </h1>
              <p style={{ color: '#64748b', fontSize: 12 }}>
                System.Text.Json · protobuf-net · MessagePack · MemoryPack · .NET 10
              </p>
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
              <div style={S.badge(serverOnline)}>
                <span style={{ width: 6, height: 6, borderRadius: '50%', background: 'currentColor', display: 'inline-block' }} />
                {serverOnline === true ? 'Server Online' : serverOnline === false ? 'Server Offline' : 'Connessione...'}
              </div>
              {serverOnline === false && (
                <button onClick={checkHealth} style={{ ...S.btn(), fontSize: 11 }}>↻ Riprova</button>
              )}
            </div>
          </div>
        </div>
      </div>

      <div style={{ maxWidth: 1200, margin: '0 auto', padding: '24px 24px 60px' }}>

        {/* Server offline notice */}
        {serverOnline === false && (
          <div style={{
            background: '#ef444411', border: '1px solid #ef444433',
            borderRadius: 10, padding: '16px 20px', marginBottom: 24,
            color: '#fca5a5', fontSize: 13, lineHeight: 1.7,
          }}>
            <strong>⚠️ Server non raggiungibile.</strong> Avvia il server ASP.NET Core prima di eseguire i benchmark:
            <br /><code style={{ color: '#f87171', background: '#00000033', padding: '2px 8px', borderRadius: 4 }}>
              cd Server && dotnet run
            </code>
          </div>
        )}

        <div style={{ display: 'grid', gridTemplateColumns: '320px 1fr', gap: 24, alignItems: 'start' }}>

          {/* ── LEFT PANEL: Controls ── */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>

            {/* Scenario */}
            <div style={S.card}>
              <span style={S.label}>📋 Scenario</span>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
                {(scenarios.length ? scenarios : [
                  { id: 'integers', label: 'Interi (int/long)', icon: '🔢', description: '' },
                  { id: 'floats', label: 'Float / Double', icon: '🔣', description: '' },
                  { id: 'strings', label: 'Stringhe', icon: '📝', description: '' },
                  { id: 'nested', label: 'Oggetti annidati', icon: '🏗️', description: '' },
                  { id: 'datetime', label: 'DateTime / GUID', icon: '📅', description: '' },
                  { id: 'repeated', label: 'Dati ripetuti', icon: '🔁', description: '' },
                  { id: 'bulkarray', label: 'Array primitivi (bulk)', icon: '📦', description: '' },
                ]).map(s => (
                  <button key={s.id} onClick={() => setScenario(s.id)} style={S.btn(scenario === s.id)}>
                    {s.icon} {s.label}
                  </button>
                ))}
              </div>
            </div>

            {/* Serializers */}
            <div style={S.card}>
              <span style={S.label}>⚙️ Serializzatori</span>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
                {(['SystemTextJson', 'Protobuf', 'MessagePack', 'MemoryPack'] as const).map(id => {
                  const info = serializers.find(s => s.id === id)
                  const color = getColor(id)
                  const active = selectedSerializers.includes(id)
                  return (
                    <button key={id} onClick={() => toggleSerializer(id)}
                      style={{ ...S.btn(active, color), display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <span style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                        <span style={{ width: 8, height: 8, borderRadius: '50%', background: active ? color : '#374151', display: 'inline-block' }} />
                        {info?.label ?? id}
                      </span>
                      {info && <span style={{ fontSize: 10, color: '#475569' }}>v{info.version}</span>}
                    </button>
                  )
                })}
              </div>
            </div>

            {/* Parameters */}
            <div style={S.card}>
              <span style={S.label}>🎛️ Parametri</span>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
                <div>
                  <label style={{ ...S.label, marginBottom: 4 }}>Numero elementi: <strong style={{ color: '#e2e8f0' }}>{itemCount.toLocaleString()}</strong></label>
                  <input type="range" min={100} max={10000} step={100} value={itemCount}
                    onChange={e => setItemCount(Number(e.target.value))}
                    style={{ width: '100%', accentColor: '#22c55e' }} />
                  <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 10, color: '#475569', marginTop: 2 }}>
                    <span>100</span><span>10.000</span>
                  </div>
                </div>
                <div>
                  <label style={{ ...S.label, marginBottom: 4 }}>Iterazioni: <strong style={{ color: '#e2e8f0' }}>{iterations}</strong></label>
                  <input type="range" min={10} max={1000} step={10} value={iterations}
                    onChange={e => setIterations(Number(e.target.value))}
                    style={{ width: '100%', accentColor: '#22c55e' }} />
                  <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 10, color: '#475569', marginTop: 2 }}>
                    <span>10</span><span>1.000</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Run button */}
            <button onClick={handleRun} disabled={loading || selectedSerializers.length === 0} style={S.runBtn}>
              {loading ? '⏳ Esecuzione...' : '▶ Esegui Benchmark'}
            </button>

            {error && (
              <div style={{ background: '#ef444411', border: '1px solid #ef444433', borderRadius: 8, padding: '10px 14px', color: '#fca5a5', fontSize: 12 }}>
                ❌ {error}
              </div>
            )}
          </div>

          {/* ── RIGHT PANEL: Results ── */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>

            {/* Metric selector */}
            <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
              {(Object.keys(METRIC_LABELS) as MetricKey[]).map(k => (
                <button key={k} onClick={() => setMetric(k)}
                  style={{
                    ...S.btn(metric === k, '#22c55e'),
                    borderRadius: 20, padding: '6px 16px',
                    background: metric === k ? '#22c55e' : '#0f172a',
                    color: metric === k ? '#020817' : '#64748b',
                    fontWeight: metric === k ? 700 : 400,
                  }}>
                  {METRIC_LABELS[k].short}
                </button>
              ))}
              {/* Tab selector */}
              <div style={{ marginLeft: 'auto', display: 'flex', gap: 6 }}>
                {(['bar', 'radar', 'history'] as const).map(t => (
                  <button key={t} onClick={() => setActiveTab(t)}
                    style={{ ...S.btn(activeTab === t), textTransform: 'capitalize' }}>
                    {t === 'bar' ? '📊 Bar' : t === 'radar' ? '🕸️ Radar' : '📈 Storia'}
                  </button>
                ))}
              </div>
            </div>

            {/* Results cards */}
            {results.length > 0 && (
              <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))', gap: 12 }}>
                {results
                  .filter(r => !r.error)
                  .sort((a, b) => a[metric] - b[metric])
                  .map((r, i) => {
                    const color = getColor(r.serializer)
                    const label = serializers.find(s => s.id === r.serializer)?.label ?? r.serializer
                    const rankEmoji = ['🥇', '🥈', '🥉', '🔴'][Math.min(i, 3)]
                    return (
                      <div key={r.serializer} style={{
                        background: '#0f172a',
                        border: `1px solid ${color}33`,
                        borderTop: `3px solid ${color}`,
                        borderRadius: 10, padding: '14px 16px',
                      }}>
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 }}>
                          <span style={{ color, fontSize: 12, fontWeight: 700 }}>{label}</span>
                          <span>{rankEmoji}</span>
                        </div>
                        <div style={{ fontSize: 22, fontWeight: 700, color: '#f1f5f9', marginBottom: 4 }}>
                          {r[metric].toLocaleString()}
                          <span style={{ fontSize: 12, color: '#64748b', marginLeft: 4 }}>{METRIC_LABELS[metric].unit}</span>
                        </div>
                        <div style={{ fontSize: 11, color: '#475569' }}>
                          📦 {r.sizeBytes.toLocaleString()} B<br />
                          ⬆ {r.serializeMs} ms &nbsp;⬇ {r.deserializeMs} ms
                        </div>
                      </div>
                    )
                  })}
              </div>
            )}

            {/* Charts */}
            {results.length > 0 && activeTab === 'bar' && (
              <div style={S.card}>
                <p style={{ ...S.label, marginBottom: 16 }}>📊 {METRIC_LABELS[metric].label} — basso = meglio</p>
                <ResponsiveContainer width="100%" height={340}>
                  <BarChart data={chartData} margin={{ left: 0, right: 30, bottom: 20, top: 20 }} layout="vertical">
                    <CartesianGrid strokeDasharray="3 3" stroke="#1e293b" horizontal={false} />
                    <XAxis type="number" tick={{ fill: '#94a3b8', fontSize: 11, fontFamily: 'IBM Plex Mono' }} domain={[0, 'auto']} />
                    <YAxis dataKey="name" type="category" width={110} tick={{ fill: '#e2e8f0', fontSize: 12, fontWeight: 600 }} />
                    <Tooltip content={<CustomTooltip />} cursor={{ fill: '#ffffff06' }} />
                    <Bar dataKey="value" radius={[0, 6, 6, 0]} barSize={32} label={{ position: 'right', fill: '#94a3b8', fontSize: 11, formatter: (val: number) => val.toLocaleString(undefined, { maximumFractionDigits: 2 }) }}>
                      {chartData.map(entry => (
                        <rect key={entry.name} fill={entry.fill} />
                      ))}
                    </Bar>
                  </BarChart>
                </ResponsiveContainer>
              </div>
            )}

            {results.length > 0 && activeTab === 'radar' && (
              <div style={S.card}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 16 }}>
                  <div>
                    <p style={{ ...S.label, marginBottom: 4 }}>🕸️ Score Comparativo (100 = Migliore)</p>
                    <p style={{ color: '#475569', fontSize: 11 }}>Normalizzato: 100 rappresenta la migliore performance registrata per quella metrica</p>
                  </div>
                </div>
                <ResponsiveContainer width="100%" height={400}>
                  <RadarChart data={radarData} margin={{ top: 20, right: 30, bottom: 20, left: 30 }}>
                    <PolarGrid stroke="#1e293b" />
                    <PolarAngleAxis dataKey="metric" tick={{ fill: '#94a3b8', fontSize: 12, fontWeight: 600, fontFamily: 'IBM Plex Mono' }} />
                    <PolarRadiusAxis angle={30} domain={[0, 100]} tick={{ fill: '#475569', fontSize: 10 }} axisLine={false} tickCount={6} />
                    {results.filter(r => !r.error).map(r => {
                      const label = serializers.find(s => s.id === r.serializer)?.label ?? r.serializer
                      return (
                        <Radar
                          key={r.serializer}
                          name={label}
                          dataKey={label}
                          stroke={getColor(r.serializer)}
                          fill={getColor(r.serializer)}
                          fillOpacity={0.15}
                          strokeWidth={3}
                          hide={hiddenLines[label]}
                          dot={{ r: 4, fillOpacity: 1 }}
                          activeDot={{ r: 6 }}
                        />
                      )
                    })}
                    <Tooltip 
                      content={<CustomTooltip />}
                      formatter={(value: any, name: string, props: any) => {
                        return [value, name, { originalValue: props.payload[`${name}_val`], unit: props.payload.unit }]
                      }}
                    />
                    <Legend 
                      onClick={handleLegendClick}
                      wrapperStyle={{ cursor: 'pointer', paddingTop: 20 }}
                      formatter={(v) => (
                        <span style={{ 
                          color: hiddenLines[v] ? '#475569' : '#94a3b8', 
                          fontSize: 11, 
                          fontFamily: 'IBM Plex Mono',
                          textDecoration: hiddenLines[v] ? 'line-through' : 'none',
                          transition: 'all 0.2s'
                        }}>
                          {v}
                        </span>
                      )}
                    />
                  </RadarChart>
                </ResponsiveContainer>
              </div>
            )}

            {activeTab === 'history' && (
              <div style={S.card}>
                <p style={{ ...S.label, marginBottom: 16 }}>📈 Storico run — {METRIC_LABELS[metric].label}</p>
                {historyChartData.length === 0 ? (
                  <p style={{ color: '#475569', textAlign: 'center', padding: '40px 0' }}>Nessun benchmark eseguito ancora.</p>
                ) : (
                  <ResponsiveContainer width="100%" height={300}>
                    <LineChart data={historyChartData}>
                      <CartesianGrid strokeDasharray="3 3" stroke="#1e293b" />
                      <XAxis dataKey="run" tick={{ fill: '#475569', fontSize: 10, fontFamily: 'IBM Plex Mono' }} />
                      <YAxis tick={{ fill: '#475569', fontSize: 10, fontFamily: 'IBM Plex Mono' }} />
                      <Tooltip content={<CustomTooltip />} />
                      {allSerializerLabels.map(label => {
                        const ser = serializers.find(s => s.label === label)
                        const color = ser ? getColor(ser.id) : '#64748b'
                        return (
                          <Line key={label} type="monotone" dataKey={label} hide={hiddenLines[label]}
                            stroke={color} strokeWidth={2} dot={{ fill: color, r: 4 }} name={label} />
                        )
                      })}
                      {allSerializerLabels.map(label => {
                        const ser = serializers.find(s => s.label === label)
                        const color = ser ? getColor(ser.id) : '#64748b'
                        const mediaLabel = `Media ${label}`
                        return (
                          <Line key={mediaLabel} type="step" dataKey={mediaLabel} hide={hiddenLines[mediaLabel]}
                            stroke={color} strokeDasharray="3 3" strokeOpacity={0.5} strokeWidth={1} dot={false} name={mediaLabel} />
                        )
                      })}
                      <Legend onClick={handleLegendClick} wrapperStyle={{ cursor: 'pointer' }}
                        formatter={(v) => <span style={{ color: hiddenLines[v] ? '#475569' : '#94a3b8', fontSize: 11, fontFamily: 'IBM Plex Mono', textDecoration: hiddenLines[v] ? 'line-through' : 'none', transition: 'all 0.2s' }}>{v}</span>} />
                    </LineChart>
                  </ResponsiveContainer>
                )}
              </div>
            )}

            {/* Detail table */}
            {results.length > 0 && (
              <div style={S.card}>
                <p style={{ ...S.label, marginBottom: 16 }}>📋 Dettaglio completo</p>
                <div style={{ overflowX: 'auto' }}>
                  <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 12 }}>
                    <thead>
                      <tr style={{ borderBottom: '1px solid #1e293b' }}>
                        {['Rank', 'Serializer', 'Dimensione (B)', 'Ser (ms)', 'Deser (ms)', 'Iter.', 'Stato'].map(h => (
                          <th key={h} style={{ padding: '8px 12px', color: '#475569', fontWeight: 400, textAlign: 'left', whiteSpace: 'nowrap' }}>{h}</th>
                        ))}
                      </tr>
                    </thead>
                    <tbody>
                      {results
                        .sort((a, b) => {
                          if (a.error) return 1
                          if (b.error) return -1
                          return (a.sizeBytes + a.serializeMs * 100 + a.deserializeMs * 100) -
                            (b.sizeBytes + b.serializeMs * 100 + b.deserializeMs * 100)
                        })
                        .map((r, i) => {
                          const color = getColor(r.serializer)
                          const label = serializers.find(s => s.id === r.serializer)?.label ?? r.serializer
                          const rankEmoji = r.error ? '❌' : ['🥇', '🥈', '🥉', '🔴'][Math.min(i, 3)]
                          return (
                            <tr key={r.serializer} style={{ borderBottom: '1px solid #0f172a', background: i === 0 && !r.error ? '#22c55e08' : 'transparent' }}>
                              <td style={{ padding: '10px 12px', fontSize: 16 }}>{rankEmoji}</td>
                              <td style={{ padding: '10px 12px' }}>
                                <span style={{ color, fontWeight: 700, display: 'flex', alignItems: 'center', gap: 6 }}>
                                  <span style={{ width: 8, height: 8, borderRadius: '50%', background: color, display: 'inline-block' }} />
                                  {label}
                                </span>
                              </td>
                              <td style={{ padding: '10px 12px', color: '#e2e8f0' }}>{r.error ? '—' : r.sizeBytes.toLocaleString()}</td>
                              <td style={{ padding: '10px 12px', color: '#e2e8f0' }}>{r.error ? '—' : r.serializeMs}</td>
                              <td style={{ padding: '10px 12px', color: '#e2e8f0' }}>{r.error ? '—' : r.deserializeMs}</td>
                              <td style={{ padding: '10px 12px', color: '#64748b' }}>{r.error ? '—' : r.iterations}</td>
                              <td style={{ padding: '10px 12px', color: r.error ? '#ef4444' : '#22c55e', fontSize: 11 }}>
                                {r.error ? `ERR: ${r.error}` : 'OK'}
                              </td>
                            </tr>
                          )
                        })}
                    </tbody>
                  </table>
                </div>
              </div>
            )}

            {results.length === 0 && !loading && (
              <div style={{ ...S.card, textAlign: 'center', padding: '60px 20px' }}>
                <div style={{ fontSize: 48, marginBottom: 16 }}>🔬</div>
                <p style={{ color: '#64748b', fontSize: 15 }}>Configura i parametri e premi <strong style={{ color: '#22c55e' }}>Esegui Benchmark</strong></p>
                <p style={{ color: '#374151', fontSize: 12, marginTop: 8 }}>I risultati reali arriveranno dal server ASP.NET Core</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
