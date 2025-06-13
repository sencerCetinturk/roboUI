using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using roboUI;
using roboUI.Core.Interfaces;
using roboUI.Data;
using roboUI.Services;
using roboUI.UI.ViewModels.Admin;
using roboUI.UI.Views.Windows;
using roboUI.UI.ViewModels;

namespace roboUI.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App() 
        { 
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 1. DbContext'i Kaydetme
            // ApplicationDbContext'teki parametresiz constructor veya OnConfiguring
            // metodu SQLite bağlantısını hallediyorsa bu yeterli.
            // Eğer ApplicationDbContext'iniz DbContextOptions alıyorsa:
            /*
            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SizinProjeAdinizKahve", // DbContext'tekiyle aynı olmalı
                "kahvedb.db"            // DbContext'tekiyle aynı olmalı
            );
            var dbDir = Path.GetDirectoryName(dbPath);
            if (dbDir != null && !Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}")
            );
            */
            // VEYA ApplicationDbContext'inizde parametresiz constructor varsa ve OnConfiguring'i kullanıyorsa:
            services.AddDbContext<ApplicationDbContext>();


            // 2. Servisleri Kaydetme (Arayüzleri ve Somut Sınıfları)
            // Scoped: Genellikle web uygulamalarında her HTTP isteği için bir tane oluşturulur.
            //         WPF'de bir pencere veya kullanıcı oturumu başına bir tane gibi düşünülebilir.
            // Transient: Her istendiğinde yeni bir tane oluşturulur.
            // Singleton: Uygulama ömrü boyunca sadece bir tane oluşturulur.
            // Servisler genellikle Scoped veya Transient olarak kaydedilir. State tutmayan basit servisler için Transient uygundur.
            services.AddTransient<ICoffeeProductService, CoffeeProductService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOptionGroupService, OptionGroupService>();
            services.AddTransient<IOptionChoiceService, OptionChoiceService>();
            // Diğer servisler eklendikçe buraya eklenecek...


            // 3. ViewModel'leri Kaydetme
            // ViewModel'ler genellikle Transient olarak kaydedilir, çünkü her view için yeni bir instance gerekebilir.
            services.AddTransient<MainViewModel>(); // Bu ViewModel'i bir sonraki adımda oluşturacağız
            services.AddTransient<OptionGroupManagementViewModel>(); 
            services.AddTransient<OptionChoiceManagementViewModel>();
            services.AddTransient<CoffeeProductManagementViewModel>();
            services.AddTransient<AdminViewModel>();
            services.AddTransient<AdminWindow>();
            // Diğer ViewModel'ler eklendikçe buraya eklenecek...


            // 4. Ana Pencereyi Kaydetme (isteğe bağlı, ama başlangıçta faydalı)
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Veritabanının oluşturulduğundan/migrationların uygulandığından emin ol
            // Bu satır, veritabanı yoksa oluşturur ve bekleyen migration'ları uygular.
            // Geliştirme sırasında kullanışlıdır.
            try
            {
                using (var scope = ServiceProvider.CreateScope()) // Servisleri çözümlemek için scope oluştur
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.Migrate(); // Bu satır migration'ları uygular
                }
            }
            catch (Exception ex)
            {
                // Migration veya veritabanı bağlantı hatası
                MessageBox.Show($"Veritabanı başlatılırken bir hata oluştu: {ex.Message}\n\nUygulama kapatılacak.", "Veritabanı Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                // Loglama yapılabilir
                Current.Shutdown(-1); // Uygulamayı hata koduyla kapat
                return;
            }


            // Ana pencereyi DI container'dan alıp göster
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }
    }
}
