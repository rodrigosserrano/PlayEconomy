namespace Play.Common.Settings;

public class MongoDbSettings
{
    // Declarando "init" não dará a possibilidade de alguém alterar a informação das propriedades após a inicialização do serviço
    public string Host { get; init; }
    public int Port { get; init; }
    public string ConnectionString => $"mongodb://{Host}:{Port}";
}