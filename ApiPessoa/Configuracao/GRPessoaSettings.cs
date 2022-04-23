namespace ApiPessoa.Configuracao
{
    public class GRPessoaSettings : IGRPessoaSettings
    {
        public string GRPessoaCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
