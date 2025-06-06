using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboUI.Core.Models
{
    public class OptionChoice
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OptionGroupId { get; set; } // Hangi gruba ait (boyut, süt vb)
        public virtual OptionGroup OptionGroup { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Örn: Büyük, Badem Sütü, Vanilya Şurubu, Ekstra Shot

        //Bu seçenek seçildiğinde ana ürün fiyatına eklenecek tutar.
        [Column(TypeName = "decimal(18,2)")]
        public decimal AdditionalPrice { get; set; } = 0;

        public bool IsAvailable { get; set; } = true;// bu seçenek şuanda sunuluyor mu?

        public int DefaultQuantity { get; set; } = 1;//Genellikle 1 ama, örn: şuruplar için "1 pompa"
        public int MaxQuantityAllowed { get; set; } = 1;// Bu Seçenek en fazla kaç tane seçilebilir (örn: Vanilya Şurubu max 3 pompa)

        //hangi CoffeeProductOptionDefinition'larda bu seçenek kullanılıyor
        public virtual ICollection<CoffeeProductOptionDefinition> CoffeeProductOptionDefinitions { get; set; } = new List<CoffeeProductOptionDefinition>();
    }
}
