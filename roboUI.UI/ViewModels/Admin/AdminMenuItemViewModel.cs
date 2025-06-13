using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using roboUI.UI.ViewModels;

namespace roboUI.UI.ViewModels.Admin
{
    public class AdminMenuItemViewModel :   BaseViewModel
    {

        private string _title= string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string? _iconPath;
        public string? IconPath
        {
            get => _iconPath;
            set => SetProperty(ref _iconPath, value);   
        }

        //Hangi sayfaya veya UserControl'e navigate edileceği
        public Type? TargetViewType { get; set; }

        //Veya navigasyonu yönetecek bir komut
        public ICommand? NavigateCommand { get; set; }
    }
}
