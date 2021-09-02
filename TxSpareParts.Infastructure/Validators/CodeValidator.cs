using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class CodeValidator : AbstractValidator<CodeDTO>
    {
        public CodeValidator()
        {
            RuleFor(e => e.Code)
                .NotNull()
                .NotEmpty();
        }
    }
}
