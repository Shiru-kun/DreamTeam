using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DreamTeamAPI.Models;
using DreamTeamAPI.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace DreamTeamAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController(ITeamService teamService, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly ITeamService _teamService = teamService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return  Ok(await _teamService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(Guid id)
        {
            var team = await _teamService.GetById(id);

            if (team is NullTeam)
            {
                return NotFound();
            }

            return Ok(team);
        }

       [HttpPut]
        public async Task<IActionResult> PutTeam(Team team)
        {

            bool isUpdated = _teamService.Update(team);
            await _unitOfWork.Commit();
            return Ok(isUpdated);
        }

      [HttpPost]
        public async Task<ActionResult<Guid>> PostTeam(Team team)
        {
           var id = _teamService.Save(team);
            await _unitOfWork.Commit();
            return Ok(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            var isDeleted = await _teamService.Delete(id);
            await _unitOfWork.Commit();
            return Ok(isDeleted);
        }

      
    }
}
