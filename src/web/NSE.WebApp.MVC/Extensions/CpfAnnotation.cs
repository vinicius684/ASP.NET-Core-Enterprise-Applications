using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using NSE.Core.DomainObjects;

namespace NSE.WebApp.MVC.Extensions
{
    public class CpfAttribute : ValidationAttribute //ValidationAttribute - qualquer atributo de validação do datta annotation
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return Cpf.Validar(value.ToString()) ? ValidationResult.Success : new ValidationResult("CPF em formato inválido"); //utilizando validação lá do DomainObjects
        }
    }

    public class CpfAttributeAdapter : AttributeAdapterBase<CpfAttribute> //adaptador para que eu possa usar as validações no front-end
    {

        public CpfAttributeAdapter(CpfAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
        {

        }
        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true"); 
            MergeAttribute(context.Attributes, "data-val-cpf", GetErrorMessage(context));
        }
        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return "CPF em formato inválido";
        }
    }

    public class CpfValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider //provider para o adapter
    {
        private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();
        
        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            if (attribute is CpfAttribute CpfAttribute)
            {
                return new CpfAttributeAdapter(CpfAttribute, stringLocalizer);
            }

            return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}