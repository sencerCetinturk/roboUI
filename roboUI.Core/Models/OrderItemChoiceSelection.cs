using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboUI.Core.Models
{
    public class OrderItemChoiceSelection
    {
        [Key]
        public Guid Id { get; set; }

        public Guid OrderItemId { get; set; }
        public virtual OrderItem OrderItem { get; set; }

        //Hangi OptionChoice seçildi (örn: Büyük Boy, Badem Sütü, Vanilya şurubu)
        public Guid OptionChoiceId { get; set; }
        public virtual OptionChoice OptionChoice { get; set; }

        //Bu seçenek kaç adet seçildi (örn: Vanilya şurubu için 2 pompa)
        //bu, optionchoice.MaxQuantityAllowed'ı aşmamalı
        public int SelectedQuantity { get; set; } = 1;

        //Seçildiği andaki OptionChoice'un ek fiyatı
        //Fiyatlar değişebileceği için burada anlık fiyatı saklamak iyi bir pratiktir
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtSelection { get; set; } //OptionChoice.AdditionalPrice değeri
    }
}
