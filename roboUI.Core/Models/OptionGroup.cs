using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using roboUI.Core.Enums;

namespace roboUI.Core.Models
{
    public class OptionGroup
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        //Bu gruptaki seçenekler UI'da nasıl gösterilecek
        public OptionSelectionType SelectionType { get; set; } = OptionSelectionType.Single;

        //bu gruptan bir seçim yapmak zorunlu mu?
        public bool IsRequiredForProduct { get; set; } = false;

        public int DisplayOrder { get; set; } //UI'da gösterim sırası

        //bu gruba ait tüm olası seçenekler/tercihler
        public virtual ICollection<OptionChoice> Choices { get; set; } = new List<OptionChoice>();
    }
}
