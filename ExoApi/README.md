# ExoApi — API de gerenciamento de projetos

Projeto acadêmico de API REST em ASP.NET Core 8 com EF Core/SQL Server, documentação Swagger, JWT e autorização por papel.

## Requisitos locais

- .NET 8 SDK e uma IDE compatível (Visual Studio 2022 ou VS Code). O enunciado menciona Visual Studio 2019, que não oferece suporte ao target .NET 8; caso o laboratório exija VS 2019, ajuste o target para a versão instalada e valide as dependências.
- SQL Server ou SQL Server LocalDB.

## Banco de dados

Execute `database.sql` (ou a cópia `db.sql`) no SQL Server Management Studio. A conexão padrão da aplicação é LocalDB. Para outro servidor, defina `ConnectionStrings__ExoApi` no ambiente local.

## Configuração de autenticação

Defina as variáveis abaixo no terminal antes de iniciar. Use valores locais, fortes e temporários; não os grave no Git.

```powershell
$env:Jwt__Key = 'use-uma-chave-local-com-pelo-menos-32-bytes'
$env:DemoAuth__User = 'admin-local'
$env:DemoAuth__Password = 'defina-uma-senha-local-forte'
$env:DemoAuth__Role = 'Admin'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
```

O endpoint `/api/auth/token` é um emissor didático de token com uma única conta configurada por ambiente. Não substitui um provedor de identidade real.

## Executar

Na pasta `ExoApi`:

```powershell
dotnet restore
dotnet run
```

Abra o endereço HTTPS informado pelo terminal e acrescente `/swagger`. A autenticação JWT protege as rotas de projetos; exclusão exige papel `Admin`.

## Executar os testes de integração

Na pasta `ExoApi.Tests`:

```powershell
dotnet test
```

Os testes usam EF Core InMemory para isolar os casos e um esquema de autenticação de teste. A aplicação normal usa SQL Server.

## Limite deste pacote

O pacote inclui um esquema SQL novo derivado do modelo implementado, não o anexo original `db.sql` do material didático. Compare os nomes e tipos com o arquivo disponibilizado no AVA antes de entregar, se o curso exigir aquele schema exato.
