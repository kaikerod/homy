import { FormEvent, useEffect, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { ApiError, pessoasApi } from '../api'
import { EmptyState, ErrorState, Field, PageHeader, Spinner, Toast } from '../components'
import type { Pessoa } from '../types'

export function PessoasPage() {
  const queryClient = useQueryClient()
  const [nome, setNome] = useState('')
  const [idade, setIdade] = useState('')
  const [formError, setFormError] = useState<string>()
  const [toast, setToast] = useState<string>()
  const [toDelete, setToDelete] = useState<Pessoa>()
  const pessoas = useQuery({ queryKey: ['pessoas'], queryFn: pessoasApi.list })

  useEffect(() => {
    if (!toDelete) return
    const close = (event: KeyboardEvent) => event.key === 'Escape' && setToDelete(undefined)
    window.addEventListener('keydown', close)
    return () => window.removeEventListener('keydown', close)
  }, [toDelete])

  const create = useMutation({
    mutationFn: pessoasApi.create,
    onSuccess: async () => {
      setNome('')
      setIdade('')
      setFormError(undefined)
      setToast('Pessoa adicionada com sucesso.')
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['pessoas'] }),
        queryClient.invalidateQueries({ queryKey: ['totais'] }),
      ])
    },
    onError: (error: ApiError) => setFormError(error.details[0] ?? error.message),
  })

  const remove = useMutation({
    mutationFn: pessoasApi.remove,
    onSuccess: async () => {
      setToDelete(undefined)
      setToast('Pessoa e transações vinculadas foram removidas.')
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['pessoas'] }),
        queryClient.invalidateQueries({ queryKey: ['transacoes'] }),
        queryClient.invalidateQueries({ queryKey: ['totais'] }),
      ])
    },
  })

  function submit(event: FormEvent) {
    event.preventDefault()
    const parsedAge = Number(idade)
    if (!nome.trim()) return setFormError('Informe o nome da pessoa.')
    if (idade === '' || !Number.isInteger(parsedAge) || parsedAge < 0 || parsedAge > 130) {
      return setFormError('A idade deve ser um número inteiro entre 0 e 130.')
    }
    create.mutate({ nome: nome.trim(), idade: parsedAge })
  }

  return (
    <>
      <PageHeader eyebrow="Sua casa" title="Pessoas">Cadastre quem participa das finanças e mantenha tudo em ordem.</PageHeader>
      <div className="page-grid">
        <section className="card form-card">
          <div className="card__heading"><span className="step">01</span><div><h2>Nova pessoa</h2><p>Quem vamos adicionar?</p></div></div>
          <form onSubmit={submit} noValidate>
            <Field label="Nome completo">
              <input value={nome} onChange={(event) => setNome(event.target.value)} maxLength={100} placeholder="Ex.: Marina Oliveira" autoComplete="name" />
            </Field>
            <Field label="Idade" hint="Usamos a idade para aplicar as regras de receita.">
              <input value={idade} onChange={(event) => setIdade(event.target.value)} type="number" min="0" max="130" placeholder="Ex.: 32" />
            </Field>
            {formError && <ErrorState message={formError} />}
            <button className="button button--primary button--full" disabled={create.isPending}>
              {create.isPending ? 'Adicionando…' : 'Adicionar pessoa'}
            </button>
          </form>
        </section>

        <section className="card list-card">
          <div className="card__heading card__heading--spread"><div><h2>Moradores</h2><p>{pessoas.data?.length ?? 0} cadastrados</p></div></div>
          {pessoas.isLoading ? <Spinner label="Buscando pessoas…" /> : pessoas.isError ? (
            <ErrorState message={(pessoas.error as ApiError).message} retry={() => pessoas.refetch()} />
          ) : pessoas.data?.length ? (
            <div className="people-list">
              {pessoas.data.map((pessoa) => (
                <article className="person-row" key={pessoa.id}>
                  <span className="avatar">{pessoa.nome.slice(0, 1).toUpperCase()}</span>
                  <div><strong>{pessoa.nome}</strong><small>{pessoa.idade} anos</small></div>
                  <button className="icon-button" onClick={() => setToDelete(pessoa)} aria-label={`Excluir ${pessoa.nome}`} title="Excluir pessoa">×</button>
                </article>
              ))}
            </div>
          ) : <EmptyState title="A casa ainda está vazia">Use o formulário ao lado para cadastrar a primeira pessoa.</EmptyState>}
        </section>
      </div>

      {toDelete && (
        <div className="modal-backdrop" role="presentation" onMouseDown={() => setToDelete(undefined)}>
          <div className="modal" role="dialog" aria-modal="true" aria-labelledby="delete-title" onMouseDown={(event) => event.stopPropagation()}>
            <span className="modal__warning">!</span>
            <h2 id="delete-title">Excluir {toDelete.nome}?</h2>
            <p>Todas as transações vinculadas também serão excluídas. Esta ação não pode ser desfeita.</p>
            {remove.isError && <ErrorState message={(remove.error as ApiError).message} />}
            <div className="modal__actions">
              <button className="button button--secondary" onClick={() => setToDelete(undefined)}>Cancelar</button>
              <button className="button button--danger" disabled={remove.isPending} onClick={() => remove.mutate(toDelete.id)}>
                {remove.isPending ? 'Excluindo…' : 'Sim, excluir'}
              </button>
            </div>
          </div>
        </div>
      )}
      {toast && <Toast message={toast} onClose={() => setToast(undefined)} />}
    </>
  )
}
