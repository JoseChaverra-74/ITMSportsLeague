using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
    {
        Task<IEnumerable<TournamentSponsor>> GetByTournamentIdAsync(int tournamentId);
    }
}