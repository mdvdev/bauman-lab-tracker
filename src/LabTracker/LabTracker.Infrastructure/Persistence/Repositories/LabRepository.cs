using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories
{
    public class LabRepository : ILabRepository
    {
        private readonly ApplicationDbContext _context;

        public LabRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Lab?> GetByIdAsync(Guid id)
        {
            return await _context.Labs.FindAsync(id);
        }

        public async Task<IEnumerable<Lab>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.Labs
                .Where(l => l.CourseId == courseId)
                .ToListAsync();
        }

        public async Task CreateAsync(Lab lab)
        {
            await _context.Labs.AddAsync(lab);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Lab lab)
        {
            _context.Labs.Update(lab);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var lab = await _context.Labs.FindAsync(id);
            if (lab != null)
            {
                _context.Labs.Remove(lab);
                await _context.SaveChangesAsync();
            }
        }
    }
}