using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/match/{matchId}/lineup")]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _lineupService;
        private readonly IMapper _mapper;

        public MatchLineupController(IMatchLineupService lineupService, IMapper mapper)
        {
            _lineupService = lineupService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MatchLineupResponseDTO>> AddToLineup(
            int matchId, CreateMatchLineupRequestDTO dto)
        {
            try
            {
                var lineup = _mapper.Map<MatchLineup>(dto);
                var created = await _lineupService.AddToLineupAsync(matchId, lineup);
                var createdWithDetails = await _lineupService.GetLineupByMatchAsync(matchId);
                var entry = createdWithDetails.FirstOrDefault(l => l.Id == created.Id);
                return CreatedAtAction(
                    nameof(GetLineup),
                    new { matchId },
                    _mapper.Map<MatchLineupResponseDTO>(entry));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineup(int matchId)
        {
            try
            {
                var lineups = await _lineupService.GetLineupByMatchAsync(matchId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineups));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpGet("team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchLineupResponseDTO>>> GetLineupByTeam(
            int matchId, int teamId)
        {
            try
            {
                var lineups = await _lineupService.GetLineupByMatchAndTeamAsync(matchId, teamId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupResponseDTO>>(lineups));
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFromLineup(int matchId, int id)
        {
            try
            {
                await _lineupService.DeleteFromLineupAsync(matchId, id);
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }
    }
}
