using ContactApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContactApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Contact> Contacts => Set<Contact>();
    }
}
