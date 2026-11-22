using ExoApi.Contracts;
using ExoApi.Data;
using ExoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ExoApi.Services;

public sealed class ProjetoService(ExoApiContext db) : IProjetoService
{
    public async Task<IReadOnlyList<ProjetoResponse>> ListarAsync(CancellationToken ct) =>
        await db.Projetos.AsNoTracking().OrderBy(p => p.Id).Select(p => ToResponse(p)).ToListAsync(ct);

    public async Task<ProjetoResponse> ObterAsync(int id, CancellationToken ct)
    {
        var entity = await db.Projetos.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new ProjetoNaoEncontradoException(id);
        return ToResponse(entity);
    }

    public async Task<ProjetoResponse> CriarAsync(ProjetoRequest request, CancellationToken ct)
    {
        var nome = request.Nome.Trim();
        if (await db.Projetos.AnyAsync(p => p.Nome == nome, ct)) throw new ProjetoDuplicadoException(nome);
        var now = DateTime.UtcNow;
        var entity = new Projeto
        {
            Nome = nome,
            Descricao = request.Descricao?.Trim() ?? string.Empty,
            Status = request.Status.Trim(),
            CriadoEmUtc = now,
            AtualizadoEmUtc = now
        };
        db.Projetos.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public async Task<ProjetoResponse> AtualizarAsync(int id, ProjetoRequest request, CancellationToken ct)
    {
        var entity = await db.Projetos.SingleOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new ProjetoNaoEncontradoException(id);
        var nome = request.Nome.Trim();
        if (await db.Projetos.AnyAsync(p => p.Id != id && p.Nome == nome, ct)) throw new ProjetoDuplicadoException(nome);
        entity.Nome = nome;
        entity.Descricao = request.Descricao?.Trim() ?? string.Empty;
        entity.Status = request.Status.Trim();
        entity.AtualizadoEmUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ToResponse(entity);
    }

    public async Task ExcluirAsync(int id, CancellationToken ct)
    {
        var entity = await db.Projetos.SingleOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new ProjetoNaoEncontradoException(id);
        db.Projetos.Remove(entity);
        await db.SaveChangesAsync(ct);
    }

    private static ProjetoResponse ToResponse(Projeto p) =>
        new(p.Id, p.Nome, p.Descricao, p.Status, p.CriadoEmUtc, p.AtualizadoEmUtc);
}
