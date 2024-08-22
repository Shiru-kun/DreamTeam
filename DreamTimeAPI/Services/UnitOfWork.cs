using DreamTeamAPI.Interfaces;
using DreamTeamAPI.Models;

namespace DreamTeamAPI.Services
{
    public class UnitOfWork(DreamTeamContext context) : IUnitOfWork
    {
        private readonly DreamTeamContext _context = context;   
        public async Task Commit()
        {
            await _context.SaveChangesAsync();   
        }
    }
}
