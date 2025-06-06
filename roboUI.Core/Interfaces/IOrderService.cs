using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using roboUI.Core.Models;

namespace roboUI.Core.Interfaces
{
    //UI'dan seçilen seçenekleri servise taşımak için bir DTO

    public class SelectedOptionChoiceDto
    {
        public Guid OptionChoiceId { get; set; }
        public int Quantity { get; set; } = 1;// Seçilen adedi (örn: 2 pompa şurup
    }
    public interface IOrderService
    {
        ///<summary>
        ///Verilen session/cihaz ID'si için mevcut sepeti getirir veya yoksa yeni bir tane oluşturur
        /// </summary>
        Task<Order> GetOrCreateCurrentCartAsync(string sessionId);

        ///<summary>
        ///Mevcut sepete yeni bir özelleştirilmiş kahve ürünü ekler
        /// </summary>
        /// <param name="sessionId">Geçerli sepetin ID'si.</param>
        /// <param name="coffeeProductId">Eklenecek ana kahve ürününün ID'si.</param>
        /// <param name="quantity">Ana kahveden kaç adet ekleneceği.</param>
        /// <param name="selectedChoices">Bu kahve için seçilen tüm özelleştirme seçenekleri ve adetleri.</param>
        /// 
        Task AddItemToCartAsync(string sessionId, Guid coffeeProductId, int quantity, List<SelectedOptionChoiceDto> optionChoices);

        ///<summary>
        ///Mevcut sepetin tüm detaylarını (ürünler ve seçilen seçenekler) getirir.
        /// </summary>
        /// 
        Task<Order> GetCartDetailsAsync(string sessionId);

        /// <summary>
        /// Sepetteki bir ürünü günceller (örn: adetini değiştirme).
        /// </summary>
        // Task UpdateCartItemAsync(string sessionId, Guid orderItemId, int newQuantity, List<SelectedOptionChoiceDto> updatedSelectedChoices);

        /// <summary>
        /// Sepetten bir ürünü kaldırır.
        /// </summary>
        // Task RemoveItemFromCartAsync(string sessionId, Guid orderItemId);

        /// <summary>
        /// Sepeti temizler.
        /// </summary>
        // Task ClearCartAsync(string sessionId);

        /// <summary>
        /// Mevcut sepeti siparişe dönüştürür.
        /// </summary>
        Task<Order> PlaceOrderAsync(string sessionId);

    }
}
