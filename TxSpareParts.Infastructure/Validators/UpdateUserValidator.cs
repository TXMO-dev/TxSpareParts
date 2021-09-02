using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDTO>
    {
        public  UpdateUserValidator()
        {
            RuleFor(e => e.Email)
                .Length(1,50)
                .EmailAddress();

            RuleFor(e => e.FirstName)
                .Length(1, 50);

            RuleFor(e => e.LastName)
                .Length(1, 50);

            RuleFor(e => e.CurrentPassword)
                .Length(4, 40);

            RuleFor(e => e.DigitalAddress)
                .Length(11, 12);

            RuleFor(e => e.PhysicalAddress)
                .Length(1, 60);

            RuleFor(e => e.City)
                .Length(1, 50);

            RuleFor(e => e.Region)
                .Length(1, 50);

            RuleFor(e => e.PhoneNumber)
                .Length(12, 16);

            RuleFor(e => e.NewPassword)
                .NotNull()
                .NotEmpty()
                .When(e => e.CurrentPassword != null);

            RuleFor(e => e.ConfirmNewPassword)
                .NotNull()
                .NotEmpty()
                .When(e => e.NewPassword != null);   
        }
    }
}
