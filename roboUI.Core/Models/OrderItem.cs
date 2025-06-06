using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboUI.Core.Models
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }

        //Sepete eklenen ana kahve ürünü
        public Guid CoffeeProductId { get; set; }
        public virtual CoffeeProduct CoffeeProduct {  get; set; }

        public int Quantity { get; set; } //Bu özelleştirilmiş kahveden kaç adet

        //Bu kalemin (tüm seçenekler dahil) birim fiyatı
        //CoffeeProduct.BasePrice + tüm seçilen OptionChoice.AdditionalPrice'ların toplamı
        [Column(TypeName = "decimal(18,2)")]
        public decimal CalculatedUnitPrice { get; set; }

        //Bu kalemin toplam fiyatı(CalculatedUnitPrice * Quantity)
        [NotMapped]
        public decimal TotalItemPrice => CalculatedUnitPrice * Quantity;

        //Bu sipariş kalemi için yapılan sğesifik tercihler
        public virtual ICollection<OrderItemChoiceSelection> SelectedChoices { get; set; } = new List<OrderItemChoiceSelection>();
    }
}
