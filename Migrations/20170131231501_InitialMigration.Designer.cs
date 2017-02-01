using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ABC.Leaves.Api.Models;
using ABC.Leaves.Api.Enums;

namespace ABC.Leaves.Api.Migrations
{
    [DbContext(typeof(EmployeeLeavingContext))]
    [Migration("20170131231501_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("LivitTest.Models.Employee", b =>
                {
                    b.Property<string>("GmailLogin")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Rights");

                    b.HasKey("GmailLogin");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("LivitTest.Models.EmployeeLeave", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("End");

                    b.Property<DateTime>("Start");

                    b.Property<int>("Status");

                    b.Property<string>("Token");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("EmployeeLeaves");
                });
        }
    }
}
