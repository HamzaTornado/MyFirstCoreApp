using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOME.FINANCEMENT.API.Models;


namespace HOME.FINANCEMENT.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options){}
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
