using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<ApproverProfile> ApproverProfiles { get; set; }
        public DbSet<RequesterProduct> RequesterProducts { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Role)
                .WithMany(r => r.Account)
                .UsingEntity(j => j.ToTable("AccountRole"));

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Region)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RegionId);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.ApproverProfile)
                .WithMany(ap => ap.Accounts)
                .HasForeignKey(a => a.ApproverProfileId)
                .IsRequired(false);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Request)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RequestId);

            /*modelBuilder.Entity<Comment>()
                .HasOne(c => c.Account)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AccountId);*/

            modelBuilder.Entity<Request>()
                .Property(e => e.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Request>()
                .HasOne(r => r.RequestType)
                .WithMany(rt => rt.Requests)
                .HasForeignKey(r => r.RequestTypeId);

            /*modelBuilder.Entity<Request>()
                .HasOne(r => r.Requester)
                .WithMany(a => a.Requests)
                .HasForeignKey(r => r.RequesterId);*/

            modelBuilder.Entity<RequesterProduct>()
                .HasKey(rp => new { rp.RequestId, rp.ProductId });

            modelBuilder.Entity<RequesterProduct>()
                .HasOne(rp => rp.Request)
                .WithMany(r => r.RequesterProducts)
                .HasForeignKey(rp => rp.RequestId);

            modelBuilder.Entity<RequesterProduct>()
                .HasOne(rp => rp.Product)
                .WithMany(p => p.RequesterProducts)
                .HasForeignKey(rp => rp.ProductId);

            modelBuilder.Entity<Price>()
                .HasKey(p => new { p.ProductId, p.RegionId });

            modelBuilder.Entity<Price>()
                .HasOne(p => p.Product)
                .WithMany(p => p.Prices)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<Price>()
                .HasOne(p => p.Region)
                .WithMany(r => r.Prices)
                .HasForeignKey(p => p.RegionId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.RequestType)
                .WithMany(rt => rt.Product)
                .UsingEntity(j => j.ToTable("ProductGroup"));

            //---SEED DATA---
            modelBuilder.Entity<RequestType>().HasData(
                new RequestType { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "IT Products" },
                new RequestType { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Office Supplies" },
                new RequestType { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Software & Licenses" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Laptop",               Description = "Standard business laptop for daily work use" },
                new Product { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Monitor",              Description = "24-inch Full HD display" },
                new Product { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Keyboard",             Description = "Wired USB office keyboard" },
                new Product { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Name = "Mouse",                Description = "Wired USB optical mouse" },
                new Product { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Name = "Desk",                Description = "Standard office desk with cable management" },
                new Product { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Name = "Chair",               Description = "Ergonomic office chair with lumbar support" },
                new Product { Id = Guid.Parse("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Notebook",            Description = "A5 lined notebook, pack of 3" },
                new Product { Id = Guid.Parse("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Pen Set",             Description = "Pack of 10 ballpoint pens" },
                new Product { Id = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Microsoft 365",       Description = "Annual Microsoft 365 Business Standard license" },
                new Product { Id = Guid.Parse("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Adobe Creative Cloud", Description = "Annual Adobe Creative Cloud all-apps license" },
                new Product { Id = Guid.Parse("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Antivirus",           Description = "Annual enterprise antivirus license" }
            );

            modelBuilder.Entity("ProductRequestType").HasData(
                // IT Products
                new { ProductId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RequestTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
                new { ProductId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), RequestTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
                new { ProductId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), RequestTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
                new { ProductId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), RequestTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111") },
                // Office Supplies
                new { ProductId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), RequestTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222") },
                new { ProductId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), RequestTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222") },
                new { ProductId = Guid.Parse("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RequestTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222") },
                new { ProductId = Guid.Parse("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RequestTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222") },
                // Software & Licenses
                new { ProductId = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RequestTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333") },
                new { ProductId = Guid.Parse("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RequestTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333") },
                new { ProductId = Guid.Parse("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RequestTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333") }
            );
            modelBuilder.Entity<Region>().HasData(
                new Region { Id = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "North America", Currency = "USD" },
                new Region { Id = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Name = "Europe",        Currency = "EUR" },
                new Region { Id = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Name = "Lithuania",     Currency = "EUR" }
            );

            modelBuilder.Entity<Price>().HasData(
                // Laptop
                new Price { ProductId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 1200.00m, UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 1100.00m, UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 1050.00m, UnitsOfMeasure = "pcs" },
                // Monitor
                new Price { ProductId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 300.00m,  UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 280.00m,  UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 270.00m,  UnitsOfMeasure = "pcs" },
                // Keyboard
                new Price { ProductId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 45.00m,   UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 40.00m,   UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 38.00m,   UnitsOfMeasure = "pcs" },
                // Mouse
                new Price { ProductId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 25.00m,   UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 22.00m,   UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 20.00m,   UnitsOfMeasure = "pcs" },
                // Desk
                new Price { ProductId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 350.00m,  UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 320.00m,  UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 300.00m,  UnitsOfMeasure = "pcs" },
                // Chair
                new Price { ProductId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 400.00m,  UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 370.00m,  UnitsOfMeasure = "pcs" },
                new Price { ProductId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 350.00m,  UnitsOfMeasure = "pcs" },
                // Notebook
                new Price { ProductId = Guid.Parse("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 12.00m,   UnitsOfMeasure = "pack" },
                new Price { ProductId = Guid.Parse("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 10.00m,   UnitsOfMeasure = "pack" },
                new Price { ProductId = Guid.Parse("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 9.00m,    UnitsOfMeasure = "pack" },
                // Pen Set
                new Price { ProductId = Guid.Parse("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 8.00m,    UnitsOfMeasure = "pack" },
                new Price { ProductId = Guid.Parse("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 7.00m,    UnitsOfMeasure = "pack" },
                new Price { ProductId = Guid.Parse("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 6.50m,    UnitsOfMeasure = "pack" },
                // Microsoft 365
                new Price { ProductId = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 150.00m,  UnitsOfMeasure = "license" },
                new Price { ProductId = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 140.00m,  UnitsOfMeasure = "license" },
                new Price { ProductId = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 135.00m,  UnitsOfMeasure = "license" },
                // Adobe Creative Cloud
                new Price { ProductId = Guid.Parse("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 600.00m,  UnitsOfMeasure = "license" },
                new Price { ProductId = Guid.Parse("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 560.00m,  UnitsOfMeasure = "license" },
                new Price { ProductId = Guid.Parse("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 540.00m,  UnitsOfMeasure = "license" },
                // Antivirus
                new Price { ProductId = Guid.Parse("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Amount = 80.00m,   UnitsOfMeasure = "license" },
                new Price { ProductId = Guid.Parse("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), Amount = 75.00m,   UnitsOfMeasure = "license" },
                new Price { ProductId = Guid.Parse("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), RegionId = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), Amount = 70.00m,   UnitsOfMeasure = "license" }
            );
        }
    }
}
