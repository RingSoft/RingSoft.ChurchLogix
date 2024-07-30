﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RingSoft.ChurchLogix.Sqlite;

#nullable disable

namespace RingSoft.ChurchLogix.Sqlite.Migrations
{
    [DbContext(typeof(ChurchLogixSqliteDbContext))]
    [Migration("20240730221716_FundNullToDefValue")]
    partial class FundNullToDefValue
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.Financial_Management.Fund", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<double>("Goal")
                        .HasColumnType("numeric");

                    b.Property<double>("TotalCollected")
                        .HasColumnType("numeric");

                    b.Property<double>("TotalSpent")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Funds");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.MemberManagement.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar");

                    b.Property<int?>("HouseholdId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("Notes")
                        .HasColumnType("ntext");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar");

                    b.HasKey("Id");

                    b.HasIndex("HouseholdId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.StaffManagement.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("Rights")
                        .HasColumnType("ntext");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.StaffManagement.StaffGroup", b =>
                {
                    b.Property<int>("StaffPersonId")
                        .HasColumnType("integer");

                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.HasKey("StaffPersonId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("StaffGroups");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.StaffManagement.StaffPerson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar");

                    b.Property<int?>("MemberId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("Notes")
                        .HasColumnType("ntext");

                    b.Property<string>("Password")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar");

                    b.Property<string>("Rights")
                        .HasColumnType("ntext");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("Staff");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.SystemMaster", b =>
                {
                    b.Property<string>("ChurchName")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("ChurchName");

                    b.ToTable("SystemMaster");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.SystemPreferences", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("SystemPreferences");
                });

            modelBuilder.Entity("RingSoft.DbLookup.AdvancedFind.AdvancedFind", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    b.Property<bool?>("Disabled")
                        .HasColumnType("bit");

                    b.Property<string>("FromFormula")
                        .HasColumnType("ntext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<int?>("RedAlert")
                        .HasColumnType("integer");

                    b.Property<byte?>("RefreshCondition")
                        .HasColumnType("smallint");

                    b.Property<byte?>("RefreshRate")
                        .HasColumnType("smallint");

                    b.Property<int?>("RefreshValue")
                        .HasColumnType("integer");

                    b.Property<string>("Table")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<int?>("YellowAlert")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("AdvancedFinds");
                });

            modelBuilder.Entity("RingSoft.DbLookup.AdvancedFind.AdvancedFindColumn", b =>
                {
                    b.Property<int>("AdvancedFindId")
                        .HasColumnType("integer");

                    b.Property<int>("ColumnId")
                        .HasColumnType("integer");

                    b.Property<string>("Caption")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar");

                    b.Property<byte>("DecimalFormatType")
                        .HasColumnType("smallint");

                    b.Property<byte>("FieldDataType")
                        .HasColumnType("smallint");

                    b.Property<string>("FieldName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("Formula")
                        .HasColumnType("ntext");

                    b.Property<string>("Path")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar");

                    b.Property<double>("PercentWidth")
                        .HasColumnType("numeric");

                    b.Property<string>("PrimaryFieldName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("PrimaryTableName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TableName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.HasKey("AdvancedFindId", "ColumnId");

                    b.ToTable("AdvancedFindColumns");
                });

            modelBuilder.Entity("RingSoft.DbLookup.AdvancedFind.AdvancedFindFilter", b =>
                {
                    b.Property<int>("AdvancedFindId")
                        .HasColumnType("integer");

                    b.Property<int>("FilterId")
                        .HasColumnType("integer");

                    b.Property<bool>("CustomDate")
                        .HasColumnType("bit");

                    b.Property<byte>("DateFilterType")
                        .HasColumnType("smallint");

                    b.Property<byte>("EndLogic")
                        .HasColumnType("smallint");

                    b.Property<string>("FieldName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("Formula")
                        .HasColumnType("ntext");

                    b.Property<byte>("FormulaDataType")
                        .HasColumnType("smallint");

                    b.Property<string>("FormulaDisplayValue")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<byte>("LeftParentheses")
                        .HasColumnType("smallint");

                    b.Property<byte>("Operand")
                        .HasColumnType("smallint");

                    b.Property<string>("Path")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar");

                    b.Property<string>("PrimaryFieldName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("PrimaryTableName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<byte>("RightParentheses")
                        .HasColumnType("smallint");

                    b.Property<int?>("SearchForAdvancedFindId")
                        .HasColumnType("integer");

                    b.Property<string>("SearchForValue")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TableName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.HasKey("AdvancedFindId", "FilterId");

                    b.HasIndex("SearchForAdvancedFindId");

                    b.ToTable("AdvancedFindFilters");
                });

            modelBuilder.Entity("RingSoft.DbLookup.RecordLocking.RecordLock", b =>
                {
                    b.Property<string>("Table")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<string>("PrimaryKey")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime>("LockDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("User")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar");

                    b.HasKey("Table", "PrimaryKey");

                    b.ToTable("RecordLocks");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.MemberManagement.Member", b =>
                {
                    b.HasOne("RingSoft.ChurchLogix.DataAccess.Model.MemberManagement.Member", "Household")
                        .WithMany("HouseholdMembers")
                        .HasForeignKey("HouseholdId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Household");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.StaffManagement.StaffGroup", b =>
                {
                    b.HasOne("RingSoft.ChurchLogix.DataAccess.Model.StaffManagement.Group", "Group")
                        .WithMany("StaffGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("RingSoft.ChurchLogix.DataAccess.Model.StaffManagement.StaffPerson", "StaffPerson")
                        .WithMany("Groups")
                        .HasForeignKey("StaffPersonId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("StaffPerson");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.StaffManagement.StaffPerson", b =>
                {
                    b.HasOne("RingSoft.ChurchLogix.DataAccess.Model.MemberManagement.Member", "Member")
                        .WithMany("Staff")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Member");
                });

            modelBuilder.Entity("RingSoft.DbLookup.AdvancedFind.AdvancedFindColumn", b =>
                {
                    b.HasOne("RingSoft.DbLookup.AdvancedFind.AdvancedFind", "AdvancedFind")
                        .WithMany("Columns")
                        .HasForeignKey("AdvancedFindId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("AdvancedFind");
                });

            modelBuilder.Entity("RingSoft.DbLookup.AdvancedFind.AdvancedFindFilter", b =>
                {
                    b.HasOne("RingSoft.DbLookup.AdvancedFind.AdvancedFind", "AdvancedFind")
                        .WithMany("Filters")
                        .HasForeignKey("AdvancedFindId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("RingSoft.DbLookup.AdvancedFind.AdvancedFind", "SearchForAdvancedFind")
                        .WithMany("SearchForAdvancedFindFilters")
                        .HasForeignKey("SearchForAdvancedFindId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("AdvancedFind");

                    b.Navigation("SearchForAdvancedFind");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.MemberManagement.Member", b =>
                {
                    b.Navigation("HouseholdMembers");

                    b.Navigation("Staff");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.StaffManagement.Group", b =>
                {
                    b.Navigation("StaffGroups");
                });

            modelBuilder.Entity("RingSoft.ChurchLogix.DataAccess.Model.StaffManagement.StaffPerson", b =>
                {
                    b.Navigation("Groups");
                });

            modelBuilder.Entity("RingSoft.DbLookup.AdvancedFind.AdvancedFind", b =>
                {
                    b.Navigation("Columns");

                    b.Navigation("Filters");

                    b.Navigation("SearchForAdvancedFindFilters");
                });
#pragma warning restore 612, 618
        }
    }
}
