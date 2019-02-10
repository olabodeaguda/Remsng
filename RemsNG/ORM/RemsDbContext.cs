using Microsoft.EntityFrameworkCore;
using RemsNG.Models;

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
        public DbSet<Taxpayer> Taxpayers { get; set; }
        public DbSet<TaxpayerExtension> TaxpayerExtensions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyExt> CompanyExts { get; set; }
        public DbSet<CompanyItem> companyItems { get; set; }
        public DbSet<CompanyItemExt> companyItemExts { get; set; }
        public DbSet<DemandNotice> DemandNotices { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<DemandNoticeTaxpayersDetail> DemandNoticeTaxpayersDetails { get; set; }
        public DbSet<DemandNoticeItem> DemandNoticeItems { get; set; }
        public DbSet<DemandNoticeItemExt> DemandNoticeItemExts { get; set; }
        public DbSet<DemandNoticeArrearsExt> DemandNoticeArrearsExts { get; set; }
        public DbSet<DemandNoticeItemPenaltyExt> DemandNoticeItemPenaltyExts { get; set; }
        public DbSet<DemandNoticeItemExtension> DemandNoticeItemExtensions { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Images> Imagess { get; set; }
        public DbSet<DemandNoticeItemPenalty> DemandNoticeItemPenaties { get; set; }
        public DbSet<DemandNoticeArrears> DemandNoticeArrearss { get; set; }
        public DbSet<DemandNoticeDownloadHistory> DemandNoticeDownloadHistories { get; set; }
        public DbSet<LcdaBank> LcdaBanks { get; set; }
        public DbSet<BatchDemandNoticeModel> BatchDemanNoticeModels { get; set; }
        public DbSet<LcdaProperty> LcdaProperties { get; set; }
        public DbSet<DNAmountDueModel> DNAmountDueModels { get; set; }
        public DbSet<DemandNoticePaymentHistory> DemandNoticePaymentHistories { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<DemandNoticePaymentHistoryExt> DemandNoticePaymentHistoryExts { get; set; }
        public DbSet<ItemReportSummaryModel> ItemReportSummaryModels { get; set; }
        public DbSet<ChartReport> ChartReports { get; set; }
        public DbSet<DnError> DnErroor { get; set; }
        public DbSet<SyncDataModel> SyncDataModels { get; set; }
        public DbSet<Prepayment> Prepayments { get; set; }
        public DbSet<TaxpayerExtension2> TaxpayerExtension2s { get; set; }

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
            modelBuilder.Entity<Taxpayer>().ToTable("tbl_taxPayer");
            modelBuilder.Entity<Company>().ToTable("tbl_company");
            modelBuilder.Entity<CompanyItem>().ToTable("tbl_companyItem");
            modelBuilder.Entity<DemandNotice>().ToTable("tbl_demandnotice");
            modelBuilder.Entity<Error>().ToTable("tbl_error");
            modelBuilder.Entity<DemandNoticeTaxpayersDetail>().ToTable("tbl_demandNoticeTaxpayers");
            modelBuilder.Entity<DemandNoticeItem>().ToTable("tbl_demandNoticeItem");
            modelBuilder.Entity<State>().ToTable("tbl_state");
            modelBuilder.Entity<Images>().ToTable("tbl_images");
            modelBuilder.Entity<DemandNoticeItemPenalty>().ToTable("tbl_demandNoticePenalty");
            modelBuilder.Entity<DemandNoticeArrears>().ToTable("tbl_demandNoticeArrears");
            modelBuilder.Entity<DemandNoticeDownloadHistory>().ToTable("tbl_DemandNoticeDownloadHistory");
            modelBuilder.Entity<BatchDemandNoticeModel>().ToTable("tbl_batchDownloadRequest");
            modelBuilder.Entity<LcdaProperty>().ToTable("tbl_LcdaProperty");
            modelBuilder.Entity<Prepayment>().ToTable("tbl_prepayment");
        }
    }
}
