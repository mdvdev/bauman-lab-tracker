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
            var lab =  await _context.Labs
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (lab is null)
            {
                throw new KeyNotFoundException($"Lab with {id} id not found");
            }
            return lab;
        }

        public async Task<IEnumerable<Lab>> GetAllAsync()
        {
            var labs = await _context.Labs.ToListAsync();
            if (labs is null)
            {
                throw new KeyNotFoundException("There are no Labs");
            }
            return labs;
        }

        public async Task<IEnumerable<Lab>> GetByCourseIdAsync(Guid courseId)
        {
            var labs =  await _context.Labs
                .Include(l => l.Course) 
                .Where(l => l.CourseId == courseId)
                .ToListAsync();
            if (labs is null)
            {
                throw new KeyNotFoundException("There are no Labs");
            }
            return labs;
        }

        public async Task CreateAsync(Lab lab)
        {
            await _context.Labs.AddAsync(lab);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Lab lab)
        {
            var labToUpdate = await _context.Labs.FindAsync(lab.Id);
            if (labToUpdate is null)
            {
                throw new KeyNotFoundException($"Lab with {lab.Id} id not found");
            }
            labToUpdate.Name = lab.Name;
            labToUpdate.Description = lab.Description;
            labToUpdate.Deadline = lab.Deadline;
            labToUpdate.Score = lab.Score;
            labToUpdate.ScoreAfterDeadline = lab.ScoreAfterDeadline;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var lab = await _context.Labs.FindAsync(id);
            if (lab is null)
            {
                throw new KeyNotFoundException("There are no Labs");
            }
            _context.Labs.Remove(lab);
            await _context.SaveChangesAsync();
        }
    }
}