namespace ApiCdade.Configuracao
{
    public class GRCidadeSettings : IGRCidadeSettings
    {
        public string GRCidadeCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
