using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InsureTrust.QueryService.Models;

namespace InsureTrust.SupportService.Data
{
    public class InsureTrustSupportServiceContext : DbContext
    {
        public InsureTrustSupportServiceContext (DbContextOptions<InsureTrustSupportServiceContext> options)
            : base(options)
        {
        }

        public DbSet<InsureTrust.QueryService.Models.SupportQuery> SupportQuery { get; set; } = default!;
    }
}
