using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Users.Model;


namespace Users.DataAccess
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<CareGiver> CareGivers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(
                Environment.GetEnvironmentVariable("DBURI"),
                Environment.GetEnvironmentVariable("dbkey"),
                Environment.GetEnvironmentVariable("DBName"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().ToContainer("Patients").HasPartitionKey(p => p.Id);
            modelBuilder.Entity<CareGiver>().ToContainer("CareGivers").HasPartitionKey(c => c.Id);
            //modelBuilder.Entity<Patient>().HasOne<CareGiver>(p => p.CareGiver).WithMany(c => c.Patients);
            modelBuilder.Entity<CareGiver>().HasMany<Patient>(c => c.Patients).WithOne(p => p.CareGiver);
        }
    }
}
