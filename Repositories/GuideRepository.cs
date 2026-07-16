using Microsoft.EntityFrameworkCore;
using LayerManager.API.Data;
using LayerManager.API.Models;
using LayerManager.API.Repositories.Interfaces;

namespace LayerManager.API.Repositories;

public class GuideRepository : IGuideRepository
{
    private readonly AppDbContext _context;

    public GuideRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MapGuide>> GetAllAsync()
    {
        return await _context.MapGuides
            .OrderBy(g => g.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<MapGuide?> GetByIdAsync(Guid id)
    {
        return await _context.MapGuides
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<MapGuide> CreateAsync(MapGuide guide)
    {
        _context.MapGuides.Add(guide);
        await _context.SaveChangesAsync();
        return guide;
    }

    public async Task UpdateAsync(MapGuide guide)
    {
        _context.MapGuides.Update(guide);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.MapGuides.FindAsync(id);
        if (entity is not null)
        {
            _context.MapGuides.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}