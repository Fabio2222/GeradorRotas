namespace ApiEquipe.Configuracao
{
    public class GREquipeSettings : IGREquipeSettings
    {
        public string GREquipeCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
