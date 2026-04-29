using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InsureTrust.QueryService.Models;

   namespace InsureTrust.QueryService.Data
{
    public class InsureTrustQueryServiceContext : DbContext
    {
        public InsureTrustQueryServiceContext (DbContextOptions<InsureTrustQueryServiceContext> options)
            : base(options)
        {
        }

        public DbSet<InsureTrust.QueryService.Models.SupportQuery> SupportQuery { get; set; } = default!;
    }
}
