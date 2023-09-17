using Microsoft.AspNetCore.Identity;
using PenSword.Data;
using PenSword.Services.Interfaces;
using PenSword.Models;
using Microsoft.EntityFrameworkCore;

namespace PenSword.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<BlogUser> _userMananger;
        private readonly ApplicationDbContext _context;

        public RoleService(UserManager<BlogUser> userMananger,
            ApplicationDbContext context)
        {
            _userMananger = userMananger;
            _context = context;
        }

        public async Task<bool> AddUserToRoleAsync(string? userId, string? roleName)
        {
            try
            {
                BlogUser? user = await _context.Users
                    .FirstOrDefaultAsync(b => b.Id == userId);

                if (user != null && !string.IsNullOrEmpty(roleName))
                {
                    bool result = (await _userMananger.AddToRoleAsync(user, roleName)).Succeeded;
                    return result;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                List<IdentityRole> result = new();
                result = await _context.Roles.ToListAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string? userId)
        {
            try
            {
                BlogUser? user = await _context.Users
                    .FirstOrDefaultAsync(b => b.Id == userId);
                if (user != null)
                {
                    IEnumerable<string> result = await _userMananger.GetRolesAsync(user);
                    return result;
                }
                return Enumerable.Empty<string>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BlogUser>> GetUsersInRoleAsync(string? roleName)
        {
            try
            {
                if (!string.IsNullOrEmpty(roleName))
                {
                    return (await _userMananger.GetUsersInRoleAsync(roleName)).ToList();
                }
                else return new List<BlogUser>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsUserInRoleAsync(string? userId, string? roleName)
        {
            try
            {
                BlogUser? user = await _context.Users
                    .FirstOrDefaultAsync(b => b.Id == userId);
                if (user != null && !string.IsNullOrEmpty(roleName))
                    return await _userMananger.IsInRoleAsync(user, roleName);
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RemoveUserFromRoleAsync(string? userId, string? roleName)
        {
            try
            {
                BlogUser? user = await _context.Users
                    .FirstOrDefaultAsync(b => b.Id == userId);
                if (user != null && !string.IsNullOrEmpty(roleName))
                    return (await _userMananger.RemoveFromRoleAsync(user, roleName)).Succeeded;
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RemoveUserFromRolesAsync(string? userId, IEnumerable<string>? roleNames)
        {
            try
            {
                BlogUser? user = await _context.Users
                    .FirstOrDefaultAsync(b => b.Id == userId);
                if (user != null && roleNames != null) return (await _userMananger.RemoveFromRolesAsync(user, roleNames)).Succeeded;
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
