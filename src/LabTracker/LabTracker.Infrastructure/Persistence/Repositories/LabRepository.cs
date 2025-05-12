using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Infrastructure.Persistence.Entities;
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
            var entity = await _context.Labs
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);

            return entity?.ToDomain();
        }

        public async Task<IEnumerable<Lab>> GetAllAsync()
        {
            var entities = await _context.Labs
                .Include(l => l.Course)
                .ToListAsync();

            return entities.Select(e => e.ToDomain());
        }

        public async Task<IEnumerable<Lab>> GetByCourseIdAsync(Guid courseId)
        {
            var entities = await _context.Labs
                .Include(l => l.Course)
                .Where(l => l.CourseId == courseId)
                .ToListAsync();

            return entities.Select(e => e.ToDomain());
        }

        public async Task<Guid> CreateAsync(Lab lab)
        {
            var entity = LabEntity.FromDomain(lab);
            await _context.Labs.AddAsync(entity);
            await _context.SaveChangesAsync();
            return lab.Id;
        }

        public async Task UpdateAsync(Lab lab)
        {
            var entity = await _context.Labs.FindAsync(lab.Id);
            if (entity != null)
            {
                entity.Name = lab.Name;
                entity.Description = lab.Description;
                entity.Deadline = lab.Deadline;
                entity.Score = lab.Score;
                entity.ScoreAfterDeadline = lab.ScoreAfterDeadline;
                
                if (lab.Course is not null && (entity.Course is null || entity.Course.Id != lab.Course.Id))
                {
                    entity.Course = await _context.Courses.FindAsync(lab.Course.Id);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Labs.FindAsync(id);
            if (entity is not null)
            {
                _context.Labs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}