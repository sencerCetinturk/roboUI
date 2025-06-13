using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using roboUI.UI.ViewModels.Admin;

namespace roboUI.UI.Views.Pages.Admin
{
    /// <summary>
    /// OptionGroupManagementPage.xaml etkileşim mantığı
    /// </summary>
    public partial class OptionGroupManagementPage : Page
    {
        // ViewModel'i constructor injection ile al
        public OptionGroupManagementPage(OptionGroupManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel; // DataContext'i ata
        }

        // XAML Tasarımcısı ve DI kullanılmayan durumlar için parametresiz constructor
        // Bu constructor'ı sadece tasarım zamanı için kullanmak veya DI ile çözülmüyorsa
        // ViewModel'i manuel olarak atamak (App.ServiceProvider gibi) gerekebilir.
        // Ancak DI ile yukarıdaki constructor tercih edilir.
        public OptionGroupManagementPage()
        {
            InitializeComponent();
            // Bu constructor çağrıldığında DataContext'in nasıl set edileceğine dikkat edin.
            // Tasarım zamanı için mock bir ViewModel set edilebilir.
            // if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            //     // DataContext = new OptionGroupManagementViewModel(new MockOptionGroupService());
            // }
        }
    }
}
