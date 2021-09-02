using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Validators
{
    class AllAdminStaffValidator : AbstractValidator<AllAdminStaffDTO>
    {
        public AllAdminStaffValidator()
        {
            RuleFor(e => e.CompanyName)
                .Length(1, 50);

            RuleFor(e => e.FirstName)
                .Length(30);

            RuleFor(e => e.LastName)
               .Length(30);

            RuleFor(e => e.EmployeeStatus)
                .Length(40);

        }
    }
}
