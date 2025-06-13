
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using roboUI.UI.ViewModels.Admin;
using roboUI.UI.Views.Pages.Admin;


namespace roboUI.UI.Views.Windows
{
    /// <summary>
    /// AdminWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly AdminViewModel _viewModel;
        public AdminWindow(AdminViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel; // Pencerenin DataContext'ini ViewModel'e ata

            //ViewModel'e Frame'i bildir, böylece navigasyonu yapabilir
            _viewModel.InitializeNavigationFrame(AdminContentFrame);
        
        //LoadOptionGroupManagementPage();
        }

  

        private void CloseAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        //private void LoadOptionGroupManagementPage()
        //{
        //    // ViewModel'i DI container'dan al
        //    // Bunun için App.xaml.cs'de OptionGroupManagementViewModel'in kaydedilmiş olması gerekir.
        //    var viewModel = App.ServiceProvider.GetRequiredService<OptionGroupManagementViewModel>();
        //    var page = new OptionGroupManagementPage(); // Sayfayı yeni oluşturuyoruz
        //    page.DataContext = viewModel; // DataContext'i atıyoruz
        //    AdminContentFrame.Navigate(page);
        //}

        // İleride diğer yönetim sayfalarına geçiş için metotlar eklenebilir:
        // private void LoadOptionChoiceManagementPage() { /* ... */ }
        // private void LoadCoffeeProductManagementPage() { /* ... */ }
    }
}
