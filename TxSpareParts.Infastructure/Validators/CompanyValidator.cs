using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class CompanyValidator : AbstractValidator<CompanyDTO>
    {
        public CompanyValidator()
        {
            RuleFor(e => e.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Email)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.PhoneNumber)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Address)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.City)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Region)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.DigitalAddress)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.PostOfficeBox)
                .NotEmpty()
                .NotNull();

            
        }
    }
}
