using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboUI.Core.Models
{
    public class CoffeeProduct
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }    

        [Required]
        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength (255)]
        public string? ImagePath { get; set; }

        public bool IsAvailable { get; set; } = true;//Ürün mevcut mu? satışta mı?

        public Guid? CategoryId { get; set; }
        public virtual CoffeeCategory? Category { get; set; }
        // Bu kahve için sunulan özelleştirme seçeneklerini tanımlar
        public virtual ICollection<CoffeeProductOptionDefinition> AvailableOptions { get; set; } = new List<CoffeeProductOptionDefinition>();
        // Oluşturulma ve güncellenme zamanları (isteğe bağlı ama faydalı)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public CoffeeProduct()
        {
            Id=Guid.NewGuid();
        }

    }
}
