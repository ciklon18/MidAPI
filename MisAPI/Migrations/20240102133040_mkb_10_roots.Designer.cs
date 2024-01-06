﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MisAPI.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MisAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240102133040_mkb_10_roots")]
    partial class mkb_10_roots
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MisAPI.Entities.Doctor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birthday");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("email");

                    b.Property<int>("Gender")
                        .HasColumnType("integer")
                        .HasColumnName("gender");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("password");

                    b.Property<string>("Phone")
                        .HasMaxLength(12)
                        .HasColumnType("character varying(12)")
                        .HasColumnName("phone");

                    b.Property<Guid>("SpecialityId")
                        .HasColumnType("uuid")
                        .HasColumnName("speciality_id");

                    b.HasKey("Id");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("MisAPI.Entities.Mkb10", b =>
                {
                    b.Property<int>("IdInt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id_int");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdInt"));

                    b.Property<string>("AddlCode")
                        .HasColumnType("text")
                        .HasColumnName("addl_code");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<int>("IdParent")
                        .HasColumnType("integer")
                        .HasColumnName("parent_id");

                    b.Property<Guid?>("IdUuid")
                        .HasColumnType("uuid")
                        .HasColumnName("id_uuid");

                    b.Property<string>("MkbCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("mkb_code");

                    b.Property<string>("MkbName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("mkb_name");

                    b.Property<string>("RecCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("rec_code");

                    b.Property<Guid?>("RootIdGuid")
                        .HasColumnType("uuid")
                        .HasColumnName("root_id_guid");

                    b.Property<int?>("RootIdInt")
                        .HasColumnType("integer")
                        .HasColumnName("root_id_int");

                    b.HasKey("IdInt");

                    b.ToTable("Mkb10");
                });

            modelBuilder.Entity("MisAPI.Entities.Mkb10Root", b =>
                {
                    b.Property<int>("KeyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("key_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("KeyId"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("KeyId");

                    b.ToTable("Mkb10Roots");
                });

            modelBuilder.Entity("MisAPI.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid")
                        .HasColumnName("doctorId");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires");

                    b.Property<bool>("Revoked")
                        .HasColumnType("boolean")
                        .HasColumnName("revoked");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("token");

                    b.HasKey("Id");

                    b.ToTable("Refresh tokens");
                });

            modelBuilder.Entity("MisAPI.Entities.Specialty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("Specialties");
                });
#pragma warning restore 612, 618
        }
    }
}
