namespace NSE.WebAPI.Core.Identidade
{
    //obs: Configure() necessário para mapear infos da app settings pra essa minha classe está na  na JwtConfig
    public class AppSettings 
    {
        public string Secret { get; set; }
        public int ExpiracaoHoras { get; set; }
        public string Emissor { get; set; }
        public string ValidoEm { get; set; }
    }
}
