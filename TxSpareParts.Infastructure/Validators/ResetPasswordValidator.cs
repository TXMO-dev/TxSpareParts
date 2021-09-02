using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordValidator()
        {
            RuleFor(e => e.NewPassword)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.ConfirmNewPassword)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Token)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();

        }
    }
}
