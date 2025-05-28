using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.Submissions.Abstractions.Repositories;
using LabTracker.Submissions.Domain;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public SubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Submission?> GetByIdAsync(Guid submissionId)
    {
        var entity = await _context.Submissions.FindAsync(submissionId);
        return entity?.ToDomain();
    }

    public async Task<IEnumerable<Submission>> GetAllAsync()
    {
        var entities = await _context.Submissions.ToListAsync();
        return entities.Select(e => e.ToDomain());
    }

    public async Task<IEnumerable<Submission>> GetByCourseIdAsync(Guid courseId)
    {
        var entities = await _context.Submissions
            .Where(s => s.CourseId == courseId)
            .ToListAsync();
        return entities.Select(e => e.ToDomain());
    }

    public async Task<Submission> CreateAsync(Submission submission)
    {
        if (await _context.Submissions.FindAsync(submission.Id) is null)
        {
            await _context.Submissions.AddAsync(SubmissionEntity.FromDomain(submission));
            await _context.SaveChangesAsync();
        }

        return submission;
    }

    public async Task<Submission> UpdateAsync(Submission submission)
    {
        var entity = await _context.Submissions.FindAsync(submission.Id);

        if (entity is null) return await CreateAsync(submission);

        entity.CourseId = submission.CourseId;
        entity.StudentId = submission.StudentId;
        entity.LabId = submission.LabId;
        entity.SlotId = submission.SlotId;
        entity.SubmissionStatus = submission.SubmissionStatus;
        entity.Comment = submission.Comment;
        entity.UpdatedAt = submission.UpdatedAt;
        entity.CreatedAt = submission.CreatedAt;

        _context.Submissions.Update(entity);
        await _context.SaveChangesAsync();

        return entity.ToDomain();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Submissions.FindAsync(id);
        if (entity is not null)
        {
            _context.Submissions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}