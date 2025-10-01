using RandomUserProject.Models;

namespace RandomUserProject.Services
{
    public interface IRandomUserService
    {
        Task<List<User>> GetRandomUsersAsync(int count = 10);
    }
}