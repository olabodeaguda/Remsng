using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemsNG.Migrations
{
    public partial class initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "tbl_batchDownloadRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BatchNo = table.Column<string>(nullable: true),
                    RequestStatus = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: true),
                    BatchFileName = table.Column<string>(nullable: true),
                    Createdby = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_batchDownloadRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_cloudData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DomainId = table.Column<Guid>(nullable: false),
                    DataTitle = table.Column<string>(nullable: true),
                    SyncStatus = table.Column<string>(nullable: true),
                    JsonContent = table.Column<string>(nullable: true),
                    BillingNumber = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_cloudData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_contactDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    ContactValue = table.Column<string>(nullable: true),
                    ContactType = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_contactDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_contactPerson",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Surname = table.Column<string>(nullable: true),
                    Firstname = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    TaxPayerId = table.Column<Guid>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_contactPerson", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_country",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CountryName = table.Column<string>(nullable: true),
                    CountryCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_DemandNoticeDownloadHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BillingNumber = table.Column<string>(nullable: true),
                    GrandTotal = table.Column<decimal>(nullable: false),
                    Charges = table.Column<decimal>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DemandNoticeDownloadHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_error",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ErrorType = table.Column<string>(nullable: true),
                    Errorvalue = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
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
                    ImgFilename = table.Column<string>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false),
                    ImgType = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_LcdaProperty",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PropertyKey = table.Column<string>(nullable: true),
                    PropertyValue = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    PropertyStatus = table.Column<string>(nullable: true)
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
                    PermissionName = table.Column<string>(nullable: true),
                    PermissionDescription = table.Column<string>(nullable: true)
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
                    taxpayerId = table.Column<Guid>(nullable: false),
                    amount = table.Column<decimal>(nullable: false),
                    datecreated = table.Column<DateTime>(nullable: false),
                    prepaymentStatus = table.Column<string>(nullable: true)
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
                    TaxpayerCategoryName = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
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
                    Email = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    LockedOutEndDateUtc = table.Column<DateTime>(nullable: true),
                    Lockedoutenabled = table.Column<bool>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    UserStatus = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Firstname = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticePaymentHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    BillingNumber = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Charges = table.Column<decimal>(nullable: false),
                    PaymentMode = table.Column<string>(nullable: true),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    BankId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    PaymentStatus = table.Column<string>(nullable: true),
                    SyncStatus = table.Column<bool>(nullable: false),
                    IsWaiver = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticePaymentHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_demandNoticePaymentHistory_tbl_bank_BankId",
                        column: x => x.BankId,
                        principalTable: "tbl_bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "tbl_company",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompanyName = table.Column<string>(nullable: true),
                    StreetId = table.Column<Guid>(nullable: true),
                    SectorId = table.Column<Guid>(nullable: true),
                    AddressId = table.Column<Guid>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: true),
                    CompanyStatus = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_company_tbl_taxpayerCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "tbl_taxpayerCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_domain",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DomainName = table.Column<string>(nullable: true),
                    DomainCode = table.Column<string>(nullable: true),
                    Datecreated = table.Column<DateTime>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: true),
                    DomainStatus = table.Column<string>(nullable: true),
                    DomainType = table.Column<string>(nullable: true),
                    StateId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_domain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_domain_tbl_state_StateId",
                        column: x => x.StateId,
                        principalTable: "tbl_state",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_lcda",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DomainId = table.Column<Guid>(nullable: false),
                    LcdaName = table.Column<string>(nullable: true),
                    LcdaCode = table.Column<string>(nullable: true),
                    AddressId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    LcdaStatus = table.Column<string>(nullable: true),
                    Charges = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_lcda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_lcda_tbl_domain_DomainId",
                        column: x => x.DomainId,
                        principalTable: "tbl_domain",
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
                name: "tbl_bank_lcda",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BankId = table.Column<Guid>(nullable: false),
                    LcdaId = table.Column<Guid>(nullable: false),
                    BankAccount = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_bank_lcda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_bank_lcda_tbl_bank_BankId",
                        column: x => x.BankId,
                        principalTable: "tbl_bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_bank_lcda_tbl_lcda_LcdaId",
                        column: x => x.LcdaId,
                        principalTable: "tbl_lcda",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_item",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ItemDescription = table.Column<string>(nullable: true),
                    ItemStatus = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    ItemCode = table.Column<string>(nullable: true)
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
                    RoleName = table.Column<string>(nullable: true),
                    DomainId = table.Column<Guid>(nullable: false),
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
                    SectorName = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    Prefix = table.Column<string>(nullable: true)
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
                name: "tbl_userlcda",
                columns: table => new
                {
                    LgdaId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
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
                name: "tbl_ward",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WardName = table.Column<string>(nullable: true),
                    LcdaId = table.Column<Guid>(nullable: false),
                    WardStatus = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
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
                name: "tbl_itempenalty",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    IsPercentage = table.Column<bool>(nullable: false),
                    PenaltyStatus = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Duration = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
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
                    RoleId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
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
                    WardId = table.Column<Guid>(nullable: false),
                    StreetName = table.Column<string>(nullable: true),
                    NumberOfHouse = table.Column<int>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    StreetStatus = table.Column<string>(nullable: true),
                    StreetDescription = table.Column<string>(nullable: true)
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
                name: "tbl_demandnotice",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Query = table.Column<string>(nullable: true),
                    PlainTextQuery = table.Column<string>(nullable: true),
                    BatchNo = table.Column<string>(nullable: true),
                    DemandNoticeStatus = table.Column<string>(nullable: true),
                    BillingYear = table.Column<int>(nullable: false),
                    LcdaId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    WardId = table.Column<Guid>(nullable: true),
                    StreetId = table.Column<Guid>(nullable: true),
                    IsUnbilled = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandnotice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_demandnotice_tbl_street_StreetId",
                        column: x => x.StreetId,
                        principalTable: "tbl_street",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_demandnotice_tbl_ward_WardId",
                        column: x => x.WardId,
                        principalTable: "tbl_ward",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticeTaxpayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DnId = table.Column<Guid>(nullable: false),
                    TaxpayerId = table.Column<Guid>(nullable: false),
                    TaxpayersName = table.Column<string>(nullable: true),
                    BillingNumber = table.Column<string>(nullable: true),
                    AddressName = table.Column<string>(nullable: true),
                    WardName = table.Column<string>(nullable: true),
                    LcdaName = table.Column<string>(nullable: true),
                    BillingYr = table.Column<int>(nullable: false),
                    DomainName = table.Column<string>(nullable: true),
                    LcdaAddress = table.Column<string>(nullable: true),
                    LcdaState = table.Column<string>(nullable: true),
                    LcdaLogoFileName = table.Column<string>(nullable: true),
                    CouncilTreasurerSigFilen = table.Column<string>(nullable: true),
                    RevCoodinatorSigFilen = table.Column<string>(nullable: true),
                    CouncilTreasurerMobile = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    DemandNoticeStatus = table.Column<string>(nullable: true),
                    IsUnbilled = table.Column<bool>(nullable: false),
                    Period = table.Column<int>(nullable: false),
                    IsRunArrears = table.Column<bool>(nullable: false),
                    IsRunPenalty = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticeTaxpayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_demandNoticeTaxpayers_tbl_demandnotice_DnId",
                        column: x => x.DnId,
                        principalTable: "tbl_demandnotice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_taxPayer",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    StreetId = table.Column<Guid>(nullable: true),
                    AddressId = table.Column<Guid>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    TaxpayerStatus = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Firstname = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "tbl_address",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Addressnumber = table.Column<string>(nullable: true),
                    StreetId = table.Column<Guid>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    OwnerId = table.Column<Guid>(nullable: false),
                    Lcdaid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_address_tbl_taxPayer_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "tbl_taxPayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_address_tbl_street_StreetId",
                        column: x => x.StreetId,
                        principalTable: "tbl_street",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_companyItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TaxpayerId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    BillingYear = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    CompanyStatus = table.Column<string>(nullable: true)
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
                    table.ForeignKey(
                        name: "FK_tbl_companyItem_tbl_taxPayer_TaxpayerId",
                        column: x => x.TaxpayerId,
                        principalTable: "tbl_taxPayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticeArrears",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BillingNo = table.Column<string>(nullable: true),
                    TaxpayerId = table.Column<Guid>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    CurrentAmount = table.Column<decimal>(nullable: false),
                    AmountPaid = table.Column<decimal>(nullable: false),
                    OriginatedYear = table.Column<int>(nullable: false),
                    BillingYear = table.Column<int>(nullable: false),
                    ArrearsStatus = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticeArrears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_demandNoticeArrears_tbl_taxPayer_TaxpayerId",
                        column: x => x.TaxpayerId,
                        principalTable: "tbl_taxPayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticeItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BillingNo = table.Column<string>(nullable: true),
                    dn_taxpayersDetailsId = table.Column<Guid>(nullable: false),
                    DemandNoticeId = table.Column<Guid>(nullable: false),
                    TaxpayerId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    ItemName = table.Column<string>(nullable: true),
                    ItemAmount = table.Column<decimal>(nullable: false),
                    AmountPaid = table.Column<decimal>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticeItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_demandNoticeItem_tbl_item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "tbl_item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_demandNoticeItem_tbl_taxPayer_TaxpayerId",
                        column: x => x.TaxpayerId,
                        principalTable: "tbl_taxPayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_demandNoticeItem_tbl_demandNoticeTaxpayers_dn_taxpayersDetailsId",
                        column: x => x.dn_taxpayersDetailsId,
                        principalTable: "tbl_demandNoticeTaxpayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_demandNoticePenalty",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BillingNo = table.Column<string>(nullable: true),
                    TaxpayerId = table.Column<Guid>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    AmountPaid = table.Column<decimal>(nullable: false),
                    OriginatedYear = table.Column<int>(nullable: false),
                    BillingYear = table.Column<int>(nullable: false),
                    ItemPenaltyStatus = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Lastmodifiedby = table.Column<string>(nullable: true),
                    LastModifiedDate = table.Column<DateTime>(nullable: true),
                    CurrentAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_demandNoticePenalty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_demandNoticePenalty_tbl_taxPayer_TaxpayerId",
                        column: x => x.TaxpayerId,
                        principalTable: "tbl_taxPayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_address_OwnerId",
                table: "tbl_address",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_address_StreetId",
                table: "tbl_address",
                column: "StreetId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bank_lcda_BankId",
                table: "tbl_bank_lcda",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bank_lcda_LcdaId",
                table: "tbl_bank_lcda",
                column: "LcdaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_company_CategoryId",
                table: "tbl_company",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_companyItem_ItemId",
                table: "tbl_companyItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_companyItem_TaxpayerId",
                table: "tbl_companyItem",
                column: "TaxpayerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandnotice_StreetId",
                table: "tbl_demandnotice",
                column: "StreetId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandnotice_WardId",
                table: "tbl_demandnotice",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeArrears_TaxpayerId",
                table: "tbl_demandNoticeArrears",
                column: "TaxpayerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeItem_ItemId",
                table: "tbl_demandNoticeItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeItem_TaxpayerId",
                table: "tbl_demandNoticeItem",
                column: "TaxpayerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeItem_dn_taxpayersDetailsId",
                table: "tbl_demandNoticeItem",
                column: "dn_taxpayersDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticePaymentHistory_BankId",
                table: "tbl_demandNoticePaymentHistory",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticePenalty_TaxpayerId",
                table: "tbl_demandNoticePenalty",
                column: "TaxpayerId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_demandNoticeTaxpayers_DnId",
                table: "tbl_demandNoticeTaxpayers",
                column: "DnId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_domain_StateId",
                table: "tbl_domain",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_item_LcdaId",
                table: "tbl_item",
                column: "LcdaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_itempenalty_ItemId",
                table: "tbl_itempenalty",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_lcda_DomainId",
                table: "tbl_lcda",
                column: "DomainId");

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
                name: "IX_tbl_taxPayer_AddressId",
                table: "tbl_taxPayer",
                column: "AddressId",
                unique: true,
                filter: "[AddressId] IS NOT NULL");

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

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_taxPayer_tbl_address_AddressId",
                table: "tbl_taxPayer",
                column: "AddressId",
                principalTable: "tbl_address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_address_tbl_taxPayer_OwnerId",
                table: "tbl_address");

            migrationBuilder.DropTable(
                name: "tbl_bank_lcda");

            migrationBuilder.DropTable(
                name: "tbl_batchDownloadRequest");

            migrationBuilder.DropTable(
                name: "tbl_cloudData");

            migrationBuilder.DropTable(
                name: "tbl_companyItem");

            migrationBuilder.DropTable(
                name: "tbl_contactDetail");

            migrationBuilder.DropTable(
                name: "tbl_contactPerson");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticeArrears");

            migrationBuilder.DropTable(
                name: "tbl_DemandNoticeDownloadHistory");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticeItem");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticePaymentHistory");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticePenalty");

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
                name: "tbl_userdomain");

            migrationBuilder.DropTable(
                name: "tbl_userlcda");

            migrationBuilder.DropTable(
                name: "tbl_userRole");

            migrationBuilder.DropTable(
                name: "tbl_demandNoticeTaxpayers");

            migrationBuilder.DropTable(
                name: "tbl_bank");

            migrationBuilder.DropTable(
                name: "tbl_item");

            migrationBuilder.DropTable(
                name: "tbl_permission");

            migrationBuilder.DropTable(
                name: "tbl_role");

            migrationBuilder.DropTable(
                name: "tbl_users");

            migrationBuilder.DropTable(
                name: "tbl_demandnotice");

            migrationBuilder.DropTable(
                name: "tbl_taxPayer");

            migrationBuilder.DropTable(
                name: "tbl_address");

            migrationBuilder.DropTable(
                name: "tbl_company");

            migrationBuilder.DropTable(
                name: "tbl_street");

            migrationBuilder.DropTable(
                name: "tbl_taxpayerCategory");

            migrationBuilder.DropTable(
                name: "tbl_ward");

            migrationBuilder.DropTable(
                name: "tbl_lcda");

            migrationBuilder.DropTable(
                name: "tbl_domain");

            migrationBuilder.DropTable(
                name: "tbl_state");

            migrationBuilder.DropTable(
                name: "tbl_country");
        }
    }
}
