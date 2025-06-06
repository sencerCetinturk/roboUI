using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace roboUI.Core.Models
{
    public class CoffeeCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<CoffeeProduct> CoffeeProducts { get; set; }=new List<CoffeeProduct>();
    }
}
