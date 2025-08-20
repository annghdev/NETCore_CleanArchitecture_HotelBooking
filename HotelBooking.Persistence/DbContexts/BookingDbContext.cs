using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.DbContexts;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Booking> Bookings { get; set; }
    public virtual DbSet<PricingPolicy> PricingPolicies { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<UserPermission> UserPermissions { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<PaymentTransaction> Payments { get; set; }
    public virtual DbSet<UserTokens> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>(opt =>
        {
            opt.HasKey(p => p.Id);
        });
        modelBuilder.Entity<User>(opt =>
        {
            opt.HasKey(p => p.Id);
        });
        modelBuilder.Entity<Role>(opt =>
        {
            opt.HasKey(p => p.Id);
        });
        modelBuilder.Entity<Customer>(opt =>
        {
            opt.HasKey(p => p.Id);
        });
        modelBuilder.Entity<PricingPolicy>(opt =>
        {
            opt.HasKey(p => p.Id);
        });
        modelBuilder.Entity<UserPermission>(opt =>
        {
            opt.HasKey(p => new { p.UserId, p.PermissionId });
        });
        modelBuilder.Entity<UserRole>(opt =>
        {
            opt.HasKey(p => new { p.UserId, p.RoleId });
        });
        modelBuilder.Entity<RolePermission>(opt =>
        {
            opt.HasKey(p => new { p.RoleId, p.PermissionId });
        });
        modelBuilder.Entity<UserTokens>(opt =>
        {
            opt.HasKey(p => p.Id);
        });
    }
}
