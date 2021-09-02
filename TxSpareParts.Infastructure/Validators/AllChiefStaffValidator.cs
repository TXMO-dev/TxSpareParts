using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    public class AllChiefStaffValidator : AbstractValidator<AllChiefStaffDTO>
    {
        public AllChiefStaffValidator()
        {
            RuleFor(e => e.CompanyName)
                .Length(1, 60);

            RuleFor(e => e.FirstName)
                .Length(40);

            RuleFor(e => e.LastName)
                .Length(40);

            RuleFor(e => e.EmployeeStatus)
                .Length(30);
        }
    }
}
