using System.Threading.Tasks;
using FluentValidation.Results;
using NSE.Core.Data;

namespace NSE.Core.Messages
{
    public abstract class CommandHandler
    {
        protected ValidationResult ValidationResult;

        protected CommandHandler()
        {
            ValidationResult = new ValidationResult();
        }

        protected void AdicionarErro(string mensagem)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));//primeiro parâmetro é propriedade, no caso se quiser passar a propriedade(atributo) do dompinio falhou
        }

        protected async Task<ValidationResult> PersistirDados(IUnitOfWork uow)
        {
            if (!await uow.Commit()) AdicionarErro("Houve um erro ao persistir os dados");

            return ValidationResult; //independente de salvar ou não, vai retornar o ValidationResult e quem for receber vai ter que dar um IsValid nesse VR para saber se a coisa funcionou bem ou não
        }
    }
}