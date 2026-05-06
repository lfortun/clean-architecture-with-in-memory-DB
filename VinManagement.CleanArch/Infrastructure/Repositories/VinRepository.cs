using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VinRepository : IVinRepository
{
    private readonly AppDbContext _context;

    public VinRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Vin> CreateAsync(Vin vin, CancellationToken cancellationToken = default)
    {
        _context.Vins.Add(vin);
        await _context.SaveChangesAsync(cancellationToken);
        return vin;
    }

    public async Task<Vin?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vins.FindAsync([id], cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<Vin>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vins
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Vin> UpdateAsync(Vin vin, CancellationToken cancellationToken = default)
    {
        _context.Vins.Update(vin);
        await _context.SaveChangesAsync(cancellationToken);
        return vin;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var vin = await _context.Vins.FindAsync([id], cancellationToken: cancellationToken);
        if (vin != null)
        {
            _context.Vins.Remove(vin);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vins.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Vins.AnyAsync(e => e.Code == code, cancellationToken);
    }
}
