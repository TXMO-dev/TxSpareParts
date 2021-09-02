using FluentValidation;   
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class HandleDisputeValidator : AbstractValidator<HandleDisputeDTO>
    {
        public HandleDisputeValidator()
        {
            RuleFor(e => e.From)
                .NotEmpty()
                .NotNull();

            RuleFor(e => e.To)
                .NotEmpty()
                .NotNull();
                
        }
    }
}
