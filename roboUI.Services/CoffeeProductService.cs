using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using roboUI.Core.Interfaces;
using roboUI.Core.Models;
using roboUI.Data;

namespace roboUI.Services
{

    public class CoffeeProductService : ICoffeeProductService
    {
        private readonly ApplicationDbContext _context;

        public CoffeeProductService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CoffeeProduct>> GetAllActiveProductsWithCategoryAsync()
        {
            return await _context.CoffeeProducts
                .Where(cp => cp.IsAvailable)
                .Include(cp => cp.Category)
                .OrderBy(cp => cp.Category != null ? cp.Category.Name : "") //önce kategoriye göre sırala (isteğe bağlı)
                .ThenBy(cp => cp.Name)   // sonra ürün adına göre sırala    
                .AsNoTracking()//sadece okuma yapılacaksa performansı artırabilir
                .ToListAsync();
        }

        public async Task<CoffeeProduct?> GetProductWithOptionsByIdAsync(Guid productId)
        {
            return await _context.CoffeeProducts
                .Where(cp=>cp.Id == productId&& cp.IsAvailable)
                .Include(cp => cp.Category)
                .Include(cp=>cp.AvailableOptions)// Ürünün hangi seçenek gruplarını sunduğu
                    .ThenInclude(ao=>ao.OptionGroup) // Seçenek grubunun kendisi
                        .ThenInclude(og=>og.Choices.Where(c=>c.IsAvailable)) // O gruptaki mevcut tüm seçenekler
                .Include(cp=>cp.AvailableOptions)
                    .ThenInclude(ao=>ao.DefaultOptionChoice) // O grup için varsayılan seçenek
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
