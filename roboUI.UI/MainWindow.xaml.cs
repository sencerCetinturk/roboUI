using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using roboUI.UI;
using roboUI.UI.Views.Windows;

namespace roboUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.A)
            {
                // AdminWindow'u aç (DI container'dan alarak veya direkt new ile)
                // Eğer AdminWindow'u da DI'a kaydettiyseniz:
                //var adminWindow = App.ServiceProvider.GetService<AdminWindow>();
                //adminWindow?.Show();

                // Veya direkt oluşturup gösterin (DI'a kaydetmediyseniz):
                AdminWindow adminWin = new AdminWindow(); // AdminWindow.xaml.cs içinde gerekli ViewModel'i DataContext'e atayın
                adminWin.Owner = this; // İsteğe bağlı
                adminWin.Show();

                e.Handled = true;
            }
        }
    }
}