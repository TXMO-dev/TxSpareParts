using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class PhoneNumberValidator : AbstractValidator<PhoneDTO>
    {
        public PhoneNumberValidator()
        {
            RuleFor(e => e.PhoneNumber)
                .NotNull()
                .NotEmpty();
               
        }
    }
}
