using Microsoft.EntityFrameworkCore;
using RemsNG.Models;
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
        public DbSet<Lgda> lgdas { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<UserLcda> UserLcdas { get; set; }
        public DbSet<RoleExtension> RoleExtensions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<DbResponse> DbResponses { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemPenalty> ItemPenalties { get; set; }
        public DbSet<TaxpayerCategory> TaxPayersCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("tbl_users");
            modelBuilder.Entity<Domain>().ToTable("tbl_domain");
            modelBuilder.Entity<UserDomain>().ToTable("tbl_userdomain").HasKey(x => new { x.domainId, x.userId });
            modelBuilder.Entity<UserLcda>().ToTable("tbl_userlcda").HasKey(x => new { x.lgdaId, x.userId });
            modelBuilder.Entity<RolePermission>().ToTable("tbl_rolePermission").HasKey(x => new { x.permissionId, x.roleId });
            modelBuilder.Entity<Role>().ToTable("tbl_role");
            modelBuilder.Entity<Address>().ToTable("tbl_address");
            modelBuilder.Entity<Lgda>().ToTable("tbl_lcda");
            modelBuilder.Entity<Ward>().ToTable("tbl_ward");
            modelBuilder.Entity<UserRole>().ToTable("tbl_userRole").HasKey(x => new { x.roleid, x.userid });
            modelBuilder.Entity<ContactDetail>().ToTable("tbl_contactDetail");
            modelBuilder.Entity<Street>().ToTable("tbl_street");
            modelBuilder.Entity<Sector>().ToTable("tbl_sector");
            modelBuilder.Entity<ItemPenalty>().ToTable("tbl_itempenalty");
            modelBuilder.Entity<TaxpayerCategory>().ToTable("tbl_taxpayerCategory");
            modelBuilder.Entity<Item>().ToTable("tbl_item");
        }
    }
}
