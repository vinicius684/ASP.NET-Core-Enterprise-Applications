using FluentValidation.Results;
using MediatR;

namespace NSE.Core.Messages
{
    public abstract class Command : Message, IRequest<ValidationResult>
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }//objeto que possui uma lista de erros

        protected Command() 
        {
            Timestamp = DateTime.Now;
        }

        public virtual bool EhValido() //uma vez que é virtual, posso dar override nele, porém não sou obrigado a dar e se chamar o eh valido dentro de um commando sem implementá-lo, vai cair na exception de método não implementado
        {
            throw new NotImplementedException();
        }
    }
}
