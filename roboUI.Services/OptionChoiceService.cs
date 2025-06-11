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
    public class OptionChoiceService : IOptionChoiceService
    {
        private readonly ApplicationDbContext _context;

        public OptionChoiceService(ApplicationDbContext context)
        { _context = context; }

        public async Task<OptionChoice> AddOptionChoiceAsync(OptionChoice optionChoice)
        {
            if (optionChoice == null) throw new ArgumentNullException(nameof(optionChoice));
            optionChoice.Id = Guid.NewGuid();
            //OptionGroupId'nin geçerli bir OptionGroup'a ait olup olmadığını kontrol etmek iyi bir pratik olabilir
            _context.OptionChoices.Add(optionChoice);
            await _context.SaveChangesAsync();
            return optionChoice;
        }

        public async Task DeleteOptionChoiceAsync(Guid id)
        {
            var optionChoice = await _context.OptionChoices.FindAsync(id);
            if (optionChoice != null)
            {
                // İlişkili CoffeeProductOptionDefinition ve OrderItemChoiceSelection'ları nasıl ele alacağımıza dikkat.
                _context.OptionChoices.Remove(optionChoice);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<OptionChoice>> GetAllOptionChoicesAsync()
        {
            return await _context.OptionChoices
                .OrderBy(oc=>oc.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<OptionChoice>> GetChoicesByGroupIdAsync(Guid optionGroupId)
        {
            return await _context.OptionChoices
                .Where(oc => oc.OptionGroupId == optionGroupId && oc.IsAvailable)
                .OrderBy(oc => oc.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OptionChoice?> GetOptionChoiceByIdAsync(Guid id)
        {
            return await _context.OptionChoices
                .Include(oc => oc.OptionGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync(oc => oc.Id == id);
        }

        public async Task UpdateOptionChoiceAsync(OptionChoice optionChoice)
        {
            if (optionChoice == null) throw new ArgumentNullException(nameof(optionChoice));
            _context.Entry(optionChoice).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.OptionChoices.AllAsync(e => e.Id == optionChoice.Id))
                {
                    throw new KeyNotFoundException("OptionChoice not found.");
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
