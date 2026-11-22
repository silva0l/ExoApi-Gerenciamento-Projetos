using ExoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExoApi.Middleware;

public sealed class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception exception)
        {
            logger.LogError(exception, "Falha ao processar {Method} {Path}", context.Request.Method, context.Request.Path);
            var (status, title) = exception switch
            {
                ProjetoNaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                ProjetoDuplicadoException => (StatusCodes.Status409Conflict, "Conflito de dados"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno")
            };
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == 500 ? "Ocorreu um erro inesperado. Tente novamente mais tarde." : exception.Message,
                Instance = context.Request.Path
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
