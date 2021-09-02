using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class AdminValidator : AbstractValidator<AdminDTO>
    {
        public AdminValidator()
        {

            RuleFor(e => e.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();

            RuleFor(e => e.FirstName)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.LastName)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.PhoneNumber)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Password)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.ConfirmPassword)
                .NotNull()
                .NotEmpty();
        }
    }
}
