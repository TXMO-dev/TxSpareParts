using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class CompanyReviewValidator : AbstractValidator<CompanyReviewDTO>
    {
        public CompanyReviewValidator()
        {
            RuleFor(e => e.Comment)
                .NotNull()
                .NotEmpty()
                .Length(450);

            RuleFor(e => e.Rating)
                .NotEmpty()
                .NotNull()
                .LessThanOrEqualTo(5);
        }
    }
}
