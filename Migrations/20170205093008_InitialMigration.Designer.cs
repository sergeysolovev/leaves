using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.Enums;

namespace LeavesApi.Migrations
{
    [DbContext(typeof(EmployeeLeavingContext))]
    [Migration("20170205093008_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("ABC.Leaves.Api.Models.Employee", b =>
                {
                    b.Property<string>("GmailLogin")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAdmin");

                    b.HasKey("GmailLogin");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("ABC.Leaves.Api.Models.EmployeeLeave", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("End");

                    b.Property<string>("GoogleAuthAccessToken");

                    b.Property<DateTime>("Start");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("EmployeeLeaves");
                });
        }
    }
}
