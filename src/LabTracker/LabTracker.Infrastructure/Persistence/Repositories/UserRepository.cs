using LabTracker.Application.Contracts;
using LabTracker.Domain.Entities;
using LabTracker.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<UserEntity> _userManager;

    public UserRepository(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User?> GetByIdAsync(Guid key)
    {
        var entity = await _userManager.FindByIdAsync(key.ToString());
        return entity?.ToDomain(await _userManager.GetRolesAsync(entity));
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<User>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(user.ToDomain(roles));
        }

        return result;
    }

    public async Task<Guid> CreateAsync(User user)
    {
        if (await _userManager.FindByIdAsync(user.Id.ToString()) is null)
            await _userManager.CreateAsync(UserEntity.FromDomain(user));

        return user.Id;
    }

    public async Task UpdateAsync(User user)
    {
        var entity = await _userManager.FindByIdAsync(user.Id.ToString());
        if (entity is not null)
        {
            entity.FirstName = user.FirstName.Value;
            entity.LastName = user.LastName.Value;
            entity.Patronymic = user.Patronymic.Value;
            entity.Email = user.Email;
            entity.PhotoUri = user.PhotoUri;
            entity.TelegramUsername = user.TelegramUsername;
            await _userManager.UpdateAsync(entity);
        }
    }

    public async Task DeleteAsync(Guid key)
    {
        var entity = await _userManager.FindByIdAsync(key.ToString());
        if (entity is not null)
            await _userManager.DeleteAsync(entity);
    }
}