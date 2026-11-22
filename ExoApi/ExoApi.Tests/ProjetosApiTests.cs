using System.Net;
using System.Net.Http.Json;
using ExoApi.Contracts;
using Xunit;

namespace ExoApi.Tests;

public sealed class ProjetosApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    public ProjetosApiTests(ApiFactory factory)
    {
        factory.ResetDatabase();
        client = factory.CreateClient();
    }

    [Fact]
    public async Task RequerAutenticacaoParaConsultarProjetos()
    {
        var response = await client.GetAsync("/api/projetos");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CriaListaEConsultaProjeto()
    {
        var created = await SendAsRole(HttpMethod.Post, "/api/projetos", "User", ValidRequest());
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var projeto = await created.Content.ReadFromJsonAsync<ProjetoResponse>();
        Assert.NotNull(projeto);
        Assert.Equal("Projeto Integração", projeto.Nome);

        var fetched = await SendAsRole(HttpMethod.Get, $"/api/projetos/{projeto.Id}", "User");
        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
    }

    [Fact]
    public async Task RetornaConflitoParaNomeDuplicado()
    {
        await SendAsRole(HttpMethod.Post, "/api/projetos", "User", ValidRequest());
        var duplicate = await SendAsRole(HttpMethod.Post, "/api/projetos", "User", ValidRequest());
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }

    [Fact]
    public async Task RetornaNaoEncontradoParaProjetoInexistente()
    {
        var response = await SendAsRole(HttpMethod.Get, "/api/projetos/999999", "User");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SomenteAdminPodeExcluir()
    {
        var created = await SendAsRole(HttpMethod.Post, "/api/projetos", "User", ValidRequest("Projeto Protegido"));
        var projeto = await created.Content.ReadFromJsonAsync<ProjetoResponse>();
        Assert.NotNull(projeto);

        var forbidden = await SendAsRole(HttpMethod.Delete, $"/api/projetos/{projeto.Id}", "User");
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);

        var deleted = await SendAsRole(HttpMethod.Delete, $"/api/projetos/{projeto.Id}", "Admin");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
    }

    [Fact]
    public async Task AtualizaProjetoExistente()
    {
        var created = await SendAsRole(HttpMethod.Post, "/api/projetos", "User", ValidRequest("Projeto Original"));
        var projeto = await created.Content.ReadFromJsonAsync<ProjetoResponse>();
        Assert.NotNull(projeto);

        var updated = await SendAsRole(HttpMethod.Put, $"/api/projetos/{projeto.Id}", "User", ValidRequest("Projeto Atualizado"));
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        var result = await updated.Content.ReadFromJsonAsync<ProjetoResponse>();
        Assert.Equal("Projeto Atualizado", result?.Nome);
    }

    private Task<HttpResponseMessage> SendAsRole(HttpMethod method, string path, string role, object? body = null)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Add("X-Test-Role", role);
        if (body is not null) request.Content = JsonContent.Create(body);
        return client.SendAsync(request);
    }

    private static ProjetoRequest ValidRequest(string nome = "Projeto Integração") =>
        new(nome, "Projeto criado durante o teste de integração.", "Em andamento");
}
