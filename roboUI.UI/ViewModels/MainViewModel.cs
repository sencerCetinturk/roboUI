using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using roboUI.Core.Interfaces;

namespace roboUI.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ICoffeeProductService _coffeeProductService;
        private string _welcomeMessage;

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }
        public MainViewModel(ICoffeeProductService coffeeProductService)
        {
            _coffeeProductService = coffeeProductService;
            WelcomeMessage = "Kahve Dünyasına Hoş Geldiniz!";
            //LoadDataAsync(); //Örneğin constructor'da veri yükleme
        }

        public async Task LoadDataAsync()
        {
            var products= await _coffeeProductService.GetAllActiveProductsWithCategoryAsync();
            //Bu ürünleri bir ObservableCollection'a ata
            OnPropertyChanged("ProductsCollectionsProperyName");// Eğer koleksiyonun kendisi değişiyorsa
        }
    }
}
