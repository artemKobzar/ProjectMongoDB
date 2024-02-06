using MongoDB.Bson;
using ProjectMongoDB.Entities;

namespace ProjectMongoDB.Repositories
{
    public interface IPassportUserRepository
    {
        Task<IEnumerable<PassportUser>> GetAll();
        Task<PassportUser> Get(string id);
        Task Add(PassportUser user);
        Task Update(string id, PassportUser user);
        Task Delete(string id);
    }
}
