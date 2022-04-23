namespace ApiCdade.Configuracao
{
    public interface IGRCidadeSettings
    {
        public string GRCidadeCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}

