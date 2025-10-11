using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace DOANCANHAN.Data
{
    public class DOANCANHANContext : DbContext
    {
        public DOANCANHANContext (DbContextOptions<DOANCANHANContext> options)
            : base(options)
        {
        }

        public DbSet<API.Models.Voucher> Voucher { get; set; } = default!;
    }
}
