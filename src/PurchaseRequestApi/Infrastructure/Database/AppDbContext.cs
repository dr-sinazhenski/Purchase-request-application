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

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Requester)
                .WithMany(a => a.Requests)
                .HasForeignKey(r => r.RequesterId);

            modelBuilder.Entity<RequesterProduct>(builder =>
            {
                builder.HasKey(rp => new { rp.RequestId, rp.ProductId });

                builder.HasOne(rp => rp.Request)
                    .WithMany(r => r.RequesterProducts)
                    .HasForeignKey(rp => rp.RequestId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);

                builder.HasOne(rp => rp.Product)
                    .WithMany(p => p.RequesterProducts)
                    .HasForeignKey(rp => rp.ProductId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

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
            modelBuilder.Entity<ApproverProfile>().HasData(
                new ApproverProfile { Id = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111"), Name = "Junior Approver",  MinAmount = 0m,       MaxAmount = 500m    },
                new ApproverProfile { Id = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111111"), Name = "Senior Approver",  MinAmount = 500m,     MaxAmount = 2000m   },
                new ApproverProfile { Id = Guid.Parse("cccccccc-1111-1111-1111-111111111111"), Name = "Executive Approver", MinAmount = 2000m,  MaxAmount = 999999m }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("aaaaaaaa-2222-2222-2222-222222222222"), Name = "Requester" },
                new Role { Id = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222"), Name = "Approver"  },
                new Role { Id = Guid.Parse("cccccccc-2222-2222-2222-222222222222"), Name = "Admin"     }
            );

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id                = Guid.Parse("aaaaaaaa-3333-3333-3333-333333333333"),
                    Login             = "john.doe",
                    Password          = "hashed_password_1",
                    Name              = "John Doe",
                    RegionId          = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // North America
                    ApproverProfileId = null
                },
                new Account
                {
                    Id                = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"),
                    Login             = "jane.smith",
                    Password          = "hashed_password_2",
                    Name              = "Jane Smith",
                    RegionId          = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), // Europe
                    ApproverProfileId = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111111")  // Senior Approver
                },
                new Account
                {
                    Id                = Guid.Parse("cccccccc-3333-3333-3333-333333333333"),
                    Login             = "peter.jones",
                    Password          = "hashed_password_3",
                    Name              = "Peter Jones",
                    RegionId          = Guid.Parse("cccccccc-dddd-dddd-dddd-dddddddddddd"), // Lithuania
                    ApproverProfileId = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111")  // Junior Approver
                },
                new Account
                {
                    Id                = Guid.Parse("dddddddd-3333-3333-3333-333333333333"),
                    Login             = "admin",
                    Password          = "hashed_password_4",
                    Name              = "System Admin",
                    RegionId          = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), // Europe
                    ApproverProfileId = Guid.Parse("cccccccc-1111-1111-1111-111111111111")  // Executive Approver
                }
            );

            // AccountRole join table seed
            modelBuilder.Entity("AccountRole").HasData(
                new { AccountId = Guid.Parse("aaaaaaaa-3333-3333-3333-333333333333"), RoleId = Guid.Parse("aaaaaaaa-2222-2222-2222-222222222222") }, // John -> Requester
                new { AccountId = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"), RoleId = Guid.Parse("aaaaaaaa-2222-2222-2222-222222222222") }, // Jane -> Requester
                new { AccountId = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"), RoleId = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222") }, // Jane -> Approver
                new { AccountId = Guid.Parse("cccccccc-3333-3333-3333-333333333333"), RoleId = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222") }, // Peter -> Approver
                new { AccountId = Guid.Parse("dddddddd-3333-3333-3333-333333333333"), RoleId = Guid.Parse("cccccccc-2222-2222-2222-222222222222") }  // Admin -> Admin
            );

            modelBuilder.Entity<Request>().HasData(
                new Request
                {
                    Id            = Guid.Parse("aaaaaaaa-4444-4444-4444-444444444444"),
                    Title         = "New Laptop for Development",
                    Description   = "Requesting a new laptop for the development team",
                    Status        = RequestStatus.Submited,
                    CreatedAt     = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                    UpdatedAt     = new DateTime(2025, 1, 10, 9, 0, 0, DateTimeKind.Utc),
                    RequesterId   = Guid.Parse("aaaaaaaa-3333-3333-3333-333333333333"),
                    RequestTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111") // IT Products
                },
                new Request
                {
                    Id            = Guid.Parse("bbbbbbbb-4444-4444-4444-444444444444"),
                    Title         = "Office Supplies Restock",
                    Description   = "Monthly office supplies restock for the Vilnius office",
                    Status        = RequestStatus.Approved,
                    CreatedAt     = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                    UpdatedAt     = new DateTime(2025, 1, 16, 14, 0, 0, DateTimeKind.Utc),
                    RequesterId   = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"),
                    RequestTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222") // Office Supplies
                },
                new Request
                {
                    Id            = Guid.Parse("cccccccc-4444-4444-4444-444444444444"),
                    Title         = "Software Licenses Q1",
                    Description   = "Annual software license renewal for Q1",
                    Status        = RequestStatus.Rejected,
                    CreatedAt     = new DateTime(2025, 1, 20, 8, 0, 0, DateTimeKind.Utc),
                    UpdatedAt     = new DateTime(2025, 1, 22, 11, 0, 0, DateTimeKind.Utc),
                    RequesterId   = Guid.Parse("aaaaaaaa-3333-3333-3333-333333333333"),
                    RequestTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333") // Software & Licenses
                },
                new Request
                {
                    Id            = Guid.Parse("dddddddd-4444-4444-4444-444444444444"),
                    Title         = "Monitor Upgrade",
                    Description   = "Requesting monitor upgrades for the design team",
                    Status        = RequestStatus.Resubmited,
                    CreatedAt     = new DateTime(2025, 2, 1, 9, 0, 0, DateTimeKind.Utc),
                    UpdatedAt     = new DateTime(2025, 2, 3, 16, 0, 0, DateTimeKind.Utc),
                    RequesterId   = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"),
                    RequestTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111") // IT Products
                }
            );

            modelBuilder.Entity<RequesterProduct>().HasData(
                // Request 1 - Laptop x1, Keyboard x1, Mouse x1
                new { RequestId = Guid.Parse("aaaaaaaa-4444-4444-4444-444444444444"), ProductId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Quantity = 1 },
                new { RequestId = Guid.Parse("aaaaaaaa-4444-4444-4444-444444444444"), ProductId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Quantity = 1 },
                new { RequestId = Guid.Parse("aaaaaaaa-4444-4444-4444-444444444444"), ProductId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Quantity = 1 },
                // Request 2 - Notebook x5, Pen Set x3
                new { RequestId = Guid.Parse("bbbbbbbb-4444-4444-4444-444444444444"), ProductId = Guid.Parse("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Quantity = 5 },
                new { RequestId = Guid.Parse("bbbbbbbb-4444-4444-4444-444444444444"), ProductId = Guid.Parse("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Quantity = 3 },
                // Request 3 - Microsoft 365 x3, Antivirus x3
                new { RequestId = Guid.Parse("cccccccc-4444-4444-4444-444444444444"), ProductId = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Quantity = 3 },
                new { RequestId = Guid.Parse("cccccccc-4444-4444-4444-444444444444"), ProductId = Guid.Parse("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Quantity = 3 },
                // Request 4 - Monitor x2
                new { RequestId = Guid.Parse("dddddddd-4444-4444-4444-444444444444"), ProductId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Quantity = 2 }
            );

            modelBuilder.Entity<Comment>().HasData(
                // Request 1 - New Laptop for Development (Submitted)
                new Comment
                {
                    Id           = Guid.Parse("aaaaaaaa-5555-5555-5555-555555555555"),
                    RequestId    = Guid.Parse("aaaaaaaa-4444-4444-4444-444444444444"),
                    AccountId    = Guid.Parse("aaaaaaaa-3333-3333-3333-333333333333"), // John (requester)
                    Text         = "Requesting a high-performance laptop for running multiple development environments.",
                    CreationTime = new DateTime(2025, 1, 10, 9, 5, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id           = Guid.Parse("bbbbbbbb-5555-5555-5555-555555555555"),
                    RequestId    = Guid.Parse("aaaaaaaa-4444-4444-4444-444444444444"),
                    AccountId    = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"), // Jane (approver)
                    Text         = "Please specify the required RAM and storage capacity.",
                    CreationTime = new DateTime(2025, 1, 11, 10, 0, 0, DateTimeKind.Utc)
                },

                // Request 2 - Office Supplies Restock (Approved)
                new Comment
                {
                    Id           = Guid.Parse("cccccccc-5555-5555-5555-555555555555"),
                    RequestId    = Guid.Parse("bbbbbbbb-4444-4444-4444-444444444444"),
                    AccountId    = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"), // Jane (requester)
                    Text         = "Monthly restock as usual, same quantities as last month.",
                    CreationTime = new DateTime(2025, 1, 15, 10, 5, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id           = Guid.Parse("dddddddd-5555-5555-5555-555555555555"),
                    RequestId    = Guid.Parse("bbbbbbbb-4444-4444-4444-444444444444"),
                    AccountId    = Guid.Parse("cccccccc-3333-3333-3333-333333333333"), // Peter (approver)
                    Text         = "Approved. Supplies will be delivered by end of week.",
                    CreationTime = new DateTime(2025, 1, 16, 14, 0, 0, DateTimeKind.Utc)
                },

                // Request 3 - Software Licenses Q1 (Rejected)
                new Comment
                {
                    Id           = Guid.Parse("eeeeeeee-5555-5555-5555-555555555555"),
                    RequestId    = Guid.Parse("cccccccc-4444-4444-4444-444444444444"),
                    AccountId    = Guid.Parse("aaaaaaaa-3333-3333-3333-333333333333"), // John (requester)
                    Text         = "We need these licenses renewed before the end of January to avoid service interruptions.",
                    CreationTime = new DateTime(2025, 1, 20, 8, 5, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id           = Guid.Parse("ffffffff-5555-5555-5555-555555555555"),
                    RequestId    = Guid.Parse("cccccccc-4444-4444-4444-444444444444"),
                    AccountId    = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"), // Jane (approver)
                    Text         = "Rejected. Budget for software licenses has been frozen for Q1. Please resubmit in Q2.",
                    CreationTime = new DateTime(2025, 1, 22, 11, 0, 0, DateTimeKind.Utc)
                },

                // Request 4 - Monitor Upgrade (Resubmitted)
                new Comment
                {
                    Id           = Guid.Parse("11111111-5555-5555-5555-555555555555"),
                    RequestId    = Guid.Parse("dddddddd-4444-4444-4444-444444444444"),
                    AccountId    = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"), // Jane (requester)
                    Text         = "The design team currently uses outdated monitors which are affecting productivity.",
                    CreationTime = new DateTime(2025, 2, 1, 9, 5, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id           = Guid.Parse("22222222-5555-5555-5555-555555555555"),
                    RequestId    = Guid.Parse("dddddddd-4444-4444-4444-444444444444"),
                    AccountId    = Guid.Parse("cccccccc-3333-3333-3333-333333333333"), // Peter (approver)
                    Text         = "Please provide model specifications and price quotes before we can proceed.",
                    CreationTime = new DateTime(2025, 2, 2, 11, 0, 0, DateTimeKind.Utc)
                },
                new Comment
                {
                    Id           = Guid.Parse("33333333-5555-5555-5555-555555555555"),
                    RequestId    = Guid.Parse("dddddddd-4444-4444-4444-444444444444"),
                    AccountId    = Guid.Parse("bbbbbbbb-3333-3333-3333-333333333333"), // Jane (requester)
                    Text         = "Resubmitted with full specifications attached. Requesting Dell 27\" 4K monitors.",
                    CreationTime = new DateTime(2025, 2, 3, 16, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
