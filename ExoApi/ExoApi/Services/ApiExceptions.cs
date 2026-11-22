namespace ExoApi.Services;

public sealed class ProjetoNaoEncontradoException(int id)
    : Exception($"O projeto de id {id} não foi encontrado.");

public sealed class ProjetoDuplicadoException(string nome)
    : Exception($"Já existe um projeto com o nome '{nome}'.");
