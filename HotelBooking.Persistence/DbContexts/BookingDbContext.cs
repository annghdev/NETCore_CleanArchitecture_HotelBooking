using HotelBooking.Domain.Entities;
using HotelBooking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.DbContexts;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<User> Users  { get; set; }
    public virtual DbSet<Customer> Customers  { get; set; }
    public virtual DbSet<Booking> Bookings { get; set; }
    public virtual DbSet<PricingPolicy> PricingPolicies  { get; set; }
    public virtual DbSet<Role> Roles  { get; set; }
    public virtual DbSet<Permission> Permissions  { get; set; }
    public virtual DbSet<UserRole> UserRoles  { get; set; }
    public virtual DbSet<UserPermission> UserPermissions  { get; set; }
    public virtual DbSet<RolePermission> RolePermissions  { get; set; }
    public virtual DbSet<Payment> Payments  { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
