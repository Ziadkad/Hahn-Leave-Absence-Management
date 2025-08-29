using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HahnLeaveAbsenceManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeavesLeft = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BusinessDays = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Email", "FirstName", "LastName", "LeavesLeft", "ModifiedBy", "ModifiedOn", "Password", "Role" },
                values: new object[,]
                {
                    { new Guid("10ddb1bd-7956-9c97-a82b-28edd1c8ccd2"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "youssef.elfassi@hahn.local", "Youssef", "ElFassi", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 0 },
                    { new Guid("1242bf85-0669-40ef-724b-eef922556dda"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "imane.mouline@hahn.local", "Imane", "Mouline", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("35a56cc6-18e2-2e49-3239-5c878db167bc"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "hajar.bouazza@hahn.local", "Hajar", "Bouazza", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("46199b05-5208-7a6e-96a1-b1fa675af8aa"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "meriem.elmalki@hahn.local", "Meriem", "ElMalki", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("48d9ad4a-2f00-29d2-715f-8c6e02a191f7"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "anas.jebbar@hahn.local", "Anas", "Jebbar", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("5815c2a8-ad8d-03a6-a265-18f65c4882cc"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nadia.zerhouni@hahn.local", "Nadia", "Zerhouni", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("6998a4fe-7fc2-590e-616d-4ad24a81a6db"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "omar.amrani@hahn.local", "Omar", "Amrani", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 0 },
                    { new Guid("75b52741-41e2-640a-cac3-2a2a63457b4e"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "salma.alaoui@hahn.local", "Salma", "Alaoui", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("84ad3fd0-1151-9e9a-71dc-3eaf99e4bdbb"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "yassin.berrada@hahn.local", "Yassin", "Berrada", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("8e91a67b-8c05-e6b5-8801-02477295103f"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "adil.tazi@hahn.local", "Adil", "Tazi", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("901436a1-5810-6631-74fd-382120a6441b"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "reda.elidrissi@hahn.local", "Reda", "ElIdrissi", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("c298b7fb-4152-0102-2e7b-d22eef00e52d"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sara.azli@hahn.local", "Sara", "Azli", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 0 },
                    { new Guid("dbed805b-a6c2-4a54-534d-b61eff529976"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "lina.farissi@hahn.local", "Lina", "Farissi", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 },
                    { new Guid("e8b8f1f5-85a8-adb8-01a7-17783f381411"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "amina.bennani@hahn.local", "Amina", "Bennani", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 0 },
                    { new Guid("ed667da6-b4f7-3881-3cd6-96c44c15e821"), new Guid("80161798-9708-1a26-fadd-41f994fb23d8"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "karim.chafik@hahn.local", "Karim", "Chafik", 18, null, null, "$2a$10$7EqJtq98hPqEX7fNZaFWo.HWRHsGM68cVlfJgnEZJa3yDI//qK9pC", 1 }
                });

            migrationBuilder.InsertData(
                table: "LeaveRequests",
                columns: new[] { "Id", "BusinessDays", "Description", "EndDate", "StartDate", "Status", "Type", "UserId" },
                values: new object[,]
                {
                    { new Guid("099fca96-169d-8434-84ad-c5db6c657356"), 2, "", new DateTime(2025, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("901436a1-5810-6631-74fd-382120a6441b") },
                    { new Guid("0fd9aa2e-2384-104a-6760-b25b4f591102"), 4, "", new DateTime(2024, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("8e91a67b-8c05-e6b5-8801-02477295103f") },
                    { new Guid("1e6ed0df-2f49-2f41-88da-77b17348a03e"), 4, "", new DateTime(2025, 8, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, new Guid("1242bf85-0669-40ef-724b-eef922556dda") },
                    { new Guid("2105129e-a021-0e81-42ce-a5f218b31df8"), 2, "", new DateTime(2024, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("48d9ad4a-2f00-29d2-715f-8c6e02a191f7") },
                    { new Guid("21f3c03d-58e2-93e2-f6b4-eac22895e8a2"), 2, "", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("c298b7fb-4152-0102-2e7b-d22eef00e52d") },
                    { new Guid("2418c289-887d-27b6-7033-d8acef5e94b4"), 3, "", new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new Guid("1242bf85-0669-40ef-724b-eef922556dda") },
                    { new Guid("32e4ba2d-52d5-8732-8db2-c1d7e14650ad"), 1, "", new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("5815c2a8-ad8d-03a6-a265-18f65c4882cc") },
                    { new Guid("3cf26427-c262-15b2-73f2-86a008306876"), 2, "", new DateTime(2024, 3, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, new Guid("6998a4fe-7fc2-590e-616d-4ad24a81a6db") },
                    { new Guid("40015c8b-5d97-9b4f-54ed-23c786c39f44"), 1, "", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("e8b8f1f5-85a8-adb8-01a7-17783f381411") },
                    { new Guid("40de25b3-8a46-9154-5442-acb43709e63e"), 5, "", new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, new Guid("1242bf85-0669-40ef-724b-eef922556dda") },
                    { new Guid("4925c542-7440-ae33-32cd-594be6e7d1c5"), 4, "", new DateTime(2025, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, new Guid("dbed805b-a6c2-4a54-534d-b61eff529976") },
                    { new Guid("4be74fd0-d8ed-6169-c818-e0f85c649444"), 4, "", new DateTime(2025, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("901436a1-5810-6631-74fd-382120a6441b") },
                    { new Guid("52b8fafa-2b49-e765-8d62-fedf0aa81459"), 2, "", new DateTime(2025, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, new Guid("ed667da6-b4f7-3881-3cd6-96c44c15e821") },
                    { new Guid("55009f39-77da-87a5-5874-f776a37e2ee9"), 2, "", new DateTime(2025, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new Guid("10ddb1bd-7956-9c97-a82b-28edd1c8ccd2") },
                    { new Guid("5772cf27-e19a-34bb-4bde-bf4210b3e191"), 5, "", new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new Guid("46199b05-5208-7a6e-96a1-b1fa675af8aa") },
                    { new Guid("5e3459e3-58d1-b74b-b105-87d329d945ed"), 2, "", new DateTime(2025, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, new Guid("10ddb1bd-7956-9c97-a82b-28edd1c8ccd2") },
                    { new Guid("681e6113-e817-d27f-9ee6-53dc45627751"), 2, "", new DateTime(2025, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, new Guid("901436a1-5810-6631-74fd-382120a6441b") },
                    { new Guid("6d0cafb4-7233-4778-c7c6-7d6db9f4d7a1"), 5, "", new DateTime(2025, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, new Guid("c298b7fb-4152-0102-2e7b-d22eef00e52d") },
                    { new Guid("71e37e6d-a15c-f71c-da91-022f4b5ce6d9"), 3, "", new DateTime(2025, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, new Guid("10ddb1bd-7956-9c97-a82b-28edd1c8ccd2") },
                    { new Guid("73c53d02-dfb3-49fd-4f7c-f4f028aae933"), 2, "", new DateTime(2025, 8, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, new Guid("84ad3fd0-1151-9e9a-71dc-3eaf99e4bdbb") },
                    { new Guid("788f62fa-0dde-ce2f-f0ba-9f33f2ebf9ed"), 3, "", new DateTime(2025, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, new Guid("46199b05-5208-7a6e-96a1-b1fa675af8aa") },
                    { new Guid("79bfb531-45d9-e2db-50f9-ba0aa2430abf"), 3, "", new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("5815c2a8-ad8d-03a6-a265-18f65c4882cc") },
                    { new Guid("7f643d88-f6e7-c304-720e-6dae86fad544"), 4, "", new DateTime(2025, 9, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, new Guid("48d9ad4a-2f00-29d2-715f-8c6e02a191f7") },
                    { new Guid("858d04c3-0272-6dca-d4f7-c428fc00ee5f"), 3, "", new DateTime(2025, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new Guid("35a56cc6-18e2-2e49-3239-5c878db167bc") },
                    { new Guid("8b672f4e-433b-e81a-e434-c602ee8417b2"), 4, "", new DateTime(2025, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("5815c2a8-ad8d-03a6-a265-18f65c4882cc") },
                    { new Guid("97c07f4c-c246-0bbe-4c95-0cad44a6c68c"), 5, "", new DateTime(2025, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, new Guid("75b52741-41e2-640a-cac3-2a2a63457b4e") },
                    { new Guid("999233c5-2d92-9ee4-e627-d9a6d813d827"), 1, "", new DateTime(2024, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new Guid("84ad3fd0-1151-9e9a-71dc-3eaf99e4bdbb") },
                    { new Guid("a2b9402b-5443-0b15-c2f6-33c7b672cbcc"), 3, "", new DateTime(2025, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, new Guid("8e91a67b-8c05-e6b5-8801-02477295103f") },
                    { new Guid("af6db838-9a1d-29b8-9ec9-54e02ede3519"), 2, "", new DateTime(2025, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("6998a4fe-7fc2-590e-616d-4ad24a81a6db") },
                    { new Guid("b47be7e3-2dab-5d8c-50ab-82306e6d8a3d"), 5, "", new DateTime(2025, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("84ad3fd0-1151-9e9a-71dc-3eaf99e4bdbb") },
                    { new Guid("b5cf61ec-ec4a-d6f0-463f-62b38f4b87a2"), 4, "", new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, new Guid("c298b7fb-4152-0102-2e7b-d22eef00e52d") },
                    { new Guid("b7ac52b4-a393-2088-0636-2e113d78fecc"), 3, "", new DateTime(2025, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("46199b05-5208-7a6e-96a1-b1fa675af8aa") },
                    { new Guid("b821d0c1-ea29-3fae-ad8a-2b06a03fded0"), 3, "", new DateTime(2025, 8, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, new Guid("ed667da6-b4f7-3881-3cd6-96c44c15e821") },
                    { new Guid("c6d763d2-c5e6-7fae-8c06-03b9dbd42bf0"), 3, "", new DateTime(2024, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new Guid("35a56cc6-18e2-2e49-3239-5c878db167bc") },
                    { new Guid("ca4ecb07-c0c9-b812-96d1-92bcd8e0e5fa"), 2, "", new DateTime(2025, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, new Guid("ed667da6-b4f7-3881-3cd6-96c44c15e821") },
                    { new Guid("d380bc92-c66f-b9a5-eadb-efa05e226d49"), 5, "", new DateTime(2025, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("6998a4fe-7fc2-590e-616d-4ad24a81a6db") },
                    { new Guid("d6724145-cc1d-9de0-f0b1-e3536194baf5"), 1, "", new DateTime(2025, 9, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("e8b8f1f5-85a8-adb8-01a7-17783f381411") },
                    { new Guid("d82c532e-7d1a-48b8-ac9d-324d6b625dcc"), 2, "", new DateTime(2025, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("e8b8f1f5-85a8-adb8-01a7-17783f381411") },
                    { new Guid("d8e7f50d-c4f1-89c1-9396-70003be68ff1"), 5, "", new DateTime(2025, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new Guid("75b52741-41e2-640a-cac3-2a2a63457b4e") },
                    { new Guid("def128f6-ad9a-1140-bed5-e9db8c6c81f8"), 2, "", new DateTime(2025, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, new Guid("8e91a67b-8c05-e6b5-8801-02477295103f") },
                    { new Guid("df4ca9bc-2a84-3d48-cef6-af1e94ab3abf"), 3, "", new DateTime(2025, 8, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new Guid("48d9ad4a-2f00-29d2-715f-8c6e02a191f7") },
                    { new Guid("ea18c0d5-94fe-58bd-4044-5874ada6669b"), 2, "", new DateTime(2025, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, new Guid("dbed805b-a6c2-4a54-534d-b61eff529976") },
                    { new Guid("ec4fe716-5cd7-1838-3b82-216c071d4bf3"), 1, "", new DateTime(2024, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, new Guid("dbed805b-a6c2-4a54-534d-b61eff529976") },
                    { new Guid("edca6b22-c94a-e69f-e3c0-71e1fa383371"), 1, "", new DateTime(2025, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("75b52741-41e2-640a-cac3-2a2a63457b4e") },
                    { new Guid("f49d5baf-7ba6-ee9d-83ef-0bc9cf7a9ec7"), 5, "", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, new Guid("35a56cc6-18e2-2e49-3239-5c878db167bc") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_UserId",
                table: "LeaveRequests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
