using PenSword.Models;
using Microsoft.AspNetCore.Identity;

namespace PenSword.Services.Interfaces
{
    public interface IRoleService
    {
        public Task<bool> AddUserToRoleAsync(string? userId, string? roleName);
        public Task<List<IdentityRole>> GetRolesAsync();
        public Task<IEnumerable<string>> GetUserRolesAsync(string? userId);
        public Task<List<BlogUser>> GetUsersInRoleAsync(string? roleName);
        public Task<bool> IsUserInRoleAsync(string? userId, string? roleName);
        public Task<bool> RemoveUserFromRoleAsync(string? userId, string? roleName);
        public Task<bool> RemoveUserFromRolesAsync(string? userId, IEnumerable<string>? roleNames);
    }
}
