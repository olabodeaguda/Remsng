﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RemsNG.Data;

namespace RemsNG.Migrations
{
    [DbContext(typeof(RemsDbContext))]
    [Migration("20190228083703_stateDomainrela")]
    partial class stateDomainrela
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RemsNG.Data.Entities.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Addressnumber");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("Lcdaid");

                    b.Property<Guid>("OwnerId");

                    b.Property<Guid>("StreetId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("StreetId");

                    b.ToTable("tbl_address");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Bank", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BankName");

                    b.Property<DateTime?>("DateCreated");

                    b.HasKey("Id");

                    b.ToTable("tbl_bank");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.BankLcda", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BankAccount");

                    b.Property<Guid>("BankId");

                    b.Property<Guid>("LcdaId");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("LcdaId");

                    b.ToTable("tbl_bank_lcda");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.BatchDownloadRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BatchFileName");

                    b.Property<string>("BatchNo");

                    b.Property<string>("Createdby");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid?>("LcdaId");

                    b.Property<string>("RequestStatus");

                    b.HasKey("Id");

                    b.ToTable("tbl_batchDownloadRequest");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.CloudData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BillingNumber");

                    b.Property<string>("DataTitle");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<Guid>("DomainId");

                    b.Property<string>("JsonContent");

                    b.Property<string>("SyncStatus");

                    b.HasKey("Id");

                    b.ToTable("tbl_cloudData");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AddressId");

                    b.Property<Guid?>("CategoryId");

                    b.Property<string>("CompanyName");

                    b.Property<string>("CompanyStatus");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("LcdaId");

                    b.Property<Guid?>("SectorId");

                    b.Property<Guid?>("StreetId");

                    b.Property<Guid?>("TaxPayerCatgeoryId");

                    b.HasKey("Id");

                    b.HasIndex("TaxPayerCatgeoryId");

                    b.ToTable("tbl_company");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.CompanyItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<int>("BillingYear");

                    b.Property<string>("CompanyStatus");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<Guid>("ItemId");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("TaxpayerId");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("TaxpayerId");

                    b.ToTable("tbl_companyItem");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.ContactDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContactType");

                    b.Property<string>("ContactValue");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("OwnerId");

                    b.HasKey("Id");

                    b.ToTable("tbl_contactDetail");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.ContactPerson", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("Firstname");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<string>("Lastname");

                    b.Property<string>("Surname");

                    b.Property<Guid?>("TaxPayerId");

                    b.HasKey("Id");

                    b.ToTable("tbl_contactPerson");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CountryCode");

                    b.Property<string>("CountryName");

                    b.HasKey("Id");

                    b.ToTable("tbl_country");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNotice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BatchNo");

                    b.Property<int>("BillingYear");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("DemandNoticeStatus");

                    b.Property<bool>("IsUnbilled");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("LcdaId");

                    b.Property<string>("PlainTextQuery");

                    b.Property<string>("Query");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid?>("StreetId");

                    b.Property<Guid?>("WardId");

                    b.HasKey("Id");

                    b.HasIndex("StreetId");

                    b.HasIndex("WardId");

                    b.ToTable("tbl_demandnotice");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticeArrears", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AmountPaid");

                    b.Property<string>("ArrearsStatus");

                    b.Property<string>("BillingNo");

                    b.Property<int>("BillingYear");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<Guid>("ItemId");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<int>("OriginatedYear");

                    b.Property<Guid>("TaxpayerId");

                    b.Property<decimal>("TotalAmount");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("TaxpayerId");

                    b.ToTable("tbl_demandNoticeArrears");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticeDownloadHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BillingNumber");

                    b.Property<decimal>("Charges");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<decimal>("GrandTotal");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.HasKey("Id");

                    b.ToTable("tbl_DemandNoticeDownloadHistory");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticeItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AmountPaid");

                    b.Property<string>("BillingNo");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<Guid?>("DemandNoticeId");

                    b.Property<Guid>("DnTaxpayersDetailsId");

                    b.Property<decimal>("ItemAmount");

                    b.Property<Guid>("ItemId");

                    b.Property<string>("ItemName");

                    b.Property<string>("ItemStatus");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("TaxpayerId");

                    b.HasKey("Id");

                    b.HasIndex("DemandNoticeId");

                    b.HasIndex("ItemId");

                    b.HasIndex("TaxpayerId");

                    b.ToTable("tbl_demandNoticeItem");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticePaymentHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<Guid>("BankId");

                    b.Property<string>("BillingNumber");

                    b.Property<decimal>("Charges");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<bool>("IsWaiver");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("OwnerId");

                    b.Property<string>("PaymentMode");

                    b.Property<string>("PaymentStatus");

                    b.Property<string>("ReferenceNumber");

                    b.Property<bool>("SyncStatus");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.ToTable("tbl_demandNoticePaymentHistory");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticePenalty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AmountPaid");

                    b.Property<string>("BillingNo");

                    b.Property<int>("BillingYear");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<Guid>("ItemId");

                    b.Property<string>("ItemPenaltyStatus");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<int>("OriginatedYear");

                    b.Property<Guid>("TaxpayerId");

                    b.Property<decimal>("TotalAmount");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("TaxpayerId");

                    b.ToTable("tbl_demandNoticePenalty");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticeTaxpayers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressName");

                    b.Property<string>("BillingNumber");

                    b.Property<int>("BillingYr");

                    b.Property<string>("CouncilTreasurerMobile");

                    b.Property<string>("CouncilTreasurerSigFilen");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("DemandNoticeStatus");

                    b.Property<Guid>("DnId");

                    b.Property<string>("DomainName");

                    b.Property<bool>("IsUnbilled");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<string>("LcdaAddress");

                    b.Property<string>("LcdaLogoFileName");

                    b.Property<string>("LcdaName");

                    b.Property<string>("LcdaState");

                    b.Property<int>("Period");

                    b.Property<string>("RevCoodinatorSigFilen");

                    b.Property<Guid>("TaxpayerId");

                    b.Property<string>("TaxpayersName");

                    b.Property<string>("WardName");

                    b.HasKey("Id");

                    b.HasIndex("DnId");

                    b.ToTable("tbl_demandNoticeTaxpayers");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Domain", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AddressId");

                    b.Property<DateTime>("Datecreated");

                    b.Property<string>("DomainCode");

                    b.Property<string>("DomainName");

                    b.Property<string>("DomainStatus");

                    b.Property<string>("DomainType");

                    b.Property<Guid?>("StateId");

                    b.HasKey("Id");

                    b.HasIndex("StateId");

                    b.ToTable("tbl_domain");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Error", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("ErrorType");

                    b.Property<string>("Errorvalue");

                    b.Property<Guid>("OwnerId");

                    b.HasKey("Id");

                    b.ToTable("tbl_error");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Images", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("ImgFilename");

                    b.Property<string>("ImgType");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("OwnerId");

                    b.HasKey("Id");

                    b.ToTable("tbl_images");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Item", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("ItemCode");

                    b.Property<string>("ItemDescription");

                    b.Property<string>("ItemStatus");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("LcdaId");

                    b.HasKey("Id");

                    b.HasIndex("LcdaId");

                    b.ToTable("tbl_item");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.ItemPenalty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("Duration");

                    b.Property<bool>("IsPercentage");

                    b.Property<Guid>("ItemId");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<string>("PenaltyStatus");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("tbl_itempenalty");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Lcda", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AddressId");

                    b.Property<decimal>("Charges");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<Guid>("DomainId");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<string>("LcdaCode");

                    b.Property<string>("LcdaName");

                    b.Property<string>("LcdaStatus");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.ToTable("tbl_lcda");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.LcdaProperty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("LcdaId");

                    b.Property<string>("PropertyKey");

                    b.Property<string>("PropertyStatus");

                    b.Property<string>("PropertyValue");

                    b.HasKey("Id");

                    b.ToTable("tbl_LcdaProperty");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Permission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PermissionDescription");

                    b.Property<string>("PermissionName");

                    b.HasKey("Id");

                    b.ToTable("tbl_permission");
                });

            modelBuilder.Entity("Remsng.Data.Entities.Prepayment", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("amount");

                    b.Property<DateTime>("datecreated");

                    b.Property<string>("prepaymentStatus");

                    b.Property<Guid>("taxpayerId");

                    b.HasKey("id");

                    b.ToTable("tbl_prepayment");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("DomainId");

                    b.Property<string>("RoleName");

                    b.Property<string>("RoleStatus");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.ToTable("tbl_role");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.RolePermission", b =>
                {
                    b.Property<Guid>("RoleId");

                    b.Property<Guid>("PermissionId");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("tbl_rolePermission");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Sector", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("LcdaId");

                    b.Property<string>("Prefix");

                    b.Property<string>("SectorName");

                    b.HasKey("Id");

                    b.HasIndex("LcdaId");

                    b.ToTable("tbl_sector");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.State", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CountryId");

                    b.Property<string>("StateCode");

                    b.Property<string>("StateName");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("tbl_state");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Street", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<int?>("NumberOfHouse");

                    b.Property<string>("StreetDescription");

                    b.Property<string>("StreetName");

                    b.Property<string>("StreetStatus");

                    b.Property<Guid>("WardId");

                    b.HasKey("Id");

                    b.HasIndex("WardId");

                    b.ToTable("tbl_street");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.TaxPayer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AddressId");

                    b.Property<Guid>("CompanyId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("Firstname");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<string>("Lastname");

                    b.Property<Guid?>("StreetId");

                    b.Property<string>("Surname");

                    b.Property<string>("TaxpayerStatus");

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .IsUnique()
                        .HasFilter("[AddressId] IS NOT NULL");

                    b.HasIndex("CompanyId");

                    b.HasIndex("StreetId");

                    b.ToTable("tbl_taxPayer");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.TaxpayerCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("LcdaId");

                    b.Property<string>("TaxpayerCategoryName");

                    b.HasKey("Id");

                    b.ToTable("tbl_taxpayerCategory");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<string>("Email");

                    b.Property<string>("Firstname");

                    b.Property<string>("Gender");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<string>("Lastname");

                    b.Property<DateTime?>("LockedOutEndDateUtc");

                    b.Property<bool?>("Lockedoutenabled");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Surname");

                    b.Property<string>("UserStatus");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("tbl_users");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.UserDomain", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("DomainId");

                    b.HasKey("UserId", "DomainId");

                    b.HasIndex("DomainId");

                    b.ToTable("tbl_userdomain");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.UserLcda", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("LgdaId");

                    b.HasKey("UserId", "LgdaId");

                    b.HasIndex("LgdaId");

                    b.ToTable("tbl_userlcda");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.UserRole", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("tbl_userRole");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Ward", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("LastModifiedDate");

                    b.Property<string>("Lastmodifiedby");

                    b.Property<Guid>("LcdaId");

                    b.Property<string>("WardName");

                    b.Property<string>("WardStatus");

                    b.HasKey("Id");

                    b.HasIndex("LcdaId");

                    b.ToTable("tbl_ward");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Address", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.TaxPayer", "Taxpayer")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.Street", "Street")
                        .WithMany()
                        .HasForeignKey("StreetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.BankLcda", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Bank", "Bank")
                        .WithMany()
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.Lcda", "Lcda")
                        .WithMany()
                        .HasForeignKey("LcdaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Company", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.TaxpayerCategory", "TaxPayerCatgeory")
                        .WithMany()
                        .HasForeignKey("TaxPayerCatgeoryId");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.CompanyItem", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Item", "Item")
                        .WithMany("CompanyItem")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.TaxPayer", "TaxPayer")
                        .WithMany("Items")
                        .HasForeignKey("TaxpayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNotice", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Street", "Street")
                        .WithMany()
                        .HasForeignKey("StreetId");

                    b.HasOne("RemsNG.Data.Entities.Ward", "Ward")
                        .WithMany()
                        .HasForeignKey("WardId");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticeArrears", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.TaxPayer", "TaxPayer")
                        .WithMany()
                        .HasForeignKey("TaxpayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticeItem", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.DemandNotice", "DemandNotice")
                        .WithMany("DemandNoticeItem")
                        .HasForeignKey("DemandNoticeId");

                    b.HasOne("RemsNG.Data.Entities.Item", "Item")
                        .WithMany("DemandNoticeItem")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.TaxPayer", "TaxPayer")
                        .WithMany()
                        .HasForeignKey("TaxpayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticePaymentHistory", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Bank", "Bank")
                        .WithMany()
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticePenalty", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.TaxPayer", "TaxPayer")
                        .WithMany()
                        .HasForeignKey("TaxpayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.DemandNoticeTaxpayers", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.DemandNotice", "DemandNotice")
                        .WithMany()
                        .HasForeignKey("DnId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Domain", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.State", "State")
                        .WithMany()
                        .HasForeignKey("StateId");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Item", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Lcda", "Lcda")
                        .WithMany("Item")
                        .HasForeignKey("LcdaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.ItemPenalty", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Item", "Item")
                        .WithMany("Itempenalty")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Lcda", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Role", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Lcda", "Domain")
                        .WithMany("Role")
                        .HasForeignKey("DomainId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.RolePermission", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Sector", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Lcda", "Lcda")
                        .WithMany("Sector")
                        .HasForeignKey("LcdaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.State", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Country", "Country")
                        .WithMany("State")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Street", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Ward", "Ward")
                        .WithMany("Street")
                        .HasForeignKey("WardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.TaxPayer", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Address", "Address")
                        .WithOne()
                        .HasForeignKey("RemsNG.Data.Entities.TaxPayer", "AddressId");

                    b.HasOne("RemsNG.Data.Entities.Company", "Company")
                        .WithMany("TaxPayer")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.Street", "Street")
                        .WithMany("TaxPayer")
                        .HasForeignKey("StreetId");
                });

            modelBuilder.Entity("RemsNG.Data.Entities.UserDomain", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Domain", "Domain")
                        .WithMany("UserDomains")
                        .HasForeignKey("DomainId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.User", "User")
                        .WithMany("UserDomains")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.UserLcda", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Lcda", "Lcda")
                        .WithMany("UserLcdas")
                        .HasForeignKey("LgdaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.User", "User")
                        .WithMany("UserLcdas")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.UserRole", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemsNG.Data.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemsNG.Data.Entities.Ward", b =>
                {
                    b.HasOne("RemsNG.Data.Entities.Lcda", "Lcda")
                        .WithMany("Ward")
                        .HasForeignKey("LcdaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
