using Microsoft.EntityFrameworkCore;
using Remsng.Data.Entities;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;

namespace RemsNG.Data
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
        public DbSet<Lcda> lgdas { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<UserLcda> UserLcdas { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemPenalty> ItemPenalties { get; set; }
        public DbSet<TaxpayerCategory> TaxPayersCategories { get; set; }
        public DbSet<TaxPayer> Taxpayers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyItem> companyItems { get; set; }
        public DbSet<DemandNotice> DemandNotices { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<DemandNoticeTaxpayer> DemandNoticeTaxpayersDetails { get; set; }
        public DbSet<DemandNoticeItem> DemandNoticeItems { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Images> Imagess { get; set; }
        public DbSet<DemandNoticeArrear> DemandNoticeArrearss { get; set; }
        public DbSet<DemandNoticeDownloadHistory> DemandNoticeDownloadHistories { get; set; }
        public DbSet<BankLcda> LcdaBanks { get; set; }
        public DbSet<LcdaProperty> LcdaProperties { get; set; }
        public DbSet<DemandNoticePaymentHistory> DemandNoticePaymentHistories { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Prepayment> Prepayments { get; set; }
        public DbSet<DemandNoticePenalty> DemandNoticePenalties { get; set; }
        public DbSet<BatchDownloadRequest> BatchDownloadRequests { get; set; }
        public DbSet<ContactPerson> ContactPeople { get; set; }
        public DbSet<CloudData> CloudDatas { get; set; }
        public DbQuery<ItemReportSummaryModel> ItemReportSummaryModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region RolePermission
            modelBuilder.Entity<RolePermission>()
                   .HasKey(x => new { x.RoleId, x.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(r => r.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<RolePermission>()
                .HasOne(p => p.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId);
            #endregion

            #region UserDomains
            modelBuilder.Entity<UserDomain>()
                   .HasKey(x => new { x.UserId, x.DomainId });

            modelBuilder.Entity<UserDomain>()
                .HasOne(r => r.User)
                .WithMany(x => x.UserDomains)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserDomain>()
                .HasOne(p => p.Domain)
                .WithMany(x => x.UserDomains)
                .HasForeignKey(x => x.DomainId);
            #endregion

            #region UserLcda
            modelBuilder.Entity<UserLcda>()
                  .HasKey(x => new { x.UserId, x.LgdaId });

            modelBuilder.Entity<UserLcda>()
                .HasOne(r => r.User)
                .WithMany(x => x.UserLcdas)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserLcda>()
                .HasOne(p => p.Lcda)
                .WithMany(x => x.UserLcdas)
                .HasForeignKey(x => x.LgdaId);
            #endregion

            #region UserRole
            modelBuilder.Entity<UserRole>()
                 .HasKey(x => new { x.UserId, x.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(r => r.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(p => p.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId);
            #endregion

            modelBuilder.Entity<Address>()
                .HasOne(x => x.Street)
                .WithMany(r => r.Addresses)
                .HasForeignKey(d => d.StreetId);
            modelBuilder
                .Query<ItemReportSummaryModel>().ToView("View_rptTBlDnItem");
        }
    }
}
/*
 CREATE VIEW View_rptTBlDnItem AS 
select dni.Id, dni.ItemAmount,dni.BillingNo,
'ITEMS' as category,dn.WardId, dnt.WardName,
dnt.TaxpayersName, item.ItemCode, item.ItemDescription,
dni.LastModifiedDate, dnt.AddressName
from tbl_demandNoticeItem as dni
inner join tbl_item as item on item.Id = dni.ItemId
inner join tbl_demandNoticeTaxpayers as dnt on dnt.Id = dni.dn_taxpayersDetailsId
inner join tbl_demandnotice as dn on dn.Id = dnt.DnId

 */
