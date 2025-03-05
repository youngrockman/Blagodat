using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace demo_hard.Models;

public partial class User2Context : DbContext
{
    public User2Context()
    {
    }

    public User2Context(DbContextOptions<User2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<LastEnter> LastEnters { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=45.67.56.214;Port=5454;USERNAME=user2;DATABASE=user2;Password=hGcLvi0i");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("clients_pk");

            entity.ToTable("clients");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.ClientCode).HasColumnName("client_code");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.Fio)
                .HasColumnType("character varying")
                .HasColumnName("fio");
            entity.Property(e => e.Passport)
                .HasColumnType("character varying")
                .HasColumnName("passport");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeId).HasName("employees_pk");

            entity.ToTable("employees");

            entity.Property(e => e.EmployeId)
                .ValueGeneratedNever()
                .HasColumnName("employe_id");
            entity.Property(e => e.EmployeLogin)
                .HasColumnType("character varying")
                .HasColumnName("employe_login");
            entity.Property(e => e.EmployePassword)
                .HasColumnType("character varying")
                .HasColumnName("employe_password");
            entity.Property(e => e.EmployePhoto)
                .HasColumnType("character varying")
                .HasColumnName("employe_photo");
            entity.Property(e => e.Fio)
                .HasColumnType("character varying")
                .HasColumnName("fio");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
        });

        modelBuilder.Entity<LastEnter>(entity =>
        {
            entity.HasKey(e => e.EmployeId).HasName("last_enter_pk");

            entity.ToTable("last_enter");

            entity.Property(e => e.EmployeId)
                .ValueGeneratedNever()
                .HasColumnName("employe_id");
            entity.Property(e => e.EnterDatetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enter_datetime");
            entity.Property(e => e.EnterType)
                .HasColumnType("character varying")
                .HasColumnName("enter_type");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pk");

            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientCode).HasColumnName("client_code");
            entity.Property(e => e.DateClose).HasColumnName("date_close");
            entity.Property(e => e.OrderCode)
                .HasColumnType("character varying")
                .HasColumnName("order_code");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.OrderTime).HasColumnName("order_time");
            entity.Property(e => e.RentalTime).HasColumnName("rental_time");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pk");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasColumnType("character varying")
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_pk");

            entity.ToTable("service");

            entity.HasIndex(e => e.ServiceCode, "service_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ServiceCode)
                .HasColumnType("character varying")
                .HasColumnName("service_code");
            entity.Property(e => e.ServiceCost)
                .HasColumnType("character varying")
                .HasColumnName("service_cost");
            entity.Property(e => e.ServiceName)
                .HasColumnType("character varying")
                .HasColumnName("service_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
