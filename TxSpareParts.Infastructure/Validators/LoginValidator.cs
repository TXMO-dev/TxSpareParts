using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(login => login.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();

            RuleFor(login => login.Password)
                .NotNull()
                .NotEmpty();
                

            
        }
    }
}
