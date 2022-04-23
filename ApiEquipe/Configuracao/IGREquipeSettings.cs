namespace ApiEquipe.Configuracao
{
    public interface IGREquipeSettings
    {
        public string GREquipeCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
