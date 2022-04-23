namespace GeradorRotas.Frontend.Configuracao
{
    public interface IGRotasSettings
    {
        public string GRotasCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
