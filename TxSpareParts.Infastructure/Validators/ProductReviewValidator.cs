using FluentValidation;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class ProductReviewValidator : AbstractValidator<ProductReviewDTO>
    {
        public ProductReviewValidator()
        {
            RuleFor(e => e.Comment)
                .NotNull()
                .NotEmpty()
                .Length(1, 400);

            RuleFor(e => e.Rating)
                .NotNull()
                .NotEmpty()
                .LessThanOrEqualTo(5);
        }
    }
}
