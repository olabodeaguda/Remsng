using Microsoft.EntityFrameworkCore;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data
{
    public class RemsContext : DbContext
    {
        public RemsContext(DbContextOptions<RemsContext> options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankLcda> BankLcda { get; set; }
        public DbSet<BatchDownloadRequest> BatchDownloadRequests { get; set; }
        public DbSet<CloudData> CloudDatas { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyItem> CompanyItems { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public DbSet<ContactPerson> ContactPeople { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Demandnotice> Demandnotices { get; set; }
        public DbSet<DemandNoticeArrears> DemandNoticeArrears { get; set; }
        public DbSet<DemandNoticeDownloadHistory> DemandNoticeDownloadHistories { get; set; }
        public DbSet<DemandNoticeItem> DemandNoticeItems { get; set; }
        public DbSet<DemandNoticePaymentHistory> DemandNoticePaymentHistories { get; set; }
        public DbSet<DemandNoticePenalty> DemandNoticePenalties { get; set; }
        public DbSet<DemandNoticeTaxpayers> DemandNoticeTaxpayers { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Itempenalty> Itempenalties { get; set; }
        public DbSet<Lcda> Lcdas { get; set; }
        public DbSet<LcdaProperty> LcdaProperties { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<TaxPayer> TaxPayers { get; set; }
        public DbSet<TaxpayerCategory> TaxpayerCategories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDomain> UserDomains { get; set; }
        public DbSet<UserLcda> UserLcdas { get; set; }
        public DbSet<UserRole> userRoles { get; set; }
        public DbSet<Ward> Wards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
