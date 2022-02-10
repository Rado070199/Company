using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;
using Company.Models;

namespace Company.DAL
{
    public class CompanyContext : DbContext
    {
        public CompanyContext() : base("DefaultConnection")
        {

        }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<Topicality> Topicalities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        }
    }
}