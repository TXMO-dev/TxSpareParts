using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(register => register.Email)
                .NotNull()
                .EmailAddress();

            RuleFor(register => register.FirstName)
                .NotNull()
                .Length(2, 60);

            RuleFor(register => register.LastName)
                .NotNull()
                .Length(2, 60);

            RuleFor(register => register.Password)
                .NotNull()
                .Length(5, 150)
                .WithMessage("Password must be atleast 5 characters and should include Uppercase,Lowercase and symbols");

            RuleFor(register => register.ConfirmPassword)
                .NotNull()
                .Length(5, 150)
                .WithMessage("Password must be atleast 5 characters and should include Uppercase,Lowercase and symbols");

            RuleFor(register => register.PhoneNumber)
                .Length(10, 16);   

            RuleFor(register => register.DigitalAddress)
                .Length(11, 12);
               
            RuleFor(register => register.PhysicalAdress)
                .Length(1, 60);

            RuleFor(register => register.City)
                .Length(1, 50);

            RuleFor(register => register.Region)
                .Length(1, 50);

            RuleFor(register => register.Code)
                .Length(1, 8);

        }
    }
}
