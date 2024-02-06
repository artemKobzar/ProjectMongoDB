using MongoDB.Bson;
using ProjectMongoDB.Entities;

namespace ProjectMongoDB.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> Get(string id);
        Task Add(User user);
        Task Update(string id, User user);
        Task Delete(string id);
        Task<IEnumerable<User>> GetAllUserWithPassport(string? firstName, string? lastName, string? nationality, string? gender);
        Task<IEnumerable<User>> GetUserWithPassport(string id);
        Task<User> DownloadUserImageById(string id);
    }
}
