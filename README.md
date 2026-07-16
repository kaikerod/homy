# Homy — Gastos Residenciais

Aplicação full-stack para cadastro de pessoas, transações e consulta dos totais
de uma residência. A API usa .NET 10, EF Core e SQLite; a interface usa React,
TypeScript, Vite e TanStack Query.

Veja [a lógica da aplicação](docs/LOGICA-DA-APLICACAO.md) para entender os
fluxos, regras de negócio, cálculos e endpoints disponíveis.
Para a arquitetura e os contratos detalhados, consulte a [especificação técnica](SPEC.md).

## Executar com Docker

Requisito: Docker com o plugin Compose.

```bash
docker compose up --build
```

A aplicação fica disponível em <http://localhost:8080>. O frontend encaminha
as chamadas `/api` para a API, e o banco SQLite é mantido no volume nomeado
`gastos-data`, sobrevivendo à recriação dos containers.

Para encerrar:

```bash
docker compose down
```

Use `docker compose down -v` somente quando também quiser apagar os dados.

## Desenvolvimento local

Requisitos: .NET SDK 10 e Node.js 24.

```bash
dotnet restore
dotnet run --project src/GastosResidenciais.API
```

Em outro terminal:

```bash
cd frontend
cp .env.example .env
npm install
npm run dev
```

O frontend fica em <http://localhost:5173> e a documentação interativa da API
em <http://localhost:5000/swagger> (a porta exata da API também é exibida pelo
`dotnet run`). O banco `gastos.db` é criado no diretório de execução da API.

## Testes

```bash
dotnet test GastosResidenciais.sln
cd frontend && npm test
```

Para validar a versão de produção do frontend, execute `npm run build`. Os testes
do backend cobrem os seis endpoints, contratos de erro, regra de menoridade,
totais, exclusão em cascata, Swagger e persistência entre reinicializações.
