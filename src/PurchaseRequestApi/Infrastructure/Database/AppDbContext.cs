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

            /*modelBuilder.Entity<Comment>()
                .HasOne(c => c.Request)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RequestId);*/

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
        }
    }
}
