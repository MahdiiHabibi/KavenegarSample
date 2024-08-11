using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KavenegarSample.Models;

namespace KavenegarSample.Data
{
    public class ContextDb : DbContext
    {
        public ContextDb (DbContextOptions<ContextDb> options)
            : base(options)
        {
        }

        public DbSet<KavenegarSample.Models.Person> Person { get; set; } = default!;
    }
}
