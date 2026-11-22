using System.ComponentModel.DataAnnotations;

namespace ExoApi.Contracts;

public sealed record ProjetoRequest(
    [Required, StringLength(120, MinimumLength = 2)] string Nome,
    [StringLength(1000)] string? Descricao,
    [Required, StringLength(30)] string Status);

public sealed record ProjetoResponse(
    int Id, string Nome, string Descricao, string Status, DateTime CriadoEmUtc, DateTime AtualizadoEmUtc);
