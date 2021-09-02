using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Infastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ApplicationDbContextFactory()
        {

        }
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=txspareparts;Trusted_Connection=true");   
            return new ApplicationDbContext(optionsBuilder.Options);     
        }
    }
}
