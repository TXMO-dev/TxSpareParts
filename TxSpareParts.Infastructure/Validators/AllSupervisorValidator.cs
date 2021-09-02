using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class AllSupervisorValidator : AbstractValidator<AllSupervisorDTO>
    {
        public AllSupervisorValidator()
        {
            RuleFor(e => e.FirstName)
                .Length(30);

            RuleFor(e => e.LastName)
                .Length(30);

            RuleFor(e => e.PhoneNumber)
                .Length(18);
        }
    }
}
