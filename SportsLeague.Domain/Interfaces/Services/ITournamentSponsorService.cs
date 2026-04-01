using SportsLeague.Domain.Entities; 

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ITournamentSponsorService
    {
        Task RegisterSponsorAsync(TournamentSponsor tournamentSponsor);
        Task<IEnumerable<TournamentSponsor>> GetBySponsorAsync(int sponsorId);
        Task UnregisterSponsorAsync(int tournamentId, int sponsorId);
    }
}
