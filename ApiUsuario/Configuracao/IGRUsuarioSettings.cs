namespace ApiUsuario.Configuracao
{
    public interface IGRUsuarioSettings
    {
        public string GRUsuarioCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
