using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class SupervisorValidator : AbstractValidator<SupervisorDTO>
    {
        public SupervisorValidator()
        {
            RuleFor(e => e.assignedtosupervisor)
                .Length(50);
        }
    }
}
