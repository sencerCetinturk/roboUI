using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using roboUI.Core.Models;

namespace roboUI.Data
{
    public class ApplicationDbContext:DbContext
    {       
        //Kahve sistemi için DbSet'ler
        public DbSet<CoffeeProduct> CoffeeProducts { get; set; }    
        public DbSet<CoffeeCategory> CoffeeCategories { get; set; }
        public DbSet<OptionGroup> OptionGroups { get; set; }
        public DbSet<OptionChoice> OptionChoices { get; set; }
        public DbSet<CoffeeProductOptionDefinition> OptionChoicesDefinitions { get; set; }

        // Sipariş sistemi için DbSet'ler
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }  
        public DbSet<OrderItemChoiceSelection> OrderItemsChoiceSelections { get; set; }

        public string DbPath { get; }

        public ApplicationDbContext() //Parametresiz constructor migration için gerekli olabilir
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Combine(path, "roboUI","roboUILocalDb.db");
            var dbDir= Path.GetDirectoryName(DbPath);
            if(!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);
        }

        //EF Core DI için DbContextOptions<ApplicationDbContext> alan constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 
        {
            //Eğer DI kullanılıyorsa DbPath burada set edilmeyebilir, OnConfiguring'de options'tan gelir
            //Bu örnekte basitlik için yukarıdaki gibi bırakıyorum, DI ile kullanımda burası farklılaşır
            if (string.IsNullOrEmpty(DbPath))
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);
                DbPath = Path.Combine(path, "roboUI", "roboUILocalDb.db");
                var dbDir = Path.GetDirectoryName(DbPath);
                if (!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) // Sadece DI ile konfigüre edilmediyse çalışır
            {
                // SQLite bağlantı dizginizi buraya yazın
                // Örneğin:
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);
                string dbPath = Path.Combine(path, "roboUI", "roboUILocalDb.db");
                var dbDir = Path.GetDirectoryName(dbPath);
                if (dbDir != null && !Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //enumları string olarak kaydet
            modelBuilder.Entity<OptionGroup>().Property(og=>og.SelectionType).HasConversion<string>();
            modelBuilder.Entity<Order>().Property(o=>o.Status).HasConversion<string>();

            //CoffeeProduct - CoffeeCategory ilişkisi
            modelBuilder.Entity<CoffeeProduct>()
                .HasOne(cp=>cp.Category)
                .WithMany(cc=>cc.CoffeeProducts)
                .HasForeignKey(cp=>cp.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            //CoffeeProduct - CoffeeProductOptionDefinition ilişkisi
            modelBuilder.Entity<CoffeeProductOptionDefinition>()
                .HasOne(cpod => cpod.CoffeeProduct)
                .WithMany(cp => cp.AvailableOptions)
                .HasForeignKey(cpod => cpod.CoffeeProductId);

            //optionGroup - CoffeeProductOptionDefinition ilişkisi
            modelBuilder.Entity<CoffeeProductOptionDefinition>()
            .HasOne(cpod => cpod.OptionGroup)
            .WithMany()// OptionGroup'un CPOD'lara direkt listesi yok
            .HasForeignKey(cpod => cpod.OptionGroupId);

            //OptionChoice (Default) - CoffeeProductOptionDefinition ilişkisi
            modelBuilder.Entity<CoffeeProductOptionDefinition>()
                .HasOne(cpod => cpod.DefaultOptionChoice)
                .WithMany() // OptionChoice'un Default olduğu CPOD'lara direkt listesi yok
                .HasForeignKey(cpod => cpod.DefaultOptionChoiceId)
                .OnDelete(DeleteBehavior.Restrict);//varsayılan seçenek silinirse tanım bozulmasın

            //OptionGroup - OptionChoice ilişkisi
            modelBuilder.Entity<OptionChoice>()
                .HasOne(oc => oc.OptionGroup)
                .WithMany(og => og.Choices)
                .HasForeignKey(oc => oc.OptionGroupId);

            //Order - OrderItem ilişkisi
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi=>oi.Order)
                .WithMany(o=>o.OrderItems)
                .HasForeignKey(oi=>oi.OrderId);

            //OrderItem - CoffeeProduct ilişkisi
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.CoffeeProduct)
                .WithMany() // CoffeeProduct'ın OrderItem'lara direkt listesi yok
                .HasForeignKey(oi => oi.CoffeeProductId)
                .OnDelete(DeleteBehavior.Restrict);// Ürün silinirse sipariş bozulmasın.

            //orderItem - OrderItemChoiceSelection ilişkisi
            modelBuilder.Entity<OrderItemChoiceSelection>()
                .HasOne(oics => oics.OrderItem)
                .WithMany(oi => oi.SelectedChoices)
                .HasForeignKey(oics => oics.OrderItemId);

            //OrderItemChoiceSelection - OptionChoice ilişkisi 
            modelBuilder.Entity<OrderItemChoiceSelection>()
                .HasOne(oics=>oics.OptionChoice)
                .WithMany()
                .HasForeignKey(oics=>oics.OptionChoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            //decimal alanar için hassasiyet
            modelBuilder.Entity<CoffeeProduct>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<OptionChoice>().Property(p=>p.AdditionalPrice).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(p=>p.TotalAmount).HasPrecision(18, 2);    
            modelBuilder.Entity<OrderItem>().Property(p=>p.CalculatedUnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<OrderItemChoiceSelection>().Property(p=>p.PriceAtSelection).HasPrecision(18, 2);

            // Gerekirse Unique Index'ler eklenebilir (örn: OptionGroup.Name)
            // modelBuilder.Entity<OptionGroup>().HasIndex(og => og.Name).IsUnique();

            // Seed Data (Başlangıç Verileri)
            // Örnek:
            // modelBuilder.Entity<CoffeeCategory>().HasData(new CoffeeCategory { Id = 1, Name = "Sıcak Kahveler" });
            // modelBuilder.Entity<OptionGroup>().HasData(
            //     new OptionGroup { Id = 1, Name = "Boyut", SelectionType = OptionSelectionType.Single, IsRequiredForProduct = true, DisplayOrder = 1 },
            //     new OptionGroup { Id = 2, Name = "Süt Tipi", SelectionType = OptionSelectionType.Single, IsRequiredForProduct = true, DisplayOrder = 2 }
            // );
            // modelBuilder.Entity<OptionChoice>().HasData(
            //     new OptionChoice { Id = 1, OptionGroupId = 1, Name = "Orta", AdditionalPrice = 0m, IsAvailable = true, DefaultQuantity = 1, MaxQuantityAllowed = 1 },
            //     new OptionChoice { Id = 2, OptionGroupId = 1, Name = "Büyük", AdditionalPrice = 3.00m, IsAvailable = true, DefaultQuantity = 1, MaxQuantityAllowed = 1 },
            //     new OptionChoice { Id = 3, OptionGroupId = 2, Name = "Tam Yağlı Süt", AdditionalPrice = 0m, IsAvailable = true, DefaultQuantity = 1, MaxQuantityAllowed = 1 }
            // );
            // modelBuilder.Entity<CoffeeProduct>().HasData(new CoffeeProduct { Id = 1, Name = "Latte", BasePrice = 25.00m, CategoryId = 1, IsAvailable = true });
            // modelBuilder.Entity<CoffeeProductOptionDefinition>().HasData(
            //     new CoffeeProductOptionDefinition { Id = 1, CoffeeProductId = 1, OptionGroupId = 1, DefaultOptionChoiceId = 1 }, // Latte için Boyut grubu, varsayılan Orta
            //     new CoffeeProductOptionDefinition { Id = 2, CoffeeProductId = 1, OptionGroupId = 2, DefaultOptionChoiceId = 3 }  // Latte için Süt grubu, varsayılan Tam Yağlı
            // );
        }
    }
}
