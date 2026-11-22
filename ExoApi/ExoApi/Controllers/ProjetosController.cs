using ExoApi.Contracts;
using ExoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExoApi.Controllers;

[ApiController]
[Route("api/projetos")]
[Authorize]
public sealed class ProjetosController(IProjetoService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjetoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjetoResponse>>> Listar(CancellationToken ct) =>
        Ok(await service.ListarAsync(ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProjetoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjetoResponse>> Obter(int id, CancellationToken ct) =>
        Ok(await service.ObterAsync(id, ct));

    [HttpPost]
    [ProducesResponseType(typeof(ProjetoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProjetoResponse>> Criar(ProjetoRequest request, CancellationToken ct)
    {
        var created = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProjetoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProjetoResponse>> Atualizar(int id, ProjetoRequest request, CancellationToken ct) =>
        Ok(await service.AtualizarAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "ProjectDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken ct)
    {
        await service.ExcluirAsync(id, ct);
        return NoContent();
    }
}
