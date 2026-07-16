# Lógica da Aplicação

O Homy controla as finanças de uma residência. A aplicação cadastra as pessoas
que fazem parte dela, registra receitas e despesas vinculadas a cada pessoa e
calcula um resumo financeiro individual e geral.

## Fluxo principal

1. Cadastre uma pessoa informando nome e idade.
2. Registre uma transação para uma pessoa existente, escolhendo entre
   `Receita` e `Despesa` e informando um valor positivo.
3. Consulte o resumo para ver receitas, despesas e saldo de cada pessoa, além
   dos totais da residência.

No frontend, esses fluxos estão separados nas telas **Pessoas**,
**Transações** e **Resumo**. As chamadas são feitas para a API em `/api/v1`.

## Regras de negócio

- Uma transação sempre pertence a uma pessoa cadastrada. Caso o identificador
  informado não exista, a API retorna `404 Not Found`.
- Pessoas menores de 18 anos podem registrar somente despesas. Uma tentativa
  de registrar uma receita retorna `400 Bad Request`.
- Nome da pessoa e descrição da transação são obrigatórios.
- A idade aceita está entre 0 e 130 anos, e o valor de toda transação deve ser
  maior que zero.
- Ao excluir uma pessoa, todas as transações dela também são excluídas. Essa
  relação é garantida pelo banco de dados com exclusão em cascata.

## Cálculo dos totais

Para cada pessoa, o sistema separa as transações por tipo e aplica:

```text
total de receitas = soma das transações Receita
total de despesas = soma das transações Despesa
saldo da pessoa   = total de receitas - total de despesas
```

O resumo geral é a soma dos totais individuais:

```text
receitas gerais = soma das receitas de todas as pessoas
despesas gerais = soma das despesas de todas as pessoas
saldo geral     = receitas gerais - despesas gerais
```

Pessoas sem transações continuam aparecendo no resumo com todos os valores em
zero.

## API e persistência

| Operação | Endpoint | Resultado |
|---|---|---|
| Criar pessoa | `POST /api/v1/pessoas` | Cria uma pessoa com identificador gerado pelo servidor. |
| Listar pessoas | `GET /api/v1/pessoas` | Retorna as pessoas cadastradas. |
| Excluir pessoa | `DELETE /api/v1/pessoas/{id}` | Remove a pessoa e suas transações. |
| Criar transação | `POST /api/v1/transacoes` | Valida e registra uma receita ou despesa. |
| Listar transações | `GET /api/v1/transacoes` | Retorna as transações com o nome da pessoa associada. |
| Consultar resumo | `GET /api/v1/totais` | Retorna totais individuais e gerais calculados. |

Os dados são persistidos em SQLite. A API organiza o processamento em camadas:
os controllers recebem HTTP, os services aplicam validações e regras, e os
repositórios acessam o banco. Erros de validação e de regra de negócio recebem
respostas JSON padronizadas, sem expor detalhes internos.
