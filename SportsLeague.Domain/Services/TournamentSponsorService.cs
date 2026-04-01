using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class TournamentSponsorService : ITournamentSponsorService
    {
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ILogger<TournamentSponsorService> _logger;

        public TournamentSponsorService(
            ITournamentSponsorRepository tournamentSponsorRepository,
            ITournamentRepository tournamentRepository,
            ISponsorRepository sponsorRepository,
            ILogger<TournamentSponsorService> logger)
        {
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _tournamentRepository = tournamentRepository;
            _sponsorRepository = sponsorRepository;
            _logger = logger;
        }

        public async Task<TournamentSponsor?> RegisterSponsorAsync(TournamentSponsor tournamentSponsor)
        {
            var tournament = await _tournamentRepository
                .GetByIdAsync(tournamentSponsor.TournamentId);
            if (tournament == null)
                throw new KeyNotFoundException($"No se encontró el torneo con ID {tournamentSponsor.TournamentId}");

            var sponsorExists = await _sponsorRepository
                .ExistsAsync(tournamentSponsor.SponsorId);
            if (!sponsorExists)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {tournamentSponsor.SponsorId}");

            var existingRegister = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentSponsor.TournamentId, tournamentSponsor.SponsorId);
            if (existingRegister != null)
            {
                throw new InvalidOperationException("Este sponsor ya está inscrito en el torneo");
            }

            if (tournament.Status == TournamentStatus.Finished)
            {
                throw new InvalidOperationException
                    ("Solo se pueden registrar sponsors en torneos con estado Pending o InProgress ");
            }

            if (tournamentSponsor.ContractAmount <= 0)
            {
                throw new InvalidOperationException("El valor del contrato debe ser superior a 0.");
            }

            _logger.LogInformation
                ($"Registering sponsor with ID: {tournamentSponsor.SponsorId} in tournament with ID: {tournamentSponsor.TournamentId}");
            tournamentSponsor.JoinedAt = DateTime.UtcNow;
            await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);
            return await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentSponsor.TournamentId, tournamentSponsor.SponsorId);

        }

        public async Task<IEnumerable<TournamentSponsor>> GetBySponsorAsync(int sponsorId)
        {
            var sponsorExists = await _sponsorRepository
               .ExistsAsync(sponsorId);
            if (!sponsorExists)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {sponsorId}");
           
            return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);

        }

        public async Task UnregisterSponsorAsync(int tournamentId, int sponsorId)
        {
            var existingRegister = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (existingRegister == null)
            {
                throw new KeyNotFoundException
                    ($"No hay registro existente entre el sponsor con ID: {sponsorId} y el torneo con ID: {tournamentId}");
            }

            _logger.LogInformation
                ($"Deleting sponsor {sponsorId} from tournament {tournamentId}");
            await _tournamentSponsorRepository.DeleteByTournamentAndSponsorAsync(tournamentId, sponsorId);
        }
    }
}
