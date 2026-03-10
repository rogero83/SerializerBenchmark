import { useState, useCallback, useEffect } from 'react'
import type { BenchmarkResult, BenchmarkRequest, Scenario, SerializerInfo } from '../types'

const API_BASE = '/api/benchmark'

export function useBenchmark() {
  const [results, setResults] = useState<BenchmarkResult[]>([])
  const [history, setHistory] = useState<{ request: BenchmarkRequest; results: BenchmarkResult[]; ts: Date }[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [scenarios, setScenarios] = useState<Scenario[]>([])
  const [serializers, setSerializers] = useState<SerializerInfo[]>([])
  const [serverOnline, setServerOnline] = useState<boolean | null>(null)

  // Check server health
  const checkHealth = useCallback(async () => {
    try {
      const res = await fetch(`${API_BASE}/health`, { signal: AbortSignal.timeout(3000) })
      setServerOnline(res.ok)
    } catch {
      setServerOnline(false)
    }
  }, [])

  // Load metadata
  useEffect(() => {
    checkHealth()
    fetch(`${API_BASE}/scenarios`)
      .then(r => r.json())
      .then(setScenarios)
      .catch(() => {})

    fetch(`${API_BASE}/serializers`)
      .then(r => r.json())
      .then(setSerializers)
      .catch(() => {})
  }, [checkHealth])

  const run = useCallback(async (request: BenchmarkRequest) => {
    setLoading(true)
    setError(null)
    try {
      const res = await fetch(`${API_BASE}/run`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(request),
        signal: AbortSignal.timeout(60000),
      })
      if (!res.ok) {
        const text = await res.text()
        throw new Error(`Server error ${res.status}: ${text}`)
      }
      const data: BenchmarkResult[] = await res.json()
      setResults(data)
      setHistory(prev => [{ request, results: data, ts: new Date() }, ...prev.slice(0, 9)])
      return data
    } catch (err: any) {
      const msg = err.message ?? 'Errore sconosciuto'
      setError(msg)
      setServerOnline(false)
      return null
    } finally {
      setLoading(false)
    }
  }, [])

  return { results, history, loading, error, scenarios, serializers, serverOnline, run, checkHealth }
}
