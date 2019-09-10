using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Interfaces.Services;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using RemsNG.Infrastructure.Managers;
using RemsNG.Infrastructure.Services;

namespace RemsNG.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddAppServices(this IServiceCollection services,
          IConfiguration config)
        {
            //Managers
            #region Managers
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IDomainManager, DomainManager>();
            services.AddScoped<ILcdaManager, LcdaManager>();
            services.AddScoped<IWardManager, WardManager>();
            services.AddScoped<IRoleManager, RoleManager>();
            services.AddScoped<IPermissionManager, PermissionManager>();
            services.AddScoped<IContactManager, ContactManager>();
            services.AddScoped<IStreetManager, StreetManager>();
            services.AddScoped<ISectorManager, SectorManager>();
            services.AddScoped<IItemManager, ItemManager>();
            services.AddScoped<IItemPenaltyManager, ItemPenaltyManager>();
            services.AddScoped<ITaxpayerCategoryManager, TaxpayerCategoryManager>();
            services.AddScoped<ITaxpayerManager, TaxpayerManager>();
            services.AddScoped<ICompanyManager, CompanyManager>();
            services.AddScoped<IAddressManager, AddressManager>();
            services.AddScoped<ICompanyItemManager, CompanyItemManager>();
            services.AddScoped<IDemandNoticeManager, DemanNoticeManager>();
            services.AddScoped<IRunDemandNoticeManager, RunDemandNoticeManager>();
            services.AddScoped<IDnTaxpayerManager, DnTaxpayerManagers>();
            services.AddScoped<IStateManagers, StateManagers>();
            services.AddScoped<IImageManager, ImageManager>();
            services.AddScoped<IDemandNoticeTaxpayerManager, DemandNoticeTaxpayerManager>();
            services.AddScoped<IDemandNoticeItemManager, DemandNoticeItemManager>();
            services.AddScoped<IDnDownloadManager, DnDownloadManager>();
            services.AddScoped<IDemandNoticeChargesManager, DemandNoticeChargesManager>();
            services.AddScoped<IDemandNoticeChargesManager, DemandNoticeChargesManager>();
            services.AddScoped<IDemandNoticeDownloadHistoryManager, DemandNoticeDownloadHistoryManager>();
            services.AddScoped<ILcdaBankManager, LcdaBankManager>();
            services.AddScoped<IBatchDwnRequestManager, BatchDwnRequestManager>();
            services.AddScoped<IListPropertyManager, ListPropertyManager>();
            services.AddScoped<IDNAmountDueMgtManager, DNAmountDueMgtManager>();
            services.AddScoped<IDNPaymentHistoryManager, DNPaymentHistoryManager>();
            services.AddScoped<IBankManager, BankManager>();
            services.AddScoped<IAbstractManager, AbstractManager>();
            services.AddScoped<IReportManager, ReportManager>();
            services.AddScoped<ISyncManager, SyncManager>();
            services.AddScoped<IPenaltyManager, PenaltyManager>();
            services.AddScoped<IArrearsManager, ArrearsManager>();

            #endregion

            // Configuration
            services.AddSingleton<IConfigurationRoot>(provider => (IConfigurationRoot)config);
            services.AddSingleton(config.GetSection("BankCategory").Get<BankCategory>());
            services.AddSingleton(config.GetSection("TemplateDetail").Get<TemplateDetail>());

            //Services
            #region Services
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IExcelService, ExcelService>();
            #endregion

            //Repository
            #region Repository
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IBatchDownloadRequestRepository, BatchDownloadRequestRepository>();
            services.AddScoped<ICompanyItemRepository, CompanyItemRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IDemandNoticeArrearRepository, DemandNoticeArrearRepository>();
            services.AddScoped<IDemandNoticeDownloadHistoryRepository, DemandNoticeDownloadHistoryRepository>();
            services.AddScoped<IDemandNoticeItemRepository, DemandNoticeItemRepository>();
            services.AddScoped<IDemandNoticePaymentHistoryRepository, DemandNoticePaymentHistoryRepository>();
            services.AddScoped<IDemandNoticePenaltyRepository, DemandNoticePenaltyRepository>();
            services.AddScoped<IDemandNoticeRepository, DemandNoticeRepository>();
            services.AddScoped<IDemandNoticeTaxpayersRepository, DemandNoticeTaxpayersRepository>();
            services.AddScoped<IDNAmountDueMgtRepository, DNAmountDueMgtRepository>();
            services.AddScoped<IDomainRepository, DomainRepository>();
            services.AddScoped<IErrorRepository, ErrorRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IItemPenaltyRepository, ItemPenaltyRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ILcdaBankRepository, LcdaBankRepository>();
            services.AddScoped<ILcdaPropertyRepository, LcdaPropertyRepository>();
            services.AddScoped<ILcdaRepository, LcdaRepository>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ISectorRepository, SectorRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<IStreetRepository, StreetRepository>();
            services.AddScoped<ISyncRepository, SyncRepository>();
            services.AddScoped<ITaxpayerCatgoryRepository, TaxpayerCatgoryRepository>();
            services.AddScoped<ITaxpayerRepository, TaxpayerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWardRepository, WardRepository>();
            services.AddScoped<IDNPaymentHistoryRepository, DNPaymentHistoryRepository>();
            #endregion
        }
    }
}
