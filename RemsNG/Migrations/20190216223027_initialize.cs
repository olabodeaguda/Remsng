using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RemsNG.Migrations
{
    public partial class initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_address",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Addressnumber = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    Lcdaid = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    StreetId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_bank",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BankName = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_bank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_bank_lcda",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BankAccount = table.Column<string>(nullable: true),
                    BankId = table.Column<Guid>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_bank_lcda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_company",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    CompanyStatus = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    SectorId = table.Column<Guid>(nullable: true),
                    StreetId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_contactDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContactType = table.Column<string>(nullable: true),
                    ContactValue = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_contactDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_country",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CountryCode = table.Column<string>(nullable: true),
                    CountryName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandnotice",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BatchNo = table.Column<string>(nullable: true),
                    BillingYear = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DemandNoticeStatus = table.Column<string>(nullable: true),
                    IsUnbilled = table.Column<bool>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    Query = table.Column<string>(nullable: true),
                    StreetId = table.Column<Guid>(nullable: true),
                    WardId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandnotice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticeArrears",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AmountPaid = table.Column<decimal>(nullable: false),
                    ArrearsStatus = table.Column<string>(nullable: true),
                    BillingNo = table.Column<string>(nullable: true),
                    BillingYear = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    OriginatedYear = table.Column<int>(nullable: false),
                    TaxpayerId = table.Column<Guid>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticeArrears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_DemandNoticeDownloadHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BillingNumber = table.Column<string>(nullable: true),
                    Charges = table.Column<decimal>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    GrandTotal = table.Column<decimal>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DemandNoticeDownloadHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticePaymentHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    BankId = table.Column<Guid>(nullable: false),
                    BillingNumber = table.Column<string>(nullable: true),
                    Charges = table.Column<decimal>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    IsWaiver = table.Column<bool>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false),
                    PaymentMode = table.Column<string>(nullable: true),
                    PaymentStatus = table.Column<string>(nullable: true),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    SyncStatus = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticePaymentHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticeTaxpayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressName = table.Column<string>(nullable: true),
                    BillingNumber = table.Column<string>(nullable: true),
                    BillingYr = table.Column<int>(nullable: false),
                    CouncilTreasurerMobile = table.Column<string>(nullable: true),
                    CouncilTreasurerSigFilen = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DemandNoticeStatus = table.Column<string>(nullable: true),
                    DnId = table.Column<Guid>(nullable: false),
                    DomainName = table.Column<string>(nullable: true),
                    IsUnbilled = table.Column<bool>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LcdaAddress = table.Column<string>(nullable: true),
                    LcdaLogoFileName = table.Column<string>(nullable: true),
                    LcdaName = table.Column<string>(nullable: true),
                    LcdaState = table.Column<string>(nullable: true),
                    RevCoodinatorSigFilen = table.Column<string>(nullable: true),
                    TaxpayerId = table.Column<Guid>(nullable: false),
                    TaxpayersName = table.Column<string>(nullable: true),
                    WardName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticeTaxpayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_domain",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: true),
                    Datecreated = table.Column<DateTime>(nullable: false),
                    DomainCode = table.Column<string>(nullable: true),
                    DomainName = table.Column<string>(nullable: true),
                    DomainStatus = table.Column<string>(nullable: true),
                    DomainType = table.Column<string>(nullable: true),
                    StateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_domain", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_error",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    ErrorType = table.Column<string>(nullable: true),
                    Errorvalue = table.Column<string>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_error", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_images",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    ImgFilename = table.Column<string>(nullable: true),
                    ImgType = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_lcda",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false),
                    Charges = table.Column<decimal>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DomainId = table.Column<Guid>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LcdaCode = table.Column<string>(nullable: true),
                    LcdaName = table.Column<string>(nullable: true),
                    LcdaStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_lcda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_LcdaProperty",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LcdaId = table.Column<Guid>(nullable: false),
                    PropertyKey = table.Column<string>(nullable: true),
                    PropertyStatus = table.Column<string>(nullable: true),
                    PropertyValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_LcdaProperty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_permission",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PermissionDescription = table.Column<string>(nullable: true),
                    PermissionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_prepayment",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<decimal>(nullable: false),
                    datecreated = table.Column<DateTime>(nullable: false),
                    prepaymentStatus = table.Column<string>(nullable: true),
                    taxpayerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_prepayment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_taxpayerCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    TaxpayerCategoryName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_taxpayerCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Firstname = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    LockedOutEndDateUtc = table.Column<DateTime>(nullable: true),
                    Lockedoutenabled = table.Column<bool>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    UserStatus = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_state",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CountryId = table.Column<Guid>(nullable: false),
                    StateCode = table.Column<string>(nullable: true),
                    StateName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_state", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_state_tbl_country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "tbl_country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_item",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    ItemCode = table.Column<string>(nullable: true),
                    ItemDescription = table.Column<string>(nullable: true),
                    ItemStatus = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_item_tbl_lcda_LcdaId",
                        column: x => x.LcdaId,
                        principalTable: "tbl_lcda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_role",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DomainId = table.Column<Guid>(nullable: false),
                    RoleName = table.Column<string>(nullable: true),
                    RoleStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_role_tbl_lcda_DomainId",
                        column: x => x.DomainId,
                        principalTable: "tbl_lcda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_sector",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    Prefix = table.Column<string>(nullable: true),
                    SectorName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_sector", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_sector_tbl_lcda_LcdaId",
                        column: x => x.LcdaId,
                        principalTable: "tbl_lcda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_ward",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    WardName = table.Column<string>(nullable: true),
                    WardStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_ward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_ward_tbl_lcda_LcdaId",
                        column: x => x.LcdaId,
                        principalTable: "tbl_lcda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_userdomain",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    DomainId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_userdomain", x => new { x.UserId, x.DomainId });
                    table.ForeignKey(
                        name: "FK_tbl_userdomain_tbl_domain_DomainId",
                        column: x => x.DomainId,
                        principalTable: "tbl_domain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_userdomain_tbl_users_UserId",
                        column: x => x.UserId,
                        principalTable: "tbl_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_userlcda",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    LgdaId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_userlcda", x => new { x.UserId, x.LgdaId });
                    table.ForeignKey(
                        name: "FK_tbl_userlcda_tbl_lcda_LgdaId",
                        column: x => x.LgdaId,
                        principalTable: "tbl_lcda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_userlcda_tbl_users_UserId",
                        column: x => x.UserId,
                        principalTable: "tbl_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_companyItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    BillingYear = table.Column<int>(nullable: false),
                    CompanyStatus = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    TaxpayerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_companyItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_companyItem_tbl_item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "tbl_item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticeItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AmountPaid = table.Column<decimal>(nullable: false),
                    BillingNo = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DnTaxpayersDetailsId = table.Column<Guid>(nullable: false),
                    ItemAmount = table.Column<decimal>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    ItemName = table.Column<string>(nullable: true),
                    ItemStatus = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    TaxpayerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticeItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_demandNoticeItem_tbl_demandnotice_DnTaxpayersDetailsId",
                        column: x => x.DnTaxpayersDetailsId,
                        principalTable: "tbl_demandnotice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_demandNoticeItem_tbl_item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "tbl_item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_itempenalty",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Duration = table.Column<string>(nullable: true),
                    IsPercentage = table.Column<bool>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    PenaltyStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_itempenalty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_itempenalty_tbl_item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "tbl_item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_rolePermission",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(nullable: false),
                    PermissionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_rolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_tbl_rolePermission_tbl_permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "tbl_permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_rolePermission_tbl_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tbl_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_userRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_userRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_tbl_userRole_tbl_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "tbl_role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_userRole_tbl_users_UserId",
                        column: x => x.UserId,
                        principalTable: "tbl_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_street",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    NumberOfHouse = table.Column<int>(nullable: true),
                    StreetDescription = table.Column<string>(nullable: true),
                    StreetName = table.Column<string>(nullable: true),
                    StreetStatus = table.Column<string>(nullable: true),
                    WardId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_street", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_street_tbl_ward_WardId",
                        column: x => x.WardId,
                        principalTable: "tbl_ward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_taxPayer",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Firstname = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    StreetId = table.Column<Guid>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    TaxpayerStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_taxPayer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_taxPayer_tbl_company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "tbl_company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_taxPayer_tbl_street_StreetId",
                        column: x => x.StreetId,
                        principalTable: "tbl_street",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_companyItem_ItemId",
                table: "tbl_companyItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeItem_DnTaxpayersDetailsId",
                table: "tbl_demandNoticeItem",
                column: "DnTaxpayersDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeItem_ItemId",
                table: "tbl_demandNoticeItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_item_LcdaId",
                table: "tbl_item",
                column: "LcdaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_itempenalty_ItemId",
                table: "tbl_itempenalty",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_role_DomainId",
                table: "tbl_role",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_rolePermission_PermissionId",
                table: "tbl_rolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_sector_LcdaId",
                table: "tbl_sector",
                column: "LcdaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_state_CountryId",
                table: "tbl_state",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_street_WardId",
                table: "tbl_street",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_taxPayer_CompanyId",
                table: "tbl_taxPayer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_taxPayer_StreetId",
                table: "tbl_taxPayer",
                column: "StreetId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_userdomain_DomainId",
                table: "tbl_userdomain",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_userlcda_LgdaId",
                table: "tbl_userlcda",
                column: "LgdaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_userRole_RoleId",
                table: "tbl_userRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ward_LcdaId",
                table: "tbl_ward",
                column: "LcdaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_address");

            migrationBuilder.DropTable(
                name: "tbl_bank");

            migrationBuilder.DropTable(
                name: "tbl_bank_lcda");

            migrationBuilder.DropTable(
                name: "tbl_companyItem");

            migrationBuilder.DropTable(
                name: "tbl_contactDetail");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticeArrears");

            migrationBuilder.DropTable(
                name: "tbl_DemandNoticeDownloadHistory");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticeItem");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticePaymentHistory");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticeTaxpayers");

            migrationBuilder.DropTable(
                name: "tbl_error");

            migrationBuilder.DropTable(
                name: "tbl_images");

            migrationBuilder.DropTable(
                name: "tbl_itempenalty");

            migrationBuilder.DropTable(
                name: "tbl_LcdaProperty");

            migrationBuilder.DropTable(
                name: "tbl_prepayment");

            migrationBuilder.DropTable(
                name: "tbl_rolePermission");

            migrationBuilder.DropTable(
                name: "tbl_sector");

            migrationBuilder.DropTable(
                name: "tbl_state");

            migrationBuilder.DropTable(
                name: "tbl_taxPayer");

            migrationBuilder.DropTable(
                name: "tbl_taxpayerCategory");

            migrationBuilder.DropTable(
                name: "tbl_userdomain");

            migrationBuilder.DropTable(
                name: "tbl_userlcda");

            migrationBuilder.DropTable(
                name: "tbl_userRole");

            migrationBuilder.DropTable(
                name: "tbl_demandnotice");

            migrationBuilder.DropTable(
                name: "tbl_item");

            migrationBuilder.DropTable(
                name: "tbl_permission");

            migrationBuilder.DropTable(
                name: "tbl_country");

            migrationBuilder.DropTable(
                name: "tbl_company");

            migrationBuilder.DropTable(
                name: "tbl_street");

            migrationBuilder.DropTable(
                name: "tbl_domain");

            migrationBuilder.DropTable(
                name: "tbl_role");

            migrationBuilder.DropTable(
                name: "tbl_users");

            migrationBuilder.DropTable(
                name: "tbl_ward");

            migrationBuilder.DropTable(
                name: "tbl_lcda");
        }
    }
}
