using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class EmployeeEmailValidator : AbstractValidator<EmployeeEmailDTO>
    {
        public EmployeeEmailValidator()
        {
            RuleFor(e => e.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress();

            RuleFor(e => e.companyname);

            RuleFor(e => e.SupervisorName);
        }
    }
}
