# Documentação da API ExoApi com Swagger

## Visão geral

A ExoApi disponibiliza operações de autenticação e gerenciamento de projetos em JSON. A documentação OpenAPI/Swagger é registrada no arquivo `Program.cs` e a interface é habilitada somente quando o ambiente ASP.NET Core está em `Development`. Ao executar nesse ambiente, abra `/swagger` na URL local informada pelo perfil de execução.

## Autenticação

Solicite um JWT em `POST /api/auth/token` usando credenciais de demonstração configuradas localmente por variáveis de ambiente `DemoAuth:User` e `DemoAuth:Password`. O retorno contém `accessToken`, `tipo` e `expiraEmUtc`. No Swagger, use **Authorize** com o esquema Bearer e informe `Bearer <token>`. As rotas de projetos exigem token válido. A exclusão também exige o papel `Admin`.

## Endpoints

| Método e rota | Finalidade | Corpo de entrada | Respostas principais |
|---|---|---|---|
| POST `/api/auth/token` | Emitir token de acesso | `{ "usuario": "...", "senha": "..." }` | 200 com JWT; 401 para credenciais inválidas |
| GET `/api/projetos` | Listar projetos | Nenhum | 200 com lista; 401 sem token |
| GET `/api/projetos/{id}` | Consultar projeto por identificador | Nenhum | 200 com projeto; 401 sem token; 404 se não existir |
| POST `/api/projetos` | Cadastrar projeto | `{ "nome": "Projeto A", "descricao": "Descrição", "status": "Planejado" }` | 201 com o projeto criado e cabeçalho Location; 400 dados inválidos; 401 sem token; 409 nome duplicado |
| PUT `/api/projetos/{id}` | Atualizar projeto | Mesmo formato do POST | 200 com o projeto atualizado; 400 dados inválidos; 401 sem token; 404 inexistente; 409 nome duplicado |
| DELETE `/api/projetos/{id}` | Excluir projeto | Nenhum | 204 sem corpo; 401 sem token; 403 sem papel Admin; 404 inexistente |

## Modelo de projeto

A resposta de projeto contém `id`, `nome`, `descricao`, `status`, `criadoEmUtc` e `atualizadoEmUtc`. A solicitação de criação ou atualização exige `nome` com 2 a 120 caracteres e `status` com até 30 caracteres; `descricao` é opcional e aceita até 1000 caracteres.

## Códigos e tratamento de erros

- 400 Bad Request: validação do corpo da solicitação.
- 401 Unauthorized: token ausente, inválido ou credenciais de demonstração incorretas.
- 403 Forbidden: usuário autenticado sem o papel necessário para excluir.
- 404 Not Found: projeto não localizado.
- 409 Conflict: nome de projeto duplicado.
- 500 Internal Server Error: falha inesperada, apresentada sem detalhes internos.

Os erros seguem `ProblemDetails` ou `ValidationProblemDetails`. O middleware global inclui `traceId` para correlação com os logs do servidor.
