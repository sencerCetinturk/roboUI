using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using roboUI.Core.Models;

namespace roboUI.Core.Interfaces
{
    public interface IOptionGroupService
    {
        Task<IEnumerable<OptionGroup>> GetAllOptionGroupAsync();
        Task<OptionGroup?> GetOptionGroupByIdAsync(Guid id);
        Task<OptionGroup> AddOptionGroupAsync(OptionGroup optionGroup);
        Task UpdateOptionGroupAsync(OptionGroup optionGroup);
        Task DeleteOptionGroupAsync(Guid id);
    }
}
