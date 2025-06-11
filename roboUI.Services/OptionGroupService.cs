using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using roboUI.Core.Interfaces;
using roboUI.Core.Models;
using roboUI.Data;

namespace roboUI.Services
{
    public class OptionGroupService:IOptionGroupService
    {
        readonly ApplicationDbContext _context;

        public OptionGroupService(ApplicationDbContext context)
        {  _context = context; }

        public async Task<OptionGroup> AddOptionGroupAsync(OptionGroup optionGroup)
        {
            if (optionGroup==null)
            {
                throw new ArgumentNullException(nameof(optionGroup));
            }

            optionGroup.Id= Guid.NewGuid();
            _context.OptionGroups.Add(optionGroup);
            await _context.SaveChangesAsync();
            return optionGroup;
        }

        public async Task DeleteOptionGroupAsync(Guid id)
        {
            var optionGroup = await _context.OptionGroups.FindAsync(id);
            if (optionGroup != null)
            {
                //İlişkili OptionChoice'ları ve CoffeeProductOptionDefinition'ların nasıl ele alacağımıza dikkat etmeliyiz
                //OnDelete davranışları DbContext'te ayarlandı
                _context.OptionGroups.Remove(optionGroup);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OptionGroup>> GetAllOptionGroupAsync()
        {
            return await _context.OptionGroups
                .OrderBy(og => og.DisplayOrder)
                .ThenBy(og => og.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OptionGroup?> GetOptionGroupByIdAsync(Guid id)
        {
            return await _context.OptionGroups
                .OrderBy(og=>og.Choices)
                .AsNoTracking()
                .FirstOrDefaultAsync(og=>og.Id == id);
        }

        public async Task UpdateOptionGroupAsync(OptionGroup optionGroup)
        {
            if (optionGroup == null) throw new ArgumentNullException(nameof(optionGroup));
            _context.Entry(optionGroup).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Başka bir kullanıcı tarafından silinmiş yada güncellenmiş olabilir
                if(!await _context.OptionGroups.AnyAsync(e=>e.Id == optionGroup.Id))
                {
                    throw new KeyNotFoundException("OptionGroup not found.");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
