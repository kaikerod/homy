import { FormEvent, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ApiError, pessoasApi, transacoesApi } from '../api'
import { EmptyState, ErrorState, Field, formatCurrency, PageHeader, Spinner, Toast } from '../components'
import type { TipoTransacao } from '../types'

export function TransacoesPage() {
  const queryClient = useQueryClient()
  const [descricao, setDescricao] = useState('')
  const [valor, setValor] = useState('')
  const [tipo, setTipo] = useState<TipoTransacao>('Despesa')
  const [pessoaId, setPessoaId] = useState('')
  const [formError, setFormError] = useState<string>()
  const [toast, setToast] = useState<string>()
  const pessoas = useQuery({ queryKey: ['pessoas'], queryFn: pessoasApi.list })
  const transacoes = useQuery({ queryKey: ['transacoes'], queryFn: transacoesApi.list })
  const selectedPerson = pessoas.data?.find((person) => person.id === pessoaId)
  const isMinor = selectedPerson !== undefined && selectedPerson.idade < 18

  const create = useMutation({
    mutationFn: transacoesApi.create,
    onSuccess: async () => {
      setDescricao('')
      setValor('')
      setFormError(undefined)
      setToast('Transação registrada com sucesso.')
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['transacoes'] }),
        queryClient.invalidateQueries({ queryKey: ['totais'] }),
      ])
    },
    onError: (error: ApiError) => setFormError(error.details[0] ?? error.message),
  })

  function submit(event: FormEvent) {
    event.preventDefault()
    const parsedValue = Number(valor.replace(',', '.'))
    if (!descricao.trim()) return setFormError('Informe uma descrição.')
    if (!Number.isFinite(parsedValue) || parsedValue <= 0) return setFormError('Informe um valor maior que zero.')
    if (!pessoaId) return setFormError('Selecione uma pessoa.')
    create.mutate({ descricao: descricao.trim(), valor: parsedValue, tipo, pessoaId })
  }

  return (
    <>
      <PageHeader eyebrow="Movimentações" title="Transações">Registre o que entra e o que sai, sem complicação.</PageHeader>
      <div className="page-grid">
        <section className="card form-card">
          <div className="card__heading"><span className="step">02</span><div><h2>Nova transação</h2><p>Registre uma movimentação</p></div></div>
          {pessoas.isError ? <ErrorState message={(pessoas.error as ApiError).message} retry={() => pessoas.refetch()} /> : (
            <form onSubmit={submit} noValidate>
              <Field label="Descrição">
                <input value={descricao} onChange={(event) => setDescricao(event.target.value)} maxLength={200} placeholder="Ex.: Conta de energia" />
              </Field>
              <div className="field-row">
                <Field label="Valor (R$)">
                  <input value={valor} onChange={(event) => setValor(event.target.value)} inputMode="decimal" placeholder="0,00" />
                </Field>
                <Field label="Tipo" hint={isMinor ? 'Menores só podem registrar despesas.' : undefined}>
                  <select value={tipo} onChange={(event) => setTipo(event.target.value as TipoTransacao)}>
                    <option value="Despesa">Despesa</option>
                    <option value="Receita" disabled={isMinor}>Receita</option>
                  </select>
                </Field>
              </div>
              <Field label="Pessoa">
                <select value={pessoaId} onChange={(event) => {
                  const nextPerson = pessoas.data?.find((person) => person.id === event.target.value)
                  setPessoaId(event.target.value)
                  if (nextPerson && nextPerson.idade < 18) setTipo('Despesa')
                }} disabled={pessoas.isLoading || !pessoas.data?.length}>
                  <option value="">{pessoas.isLoading ? 'Carregando…' : 'Selecione uma pessoa'}</option>
                  {pessoas.data?.map((pessoa) => <option key={pessoa.id} value={pessoa.id}>{pessoa.nome} · {pessoa.idade} anos</option>)}
                </select>
              </Field>
              {!pessoas.isLoading && pessoas.data?.length === 0 && <p className="inline-note">Cadastre uma pessoa antes de registrar transações.</p>}
              {formError && <ErrorState message={formError} />}
              <button className="button button--primary button--full" disabled={create.isPending || !pessoas.data?.length}>
                {create.isPending ? 'Registrando…' : 'Registrar transação'}
              </button>
            </form>
          )}
        </section>

        <section className="card list-card">
          <div className="card__heading"><div><h2>Últimas transações</h2><p>{transacoes.data?.length ?? 0} movimentações</p></div></div>
          {transacoes.isLoading ? <Spinner label="Buscando transações…" /> : transacoes.isError ? (
            <ErrorState message={(transacoes.error as ApiError).message} retry={() => transacoes.refetch()} />
          ) : transacoes.data?.length ? (
            <div className="transaction-list">
              {transacoes.data.map((transaction) => (
                <article className="transaction-row" key={transaction.id}>
                  <span className={`type-icon type-icon--${transaction.tipo.toLowerCase()}`}>{transaction.tipo === 'Receita' ? '↗' : '↘'}</span>
                  <div><strong>{transaction.descricao}</strong><small>{transaction.pessoaNome}</small></div>
                  <div className={`money money--${transaction.tipo.toLowerCase()}`}>
                    {transaction.tipo === 'Receita' ? '+' : '−'} {formatCurrency(transaction.valor)}
                    <small>{transaction.tipo}</small>
                  </div>
                </article>
              ))}
            </div>
          ) : <EmptyState title="Nenhuma movimentação">As receitas e despesas registradas aparecerão aqui.</EmptyState>}
        </section>
      </div>
      {toast && <Toast message={toast} onClose={() => setToast(undefined)} />}
    </>
  )
}
