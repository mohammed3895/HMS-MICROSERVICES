using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Staff.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffType = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmploymentStatus = table.Column<int>(type: "int", nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Qualifications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    ShiftType = table.Column<int>(type: "int", nullable: false),
                    WorkSchedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeeklyWorkHours = table.Column<int>(type: "int", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactRelationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Languages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaffAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CheckOutTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffAttendances_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuingOrganization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificateNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NeverExpires = table.Column<bool>(type: "bit", nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffCertifications_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffEducations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartYear = table.Column<int>(type: "int", nullable: false),
                    EndYear = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffEducations_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Organization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrentPosition = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffExperiences_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffLeaves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffLeaves_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffPerformanceReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewPeriod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverallRating = table.Column<int>(type: "int", nullable: false),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreasForImprovement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Goals = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPerformanceReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffPerformanceReviews_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Staff_Department",
                table: "Staff",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_IsActive_IsDeleted",
                table: "Staff",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Staff_StaffNumber",
                table: "Staff",
                column: "StaffNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staff_StaffType",
                table: "Staff",
                column: "StaffType");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAttendances_Date",
                table: "StaffAttendances",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAttendances_StaffId",
                table: "StaffAttendances",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffCertifications_StaffId",
                table: "StaffCertifications",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffEducations_StaffId",
                table: "StaffEducations",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffExperiences_StaffId",
                table: "StaffExperiences",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffLeaves_StaffId",
                table: "StaffLeaves",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffLeaves_Status",
                table: "StaffLeaves",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StaffPerformanceReviews_StaffId",
                table: "StaffPerformanceReviews",
                column: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffAttendances");

            migrationBuilder.DropTable(
                name: "StaffCertifications");

            migrationBuilder.DropTable(
                name: "StaffEducations");

            migrationBuilder.DropTable(
                name: "StaffExperiences");

            migrationBuilder.DropTable(
                name: "StaffLeaves");

            migrationBuilder.DropTable(
                name: "StaffPerformanceReviews");

            migrationBuilder.DropTable(
                name: "Staff");
        }
    }
}
