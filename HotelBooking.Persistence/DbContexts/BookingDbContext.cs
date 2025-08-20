using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Entities.Bases;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.DbContexts;

public class BookingDbContext : DbContext
{
    private readonly IUserContext _userContext;
    public BookingDbContext(DbContextOptions<BookingDbContext> options, IUserContext userContext) : base(options)
    {
        _userContext = userContext;
    }

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
    public virtual DbSet<Payment> Payments { get; set; }

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
    }

    public override int SaveChanges()
    {
        ApplyAudit();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAudit();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAudit()
    {
        ApplyUserTracking();
        ApplyDateTracking();
    }
    private void ApplyDateTracking()
    {
        var entries = ChangeTracker.Entries<IDateTracking>();

        foreach (var entry in entries)
        {
            var now = DateTimeOffset.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedDate = now;
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDeletable softDeletableEntity)
                    {
                        softDeletableEntity.DeletedDate = now;
                        entry.State = EntityState.Modified;
                    }
                    break;
            }
        }
    }
    private void ApplyUserTracking()
    {
        var entries = ChangeTracker.Entries<IUserTracking>();

        foreach(var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _userContext.UserId;
                    break;
                case EntityState.Modified:
                case EntityState.Deleted:
                    entry.Entity.UpdatedBy = _userContext.UserId;
                    break;
            }
        }
    }
}
