namespace NSE.WebAPI.Core.Identidade
{
    //obs: Configure() necessário para mapear infos da app settings pra essa minha classe está na  na JwtConfig
    public class AppSettings 
    {
        public string AutenticacaoJwksUrl { get; set; }
    }
}
