﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MisAPI.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MisAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MisAPI.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid")
                        .HasColumnName("author_id");

                    b.Property<Guid?>("CommentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConsultationId")
                        .HasColumnType("uuid")
                        .HasColumnName("consultation_id");

                    b.Property<string>("Content")
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<DateTime>("ModifyTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("modify_time");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid")
                        .HasColumnName("parent_id");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CommentId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("MisAPI.Entities.Consultation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("CommentsNumber")
                        .HasColumnType("integer")
                        .HasColumnName("comments_number");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<Guid>("InspectionId")
                        .HasColumnType("uuid")
                        .HasColumnName("inspection_id");

                    b.Property<Guid>("RootCommentId")
                        .HasColumnType("uuid")
                        .HasColumnName("root_comment_id");

                    b.Property<Guid>("SpecialityId")
                        .HasColumnType("uuid")
                        .HasColumnName("speciality_id");

                    b.HasKey("Id");

                    b.HasIndex("InspectionId");

                    b.HasIndex("RootCommentId")
                        .IsUnique();

                    b.HasIndex("SpecialityId");

                    b.ToTable("Consultations");
                });

            modelBuilder.Entity("MisAPI.Entities.Diagnosis", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<Guid>("IcdDiagnosisId")
                        .HasColumnType("uuid")
                        .HasColumnName("icd_diagnosis_id");

                    b.Property<Guid>("InspectionId")
                        .HasColumnType("uuid")
                        .HasColumnName("inspection_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.HasIndex("InspectionId");

                    b.ToTable("Diagnoses");
                });

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

                    b.HasIndex("SpecialityId");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("MisAPI.Entities.Icd10", b =>
                {
                    b.Property<Guid?>("IdGuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id_guid");

                    b.Property<string>("AddlCode")
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)")
                        .HasColumnName("addl_code");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<string>("IcdCode")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)")
                        .HasColumnName("icd_code");

                    b.Property<string>("IcdName")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)")
                        .HasColumnName("icd_name");

                    b.Property<int>("IdInt")
                        .HasColumnType("integer")
                        .HasColumnName("id_int");

                    b.Property<int>("IdParent")
                        .HasColumnType("integer")
                        .HasColumnName("parent_id");

                    b.Property<string>("RecCode")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)")
                        .HasColumnName("rec_code");

                    b.Property<Guid?>("RootIdGuid")
                        .HasColumnType("uuid")
                        .HasColumnName("root_id_guid");

                    b.Property<int?>("RootIdInt")
                        .HasColumnType("integer")
                        .HasColumnName("root_id_int");

                    b.HasKey("IdGuid");

                    b.ToTable("Icd10");
                });

            modelBuilder.Entity("MisAPI.Entities.Icd10Root", b =>
                {
                    b.Property<int>("KeyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("key_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("KeyId"));

                    b.Property<string>("Code")
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

                    b.ToTable("Icd10Roots");
                });

            modelBuilder.Entity("MisAPI.Entities.Inspection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Anamnesis")
                        .HasColumnType("text")
                        .HasColumnName("anamnesis");

                    b.Property<Guid?>("BaseInspectionId")
                        .HasColumnType("uuid")
                        .HasColumnName("base_inspection_id");

                    b.Property<string>("Complaints")
                        .HasColumnType("text")
                        .HasColumnName("complaints");

                    b.Property<int>("Conclusion")
                        .HasColumnType("integer")
                        .HasColumnName("conclusion");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<DateTime?>("DeathDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("death_date");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid")
                        .HasColumnName("doctor_id");

                    b.Property<DateTime?>("NextVisitDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("next_visit_date");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uuid")
                        .HasColumnName("patient_id");

                    b.Property<Guid?>("PreviousInspectionId")
                        .HasColumnType("uuid")
                        .HasColumnName("previous_inspection_id");

                    b.Property<string>("Treatment")
                        .HasColumnType("text")
                        .HasColumnName("treatment");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.ToTable("Inspections");
                });

            modelBuilder.Entity("MisAPI.Entities.Patient", b =>
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

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uuid")
                        .HasColumnName("doctor_id");

                    b.Property<int>("Gender")
                        .HasColumnType("integer")
                        .HasColumnName("gender");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("Patients");
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

                    b.HasIndex("DoctorId");

                    b.ToTable("Refresh tokens");
                });

            modelBuilder.Entity("MisAPI.Entities.Speciality", b =>
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

                    b.ToTable("Specialities");
                });

            modelBuilder.Entity("MisAPI.Entities.Comment", b =>
                {
                    b.HasOne("MisAPI.Entities.Doctor", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MisAPI.Entities.Comment", null)
                        .WithMany("Children")
                        .HasForeignKey("CommentId");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("MisAPI.Entities.Consultation", b =>
                {
                    b.HasOne("MisAPI.Entities.Inspection", "Inspection")
                        .WithMany("Consultations")
                        .HasForeignKey("InspectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MisAPI.Entities.Comment", "RootComment")
                        .WithOne("Consultation")
                        .HasForeignKey("MisAPI.Entities.Consultation", "RootCommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MisAPI.Entities.Speciality", "Speciality")
                        .WithMany()
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inspection");

                    b.Navigation("RootComment");

                    b.Navigation("Speciality");
                });

            modelBuilder.Entity("MisAPI.Entities.Diagnosis", b =>
                {
                    b.HasOne("MisAPI.Entities.Inspection", "Inspection")
                        .WithMany("Diagnoses")
                        .HasForeignKey("InspectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Inspection");
                });

            modelBuilder.Entity("MisAPI.Entities.Doctor", b =>
                {
                    b.HasOne("MisAPI.Entities.Speciality", "Speciality")
                        .WithMany("Doctors")
                        .HasForeignKey("SpecialityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Speciality");
                });

            modelBuilder.Entity("MisAPI.Entities.Inspection", b =>
                {
                    b.HasOne("MisAPI.Entities.Doctor", "Doctor")
                        .WithMany("Inspections")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MisAPI.Entities.Patient", "Patient")
                        .WithMany("Inspections")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("MisAPI.Entities.Patient", b =>
                {
                    b.HasOne("MisAPI.Entities.Doctor", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");
                });

            modelBuilder.Entity("MisAPI.Entities.RefreshToken", b =>
                {
                    b.HasOne("MisAPI.Entities.Doctor", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");
                });

            modelBuilder.Entity("MisAPI.Entities.Comment", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Consultation")
                        .IsRequired();
                });

            modelBuilder.Entity("MisAPI.Entities.Doctor", b =>
                {
                    b.Navigation("Inspections");
                });

            modelBuilder.Entity("MisAPI.Entities.Inspection", b =>
                {
                    b.Navigation("Consultations");

                    b.Navigation("Diagnoses");
                });

            modelBuilder.Entity("MisAPI.Entities.Patient", b =>
                {
                    b.Navigation("Inspections");
                });

            modelBuilder.Entity("MisAPI.Entities.Speciality", b =>
                {
                    b.Navigation("Doctors");
                });
#pragma warning restore 612, 618
        }
    }
}
