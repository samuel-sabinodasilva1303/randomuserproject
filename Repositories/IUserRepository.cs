using RandomUserProject.Models;

namespace RandomUserProject.Repositories
{
     /// <summary>
    /// Autor: Samuel Sabino - 30/09/2025
    /// Descrição: class responsavel por retornar dados do banco e definir metodos.
    /// </summary>
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> AddAsync(User user);
        Task AddRangeAsync(IEnumerable<User> users);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<int> CountAsync();
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
    }
}
