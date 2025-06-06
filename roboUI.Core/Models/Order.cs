using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using roboUI.Core.Enums;

namespace roboUI.Core.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string? SessionId { get; set; } //Kiosk cihaz ID'si veya geçici sepet ID'si

        public DateTime OrderDate { get; set; }= DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }//Sepetteki tüm ürünlerin son hesaplanmış fiyatı

        public OrderStatus Status = OrderStatus.Pending;

        [MaxLength(100)]
        public string? Notes { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
