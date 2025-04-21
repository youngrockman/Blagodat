using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace demo_hard.Models;

public partial class User15Context : DbContext
{
    public User15Context()
    {
    }

    public User15Context(DbContextOptions<User15Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Service> Services { get; set; }
    public virtual DbSet<OrderService> OrderServices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=45.67.56.214;Port=5421;USERNAME=user15;DATABASE=user15;Password=3XkvwMOb");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("client_pk");
            entity.ToTable("client");
            entity.HasIndex(e => e.ClientCode, "client_unique").IsUnique();

            entity.Property(e => e.ClientId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("client_id");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.ClientCode).HasColumnName("client_code");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.Fio)
                .HasMaxLength(100)
                .HasColumnName("fio");
            entity.Property(e => e.Passport)
                .HasMaxLength(10)
                .HasColumnName("passport");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employees_pk");
            entity.ToTable("employees");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EnterStatus)
                .HasColumnType("character varying")
                .HasColumnName("enter_status");
            entity.Property(e => e.Fio)
                .HasColumnType("character varying")
                .HasColumnName("fio");
            entity.Property(e => e.LastEnter)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_enter");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Photo)
                .HasColumnType("character varying")
                .HasColumnName("photo");
            entity.Property(e => e.Role).HasColumnName("role");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pk");
            entity.ToTable("orders");

            entity.Property(e => e.OrderId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("order_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.OrderCode)
                .HasColumnType("character varying")
                .HasColumnName("order_code");
            entity.Property(e => e.RentTime).HasColumnName("rent_time");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.Time).HasColumnName("time");

            entity.HasMany(o => o.OrderServices)
                  .WithOne(os => os.Order)
                  .HasForeignKey(os => os.OrderId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasNoKey().ToTable("roles");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasColumnType("character varying")
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("services_pk");
            entity.ToTable("services");
            entity.HasIndex(e => e.ServiceCode, "services_unique").IsUnique();

            entity.Property(e => e.ServiceId)
                .ValueGeneratedNever()
                .HasColumnName("service_id");
            entity.Property(e => e.CostPerHour).HasColumnName("cost_per_hour");
            entity.Property(e => e.ServiceCode)
                .HasColumnType("character varying")
                .HasColumnName("service_code");
            entity.Property(e => e.ServiceName)
                .HasColumnType("character varying")
                .HasColumnName("service_name");

            entity.HasMany(s => s.OrderServices)
                  .WithOne(os => os.Service)
                  .HasForeignKey(os => os.ServiceId);
        });

        modelBuilder.Entity<OrderService>(entity =>
        {
            entity.HasKey(os => new { os.OrderId, os.ServiceId });
            entity.ToTable("order_services");

            entity.Property(os => os.OrderId).HasColumnName("order_id");
            entity.Property(os => os.ServiceId).HasColumnName("service_id");
            entity.Property(os => os.RentTime).HasColumnName("rent_time");

            entity.HasOne(os => os.Order)
                  .WithMany(o => o.OrderServices)
                  .HasForeignKey(os => os.OrderId)
                  .HasConstraintName("order_services_order_id_fkey");

            entity.HasOne(os => os.Service)
                  .WithMany(s => s.OrderServices)
                  .HasForeignKey(os => os.ServiceId)
                  .HasConstraintName("order_services_service_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}