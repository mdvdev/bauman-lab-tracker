using LabTracker.Infrastructure.Persistence.Entities;
using LabTracker.User.Abstractions.Repositories;
using LabTracker.Users.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LabTracker.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public UserRepository(UserManager<UserEntity> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Users.Domain.User?> GetByIdAsync(Guid labId)
    {
        var entity = await _userManager.FindByIdAsync(labId.ToString());
        return entity?.ToDomain(await _userManager.GetRolesAsync(entity));
    }

    public async Task<IEnumerable<Users.Domain.User>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<Users.Domain.User>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(user.ToDomain(roles));
        }

        return result;
    }

    public async Task<Users.Domain.User> CreateAsync(Users.Domain.User user)
    {
        if (await _userManager.FindByIdAsync(user.Id.ToString()) is null)
            await _userManager.CreateAsync(UserEntity.FromDomain(user));

        return user;
    }

    public async Task<Users.Domain.User> UpdateAsync(Users.Domain.User user)
    {
        var entity = await _userManager.FindByIdAsync(user.Id.ToString());

        if (entity is null) return await CreateAsync(user);

        entity.FirstName = user.FirstName;
        entity.LastName = user.LastName;
        entity.Group = user.Group;
        entity.Patronymic = user.Patronymic;
        entity.Email = user.Email;
        entity.PhotoUri = user.PhotoUri;
        entity.TelegramUsername = user.TelegramUsername;

        await _userManager.UpdateAsync(entity);

        return entity.ToDomain(user.Roles.Select(r => r.ToString()));
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _userManager.FindByIdAsync(id.ToString());
        if (entity is not null)
            await _userManager.DeleteAsync(entity);
    }

    public async Task AddRoleToUserAsync(Guid userId, Role role)
    {
        if (await _roleManager.RoleExistsAsync(role.ToString()) is false)
            await _roleManager.CreateAsync(new IdentityRole<Guid>(role.ToString()));
        
        var entity = await _userManager.FindByIdAsync(userId.ToString());
        if (entity is not null)
            await _userManager.AddToRoleAsync(entity, role.ToString());
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Role role)
    {
        if (await _roleManager.RoleExistsAsync(role.ToString()) is false)
            return;
        
        var entity = await _userManager.FindByIdAsync(userId.ToString());
        if (entity is not null)
            await _userManager.RemoveFromRoleAsync(entity, role.ToString());
    }
}