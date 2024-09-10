using FluentValidation.Results;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace NSE.Core.Messages.Integration
{
    public class ResponseMessage : Message
    {
        public ValidationResult ValidationResult { get; set; }

        public ResponseMessage(ValidationResult validationResult) 
        {
            validationResult = validationResult;
        }
    }
}
