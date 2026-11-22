using ExoApi.Contracts;

namespace ExoApi.Services;

public interface IProjetoService
{
    Task<IReadOnlyList<ProjetoResponse>> ListarAsync(CancellationToken cancellationToken);
    Task<ProjetoResponse> ObterAsync(int id, CancellationToken cancellationToken);
    Task<ProjetoResponse> CriarAsync(ProjetoRequest request, CancellationToken cancellationToken);
    Task<ProjetoResponse> AtualizarAsync(int id, ProjetoRequest request, CancellationToken cancellationToken);
    Task ExcluirAsync(int id, CancellationToken cancellationToken);
}
