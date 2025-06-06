using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using roboUI.Core.Enums;
using roboUI.Core.Interfaces;
using roboUI.Core.Models;
using roboUI.Data;

namespace roboUI.Services
{
    public class OrderService:IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> GetOrCreateCurrentCartAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("Session ID cannot be empty.", nameof(sessionId));
            }

            var cart = await _context.Orders
                .Include(o=>o.OrderItems)
                    .ThenInclude(oi=>oi.CoffeeProduct)
                .Include(o=>o.OrderItems)
                    .ThenInclude(oi=>oi.SelectedChoices)
                        .ThenInclude(sc=>sc.OptionChoice)
                .FirstOrDefaultAsync(o=>o.SessionId == sessionId && o.Status == OrderStatus.Pending);

            if (cart == null)
            {
                cart = new Order
                {
                    SessionId = sessionId,
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow
                    // Id ve TotalAmount DB tarafından veya sonra hesaplanacak
                };
                _context.Orders.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }

        public async Task AddItemToCartAsync(string sessionId, Guid coffeeProductId, int quantity, List<SelectedOptionChoiceDto> selectedChoicesDto)
        {
            if(quantity <=0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

            var cart= await GetOrCreateCurrentCartAsync(sessionId);
            var coffeeProduct = await _context.CoffeeProducts.FindAsync(coffeeProductId);

            if (coffeeProduct==null || !coffeeProduct.IsAvailable)
            {
                throw new InvalidOperationException("Selected coffee product is not available.");
            }
          
            //Seçilen seçeneklerin doğruluğunu ve fiyatlarını kontrol et
            decimal currentItemCalculatedPrice = coffeeProduct.Price;
            var orderItemChoices = new List<OrderItemChoiceSelection>();

            if (selectedChoicesDto != null) 
            {
                foreach (var choice in selectedChoicesDto)
                {
                    var optionChoice = await _context.OptionChoices
                        .FirstOrDefaultAsync(oc => oc.Id == choice.OptionChoiceId && oc.IsAvailable);

                    if (optionChoice != null)
                    {
                        throw new InvalidOperationException($"Selected option choice with ID {choice.OptionChoiceId} is not available.");
                    }

                    // TODO: Seçilen adedin OptionChoice.MaxQuantityAllowed'u aşmadığını kontrol et.
                    // TODO: Bir OptionGroup'tan SelectionType'a göre doğru sayıda seçim yapıldığını kontrol et. (Bu daha karmaşık)

                    orderItemChoices.Add(new OrderItemChoiceSelection
                    {
                        Id = Guid.NewGuid(),
                        OptionChoiceId = optionChoice.Id,
                        SelectedQuantity = choice.Quantity,
                        PriceAtSelection = optionChoice.AdditionalPrice
                    });
                    currentItemCalculatedPrice += (optionChoice.AdditionalPrice * choice.Quantity);
                }
            }

            //Benzer bir ürün (aynı ana ürün ve aynı seçeneklerse) sepette var mı kontrolet
            var existingOrderItem = cart.OrderItems.FirstOrDefault(oi =>
                oi.CoffeeProductId == coffeeProductId &&
                oi.SelectedChoices.Count == orderItemChoices.Count &&
                oi.SelectedChoices.All(sc1=>orderItemChoices.Any(sc2=> sc1.OptionChoiceId == sc2.OptionChoiceId && sc1.SelectedQuantity ==sc2.SelectedQuantity))
                );

            if (existingOrderItem != null)
            {
                //eğer varsa miktarını arttır
                existingOrderItem.Quantity += quantity;
                //birim fiyatı aynı kalmalı çünkü seçenekler aynı
            }
            else
            {
                //eğer yoksa, yeni bir ORderItem oluştur
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = cart.Id,
                    CoffeeProductId = coffeeProductId,
                    Quantity = quantity,
                    CalculatedUnitPrice = currentItemCalculatedPrice,
                    SelectedChoices = orderItemChoices // bu listede zaten OrderItemId'ye ihtiyaç yoktur, EF Core ilişkiyi kurar
                };
                _context.OrderItems.Add(orderItem);//veya cart.OrderItems.Add(orderItem); (eğer cart izleniyorsa)
            }

            //sepetin toplam tutarını yeniden hesapla
            cart.TotalAmount = 0;
            foreach (var item in _context.OrderItems.Local.Where(oi=>oi.OrderId==cart.Id).ToList().Concat(cart.OrderItems.Where(oi=>oi.OrderId == cart.Id && !_context.OrderItems.Local.Contains(oi))))
            //foreach (var item in cart.OrderItems.Where(oi => oi.OrderId == cart.Id)) // Eğer yukarıdaki satır sorun çıkarırsa bunu deneyin
            {
                //OderItem'ın CalculatedUnitPrice'ı zaten seçenekleri içeriyor olmalı
                //Ancak sepete eklerken seçeneklerin fiyatı ana ürün fiyatına eklenmiştir
                //CalculatedUnitPrice'ı tekrar set etmeye gerek yok, zaten doğru olmalı
                //Sadece emin olmak için:
                decimal itemBasePrice = (await _context.CoffeeProducts.FindAsync(item.CoffeeProduct))?.Price ?? 0;
                decimal optionsPrice = item.SelectedChoices.Sum(sc => sc.PriceAtSelection * sc.SelectedQuantity);
                item.CalculatedUnitPrice = itemBasePrice + optionsPrice;// Bu satır AddItemToCart içinde yapılmalı.

                cart.TotalAmount += (item.CalculatedUnitPrice * item.Quantity);
            }
            //_context.Orders.Update(cart) // TotalAmount değiştiği için (EF Core 6+ için bu gerekmeyebilir, değişiklik izleme varsa)
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetCartDetailsAsync(string sessionId)
        {
            return await _context.Orders
                .Where(o => o.SessionId == sessionId && o.Status == OrderStatus.Pending)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.CoffeeProduct)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.SelectedChoices)
                        .ThenInclude(sc => sc.OptionChoice)
                            .ThenInclude(oc => oc.OptionGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Order> PlaceOrderAsync ( string sessionId)
        {
            var cart = await _context.Orders
                .Include(o=>o.OrderItems) // savechangesAsync'nin cascade save yapabilmesi için
                .FirstOrDefaultAsync(o=>o.SessionId == sessionId && o.Status== OrderStatus.Pending);

            if(cart == null || !cart.OrderItems.Any())
            {
                throw new InvalidOperationException("Cart is empty or not found.");
            }

            //Toplam tutarın doğru olduğundan emin ol (son bir kontrol)
            cart.TotalAmount = 0;
            foreach (var item in cart.OrderItems) 
            {
                //OrderItem'a ait CoffeeProduct ve OptionChoice'ları yüklemek gerekebilir
                //Eğer CalculateItemPrice gibi bir methodunuz varsa o anki fiyatları kullanıyorsa
                //Ancak CalculatedunitPrice sepete eklenirken set edildiği için bu yeterli olmalı
                cart.TotalAmount += (item.CalculatedUnitPrice * item.Quantity);
            }

            cart.Status = OrderStatus.Processing;
            cart.OrderDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return cart;
        }
    }
}
