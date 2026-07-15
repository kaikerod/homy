export interface Pessoa {
  id: string
  nome: string
  idade: number
}

export type TipoTransacao = 'Receita' | 'Despesa'

export interface Transacao {
  id: string
  descricao: string
  valor: number
  tipo: TipoTransacao
  pessoaId: string
  pessoaNome: string
}

export interface TotalPessoa {
  pessoaId: string
  nome: string
  totalReceitas: number
  totalDespesas: number
  saldo: number
}

export interface Totais {
  pessoas: TotalPessoa[]
  totalGeralReceitas: number
  totalGeralDespesas: number
  saldoLiquidoGeral: number
}

export interface ApiErrorBody {
  message: string
  errors: string[]
  status: number
}
