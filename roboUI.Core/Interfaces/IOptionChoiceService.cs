using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using roboUI.Core.Models;

namespace roboUI.Core.Interfaces
{
    public interface IOptionChoiceService
    {
        Task<IEnumerable<OptionChoice>> GetAllOptionChoicesAsync();
        Task<IEnumerable<OptionChoice>> GetChoicesByGroupIdAsync(Guid optionGroupId);
        Task<OptionChoice?> GetOptionChoiceByIdAsync(Guid id);
        Task<OptionChoice> AddOptionChoiceAsync(OptionChoice optionChoice);
        Task UpdateOptionChoiceAsync(OptionChoice optionChoice);
        Task DeleteOptionChoiceAsync(Guid id);
    }
}
