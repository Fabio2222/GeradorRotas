namespace ApiPessoa.Configuracao
{
    public interface IGRPessoaSettings
    {
        public string GRPessoaCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
