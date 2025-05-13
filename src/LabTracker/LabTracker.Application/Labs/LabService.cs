using LabTracker.Application.Abstractions;
using LabTracker.Application.Contracts;
using LabTracker.Application.Contracts.Labs;
using LabTracker.Domain.Entities;

namespace LabTracker.Application.Labs
{
    public class LabService : ILabService
    {
        private readonly ILabRepository _labRepository;
        private readonly ICourseRepository _courseRepository;

        public LabService(ILabRepository labRepository, ICourseRepository courseRepository)
        {
            _labRepository = labRepository;
            _courseRepository = courseRepository;
        }

        public async Task<Lab> CreateLabAsync(Guid courseId, string name, string description, DateTimeOffset deadline, int score, int scoreAfterDeadline)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course is null)
            {
                throw new KeyNotFoundException($"Course with id '{courseId}' not found.");
            }

            var lab = new Lab
            {
                CourseId = courseId,
                Name = name,
                Description = description,
                Deadline = deadline,
                Score = score,
                ScoreAfterDeadline = scoreAfterDeadline
            };

            await _labRepository.CreateAsync(lab);
            return lab;
        }

        public async Task DeleteLabAsync(Guid id)
        {
            await _labRepository.DeleteAsync(id);
        }

        public async Task<Lab?> GetLabByIdAsync(Guid id)
        {
            return await _labRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Lab>> GetLabsByCourseIdAsync(Guid courseId)
        {
            return await _labRepository.GetByCourseIdAsync(courseId);
        }

        public async Task UpdateLabAsync(Guid id, string name, string description, DateTimeOffset deadline, int score, int scoreAfterDeadline)
        {
            var lab = await _labRepository.GetByIdAsync(id);
            if (lab is null)
            {
                throw new KeyNotFoundException($"Lab with id '{id}' not found.");
            }

            lab.Name = name;
            lab.Description = description;
            lab.Deadline = deadline;
            lab.Score = score;
            lab.ScoreAfterDeadline = scoreAfterDeadline;

            await _labRepository.UpdateAsync(lab);
        }
    }
}