using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboUI.Core.Models
{
    public class CoffeeProductOptionDefinition
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CoffeeProductId { get; set; } //Hangi kahve ürünü için
        public virtual CoffeeProduct CoffeeProduct { get; set; }
    
        public Guid OptionGroupId { get; set; }
        public virtual OptionGroup OptionGroup { get; set; }

        //Bu optiongroup için bu coffeeproduct'ta varsayılan olarak seçili gelen optionchoice
        //Null olabilir, yani varsayılan yoktur, kullanıcı seçmek zorundadır (eğer optionGroup.Isrequired = true ise)
        public Guid? DefaultOptionChoiceId { get; set; }
        public virtual OptionChoice? DefaultOptionChoice { get; set; }

        //bu CoffeeProduct için bu optionGroup'un zorunlu olup olmadığını override edebiliriz
        //Genellikle optiongroup.IsRequiredForProduct kullanılır
        public bool? IsRequiredOverride { get; set; }

        //Bu CoffeeProduct için bu OptionGroup'un gösterim sırasını override edebiliriz
        public int? DisplayOrderOverride { get; set; }

    }
}
