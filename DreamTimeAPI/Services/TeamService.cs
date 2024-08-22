using DreamTeamAPI.Interfaces;
using DreamTeamAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamAPI.Services
{
    public class TeamService(DreamTeamContext context) : ITeamService
    {
        private readonly DreamTeamContext _context =context;
      

        public async Task<bool> Delete(Guid id)
        {
            try {
                var team = await GetById(id);
                if(team is null)
                {
                    return false;
                }
                _context.Teams.Remove(team);  
                return true;
            }
            catch { 
                return false;
            }
        }

        public async Task<IEnumerable<Team>> GetAll()
        {
          return  await _context.Teams.ToListAsync();
        }

        public async Task<Team> GetById(Guid Id)
        {
            var team = await _context.Teams.FindAsync(Id);  
            if(team is null)
            {
                return new NullTeam();
            }
            return team;
        }

        public Guid Save(Team team)
        {
            try
            {
                team.Id = Guid.NewGuid();   
                _context.Teams.Add(team);
                return team.Id;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public bool Update(Team team)
        {
            try
            {
                _context.Teams.Update(team);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
