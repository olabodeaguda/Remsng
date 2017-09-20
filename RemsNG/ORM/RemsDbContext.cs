﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class RemsDbContext : DbContext
    {
        public RemsDbContext(DbContextOptions<RemsDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<UserDomain> UserDomains { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Lcda> lcdas { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<UserLcda> UserLcdas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("tbl_users");
            modelBuilder.Entity<Domain>().ToTable("tbl_domain");
            modelBuilder.Entity<UserDomain>().ToTable("tbl_userdomain").HasKey(x => new { x.domainId, x.userId });
            modelBuilder.Entity<UserLcda>().ToTable("tbl_userlcda").HasKey(x => new { x.lgdaId, x.userId });
            modelBuilder.Entity<Role>().ToTable("tbl_role");
            modelBuilder.Entity<Address>().ToTable("tbl_address");
            modelBuilder.Entity<Lcda>().ToTable("tbl_lcda");
            modelBuilder.Entity<Ward>().ToTable("tbl_ward");
        }
    }
}
