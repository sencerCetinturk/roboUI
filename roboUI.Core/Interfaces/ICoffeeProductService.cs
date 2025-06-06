using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using roboUI.Core.Models;

namespace roboUI.Core.Interfaces
{
    public interface ICoffeeProductService
    {
        //Tüm aktif (satışta olan) kahve ürünlerini kategoriyle birlikte getir
        Task<IEnumerable<CoffeeProduct>> GetAllActiveProductsWithCategoryAsync();

        //Belirli bir kahve ürünü ID'sine göre özelleştirme seçenekleriyle(OptionGroups ve Choices)
        //ve varsa varsayılan seçenekleriyle birlikte getirir. Sadece aktif seçenekleri yükler.
        Task<CoffeeProduct?> GetProductWithOptionsByIdAsync(Guid produtcId);

        // TODO: Gelecekte eklenebilecek diğer metotlar:
        // Task AddProductAsync(CoffeeProduct product);
        // Task UpdateProductAsync(CoffeeProduct product);
        // Task DeleteProductAsync(Guid productId);
        // Task<IEnumerable<CoffeeProduct>> GetProductsByCategoryAsync(Guid categoryId);
    }
}
