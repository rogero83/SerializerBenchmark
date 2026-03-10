export interface BenchmarkResult {
  serializer: string
  scenario: string
  sizeBytes: number
  serializeMs: number
  deserializeMs: number
  iterations: number
  error?: string
}

export interface BenchmarkRequest {
  scenario: string
  itemCount: number
  iterations: number
  serializers: string[]
}

export interface Scenario {
  id: string
  label: string
  icon: string
  description: string
}

export interface SerializerInfo {
  id: string
  label: string
  color: string
  version: string
}

export type MetricKey = 'sizeBytes' | 'serializeMs' | 'deserializeMs'
