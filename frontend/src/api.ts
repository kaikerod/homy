import axios from 'axios'
import type { ApiErrorBody, Pessoa, TipoTransacao, Totais, Transacao } from './types'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? '/api/v1',
  headers: { 'Content-Type': 'application/json' },
  timeout: 10_000,
})

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly details: string[] = [],
    public readonly status?: number,
  ) {
    super(message)
  }
}

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (!axios.isAxiosError<ApiErrorBody>(error) || !error.response) {
      return Promise.reject(new ApiError('Não foi possível conectar ao servidor. Tente novamente.'))
    }

    const body = error.response.data
    return Promise.reject(
      new ApiError(
        body?.message || 'Algo deu errado. Tente novamente.',
        body?.errors ?? [],
        error.response.status,
      ),
    )
  },
)

export const pessoasApi = {
  list: async () => (await api.get<Pessoa[]>('/pessoas')).data,
  create: async (input: { nome: string; idade: number }) =>
    (await api.post<Pessoa>('/pessoas', input)).data,
  remove: async (id: string) => api.delete(`/pessoas/${id}`),
}

export const transacoesApi = {
  list: async () => (await api.get<Transacao[]>('/transacoes')).data,
  create: async (input: {
    descricao: string
    valor: number
    tipo: TipoTransacao
    pessoaId: string
  }) => (await api.post<Transacao>('/transacoes', input)).data,
}

export const totaisApi = {
  get: async () => (await api.get<Totais>('/totais')).data,
}
