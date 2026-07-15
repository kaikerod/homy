# Gastos Residenciais — backend

API REST em .NET 10 para cadastro de pessoas, transações e consulta de totais
residenciais. A implementação segue a arquitetura em camadas descrita em
[`SPEC.md`](SPEC.md), usa EF Core com SQLite e aplica migrations na inicialização.

## Executar

Requisitos: .NET SDK 10.

```bash
dotnet restore
dotnet run --project src/GastosResidenciais.API
```

A documentação interativa fica disponível em `/swagger`. O banco `gastos.db` é
criado no diretório de execução da API e a origem `http://localhost:5173` está
liberada por padrão para o frontend de desenvolvimento.

## Testes

```bash
dotnet test GastosResidenciais.sln
```

Os testes unitários cobrem invariantes do domínio e validators. Os testes de
integração exercitam os seis endpoints, contratos de erro, regra de menoridade,
totais, exclusão em cascata, Swagger e persistência entre reinicializações.
