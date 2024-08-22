using DreamTeamAPI.Models;

namespace DreamTeamAPI.Interfaces
{
    public interface ITeamService
    {
        public Guid Save(Team team);
        public bool Update(Team team);
        public Task<bool> Delete(Guid Id);
        public Task<Team?> GetById(Guid Id);
        public Task<IEnumerable<Team>> GetAll();
    }
}
