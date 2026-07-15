import { useQuery } from '@tanstack/react-query'
import { ApiError, totaisApi } from '../api'
import { EmptyState, ErrorState, formatCurrency, PageHeader, Spinner } from '../components'

export function TotaisPage() {
  const totals = useQuery({ queryKey: ['totais'], queryFn: totaisApi.get })

  return (
    <>
      <PageHeader eyebrow="Visão geral" title="Resumo da casa">Um retrato claro das finanças de todo mundo.</PageHeader>
      {totals.isLoading ? <div className="card"><Spinner label="Calculando o resumo…" /></div> : totals.isError ? (
        <ErrorState message={(totals.error as ApiError).message} retry={() => totals.refetch()} />
      ) : totals.data ? (
        <>
          <section className="summary-grid" aria-label="Totais gerais">
            <article className="summary-card summary-card--income"><span>Receitas</span><strong>{formatCurrency(totals.data.totalGeralReceitas)}</strong><small>total que entrou</small></article>
            <article className="summary-card summary-card--expense"><span>Despesas</span><strong>{formatCurrency(totals.data.totalGeralDespesas)}</strong><small>total que saiu</small></article>
            <article className={`summary-card summary-card--balance ${totals.data.saldoLiquidoGeral < 0 ? 'negative' : ''}`}><span>Saldo da casa</span><strong>{formatCurrency(totals.data.saldoLiquidoGeral)}</strong><small>receitas − despesas</small></article>
          </section>
          <section className="card totals-card">
            <div className="card__heading"><div><h2>Saldo por pessoa</h2><p>Como cada morador contribui para o total</p></div></div>
            {totals.data.pessoas.length ? (
              <div className="table-scroll">
                <table>
                  <thead><tr><th>Pessoa</th><th>Receitas</th><th>Despesas</th><th>Saldo</th></tr></thead>
                  <tbody>{totals.data.pessoas.map((person) => (
                    <tr key={person.pessoaId}>
                      <td><span className="avatar avatar--small">{person.nome.slice(0, 1).toUpperCase()}</span><strong>{person.nome}</strong></td>
                      <td className="money--receita">{formatCurrency(person.totalReceitas)}</td>
                      <td className="money--despesa">{formatCurrency(person.totalDespesas)}</td>
                      <td className={person.saldo < 0 ? 'money--despesa' : 'balance-positive'}><strong>{formatCurrency(person.saldo)}</strong></td>
                    </tr>
                  ))}</tbody>
                </table>
              </div>
            ) : <EmptyState title="Nada para resumir ainda">Cadastre pessoas e transações para acompanhar os totais da casa.</EmptyState>}
          </section>
        </>
      ) : null}
    </>
  )
}
