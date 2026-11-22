# Atividade — Respostas HTTP e tratamento de exceções

O projeto ExoApi implementa respostas padronizadas com `ProblemDetails`, incluindo `status`, `title`, `detail`, `instance` e `traceId`.

## Respostas de sucesso

| Operação | Status | Significado |
|---|---:|---|
| GET de lista ou item | 200 OK | Consulta concluída. |
| POST de projeto | 201 Created | Registro criado; o cabeçalho `Location` aponta para a consulta do item. |
| PUT de projeto | 200 OK | Registro atualizado e devolvido no corpo. |
| DELETE de projeto | 204 No Content | Registro excluído sem corpo de resposta. |

## Respostas de erro

| Situação | Status | Resposta |
|---|---:|---|
| JSON ou campos inválidos | 400 Bad Request | `ValidationProblemDetails` criado pelo [ApiController]. |
| Sem token em rota protegida | 401 Unauthorized | Autenticação requerida. |
| Usuário sem papel Admin tenta excluir | 403 Forbidden | Identidade reconhecida, mas sem autorização. |
| Projeto inexistente | 404 Not Found | ProblemDetails “Recurso não encontrado”. |
| Nome duplicado | 409 Conflict | ProblemDetails “Conflito de dados”. |
| Falha inesperada | 500 Internal Server Error | Mensagem genérica, sem detalhes internos, com `traceId`. |

## Exceções tratadas

1. `ProjetoNaoEncontradoException`: lançada quando a consulta, atualização ou exclusão recebe um id inexistente; convertida em 404.
2. `ProjetoDuplicadoException`: lançada ao tentar criar ou renomear projeto para um nome já usado; convertida em 409.
3. Exceções não previstas: registradas no log do servidor e convertidas em 500 com detalhe genérico para não revelar dados internos.

## Decisões de segurança

As rotas de projeto exigem JWT válido. A política `ProjectDelete` exige usuário autenticado com papel `Admin`. A chave JWT e as credenciais da conta de demonstração são fornecidas por variáveis de ambiente; não devem ser commitadas. A emissão de token do projeto é apenas para demonstração acadêmica local, não um provedor de identidade de produção.
