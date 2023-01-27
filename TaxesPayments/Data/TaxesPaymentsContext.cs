using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxesPayments.Models;

namespace TaxesPayments.Data
{
    public class TaxesPaymentsContext : DbContext
    {
        public TaxesPaymentsContext (DbContextOptions<TaxesPaymentsContext> options)
            : base(options)
        {
        }

        public DbSet<TaxesPayments.Models.Taxes> Taxes { get; set; } = default!;
    }
}
