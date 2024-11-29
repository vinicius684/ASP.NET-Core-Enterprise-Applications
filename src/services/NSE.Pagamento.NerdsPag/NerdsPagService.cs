
namespace NSE.Pagamentos.NerdsPag
{
    public class NerdsPagService //obj onde vou passar a Chave da API e Chave de criptografia(criptografar as infos que vou transacionar por cliente) - chaves que serão recebida quando fizer o cadastro no gateway de pagamento
    {
        public readonly string ApiKey;
        public readonly string EncryptionKey;

        public NerdsPagService(string apiKey, string encryptionKey)
        {
            ApiKey = apiKey;
            EncryptionKey = encryptionKey;
        }
    }
}