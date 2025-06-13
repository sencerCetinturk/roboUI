using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using roboUI.UI.ViewModels;

namespace roboUI.UI.ViewModels.Admin
{
    public class AdminViewModel:BaseViewModel
    {
        private Frame? _adminContentFrame; //AdminWindow'daki frame'e bağlanacak

        public ObservableCollection<AdminMenuItemViewModel> AdminMenuItems { get; set; }

        private AdminMenuItemViewModel? _selectedAdminMenuItem;
        public AdminMenuItemViewModel? SelectedAdminMenuItem
        {
            get => _selectedAdminMenuItem;
            set {
                if(SetProperty(ref _selectedAdminMenuItem, value))
                {
                    if (_selectedAdminMenuItem != null)
                    {
                        NavigateToView(_selectedAdminMenuItem.TargetViewType);
                    }
                    else if(_selectedAdminMenuItem?.NavigateCommand != null && _selectedAdminMenuItem.NavigateCommand.CanExecute(null))
                    {
                        _selectedAdminMenuItem.NavigateCommand.Execute(null);
                    }
                }
            }
        }

        //IServiceProvider'ı constructor'da alıp sayfa ve ViewModel örneklerini çözebiliriz
        private readonly IServiceProvider _serviceProvider;
        
        public AdminViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            AdminMenuItems = new ObservableCollection<AdminMenuItemViewModel>();
            LoadAdminMenuItems();
        }

        public void InitializeNavigationFrame(Frame frame)
        {
            _adminContentFrame = frame;
            //Başlangıçta bir sayfa yüklenebilir
            if (AdminMenuItems.Count > 0)
            {
                SelectedAdminMenuItem = AdminMenuItems[0];
            }
        }

        private void LoadAdminMenuItems()
        {
            AdminMenuItems.Clear();
            AdminMenuItems.Add(new AdminMenuItemViewModel
            {
                Title = "Seçenek Grupları",
                //IconPath= "Assets/AdminIcons/option_groups.png,
                TargetViewType = typeof(AdminMenuItemViewModel)
            });
            AdminMenuItems.Add(new AdminMenuItemViewModel
            {
                Title = "Seçenek Tercikleri",
                //IconPath= "Assets/AdminIcons/option_groups.png,
                TargetViewType = typeof(AdminMenuItemViewModel)
            });
            AdminMenuItems.Add(new AdminMenuItemViewModel
            {
                Title = "Kahve Ürünleri",
                //IconPath= "Assets/AdminIcons/option_groups.png,
                TargetViewType = typeof(AdminMenuItemViewModel)
            });
            // Diğer yönetim menü öğeleri eklenecek...
        }

        private void NavigateToView(Type? viewType)
        {
            if (_adminContentFrame == null || viewType == null) return;

            try
            {
                //Sayfayı ve (eğer varsa) ViewModel'ini DI container'dan çöz
                //Bu, sayfanın constructor'ında ViewModel'i enjekte almasını sağlar
                //Veya sayfanın DataContext'ini manuel olarak ayarlayabiliriz.

                ///<summary>
                ///Yöntem 1: Sayfayı ve ViewModel'i ayrı ayrı DI'dan çözüp DataContext'i ayarlama
                ///var pageInstance = (Page)_serviceProvider.GetRequiredService(viewType);
                ///if(viewType == typeof(OptionGroupManagementPage))
                ///{
                ///     pageInstance.DataContext = _serviceProvider.GetRequiredService<OptionGroupMenegementViewModel>();
                ///}
                ///else if(viewType == typeof(OptionChoiceManagementPage))
                ///{
                /// //pageInstance.DataContext = _serviceProvider.GetRequiredService<OptionChoiceManagementViewModel>();
                ///}
                /// // // ... diğer sayfalar için ...
                /// 
                /// 
                ///  Yöntem 2: Eğer sayfanın consttuctor'ı ViewModel'i alıyorsa ve ikisi de DI'da kayıtlıysa,
                ///  sadece sayfayı çözümlemek yeterli olabilir
                /// </summary>
                var pageInstance = (Page)_serviceProvider.GetRequiredService(viewType);

                _adminContentFrame.Navigate(pageInstance);
            }catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Admin Navigasyon Hatası: {ex.Message}");
            }
        }
    }
}
