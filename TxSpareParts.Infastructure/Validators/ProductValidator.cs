using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class ProductValidator : AbstractValidator<ProductDTO>
    {
        public ProductValidator()
        {
            RuleFor(e => e.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Image)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Description)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Price)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.Category)
                .NotNull()
                .NotEmpty();

            RuleFor(e => e.SupervisorName)
                .Length(1, 60);

            RuleFor(e => e.Quantity)
                .NotNull()
                .NotEmpty();
                
        }
    }
}
