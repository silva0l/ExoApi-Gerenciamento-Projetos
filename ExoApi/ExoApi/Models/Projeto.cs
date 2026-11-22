using System.ComponentModel.DataAnnotations;

namespace ExoApi.Models;

public sealed class Projeto
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Status { get; set; } = "Planejado";

    public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEmUtc { get; set; } = DateTime.UtcNow;
}
