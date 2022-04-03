using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SecretSanta_Backend.Models;

namespace SecretSanta_Backend
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<Member> Members { get; set; } = null!;
        public virtual DbSet<MemberEvent> MemberEvents { get; set; } = null!;

        /*       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                {
                    if (!optionsBuilder.IsConfigured)
                    {
        #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                        optionsBuilder.UseNpgsql("Server=ec2-99-80-170-190.eu-west-1.compute.amazonaws.com; Port=5432; Database=dau95sjnjljv1t; User Id=ydbbazjjlagdww; Password=4851b131d7c6d27d9c1c99840f49f1e93ae23e23ae2945024c1976ec589350fc;");
                    }
                }
        */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Address");

                entity.Property(e => e.Apartment)
                    .HasMaxLength(5)
                    .HasColumnName("APARTMENT");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .HasColumnName("CITY");

                entity.Property(e => e.MemberId).HasColumnName("MEMBER_ID");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .HasColumnName("PHONE_NUMBER");

                entity.Property(e => e.Preferences).HasColumnName("PREFERENCES");

                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .HasColumnName("REGION");

                entity.Property(e => e.Street)
                    .HasMaxLength(50)
                    .HasColumnName("STREET");

                entity.Property(e => e.Zip)
                    .HasMaxLength(50)
                    .HasColumnName("ZIP");

                entity.HasOne(d => d.Member)
                    .WithMany()
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Address_Member_1");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Description)
                    .HasMaxLength(140)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.Endofevent).HasColumnName("ENDOFEVENT");

                entity.Property(e => e.Endofregistration).HasColumnName("ENDOFREGISTRATION");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("Member");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Attend)
                    .HasColumnName("ATTEND")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("NAME");

                entity.Property(e => e.Patronymic)
                    .HasMaxLength(50)
                    .HasColumnName("PATRONYMIC");

                entity.Property(e => e.Surname)
                    .HasMaxLength(50)
                    .HasColumnName("SURNAME");
            });

            modelBuilder.Entity<MemberEvent>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Member_Event");

                entity.Property(e => e.EventId).HasColumnName("EVENT_ID");

                entity.Property(e => e.MemberId).HasColumnName("MEMBER_ID");

                entity.HasOne(d => d.Event)
                    .WithMany()
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Member_Event_Event_1");

                entity.HasOne(d => d.Member)
                    .WithMany()
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Member_Event_Member_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
