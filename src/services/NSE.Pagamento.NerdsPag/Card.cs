using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NSE.Pagamentos.NerdsPag
{

    /*
        Dados de um cartão, na vdd trata tanto dados de cartão quanto do próprio cardHash

        Cardhash: vc vai pegar os dados do cartão do seu cliente, através do seu meio seguro e vc não vai informar esse cartão lá pro gateway. Vc vai formar o hash através da criptografia que vc tem a chave.
        Quando tenho o CardHash estou pronto para iniciar a transação
     */
    public class CardHash
    {
        public CardHash(NerdsPagService nerdsPagService)
        {
            NerdsPagService = nerdsPagService;
        }

        private readonly NerdsPagService NerdsPagService;

        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string CardExpirationDate { get; set; }
        public string CardCvv { get; set; }

        public string Generate()
        {
            using var aesAlg = Aes.Create();//simulação de uma criptografia baseada em Aes (algoritmo confiável, mas não é exatamente assim que é feito em um gateway vai ter o próprio meio dele) 

            aesAlg.IV = Encoding.Default.GetBytes(NerdsPagService.EncryptionKey);
            aesAlg.Key = Encoding.Default.GetBytes(NerdsPagService.ApiKey);

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(CardHolderName + CardNumber + CardExpirationDate + CardCvv);//gerar a criptografia com base nos dados do cartão
            }

            return Encoding.ASCII.GetString(msEncrypt.ToArray()); //vai devolver um array string que seria exatamento hash desse cartão
        }
    }
}