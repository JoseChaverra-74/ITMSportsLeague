using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
            var sponsor = await _sponsorRepository.GetByIdAsync(id);

            if (sponsor == null)
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

            return sponsor;
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            //No se puede crear un Sponsor con Name duplicado
            var existingSponsor = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (existingSponsor == true)
            {
                _logger.LogWarning("Sponsor with name '{SponsorName}' already exists", sponsor.Name);
                throw new InvalidOperationException(
                    $"Ya existe un sponsor con el nombre '{sponsor.Name}'");
            }
            
            //ContactEmail debe ser un formato válido
            if (!string.IsNullOrEmpty(sponsor.ContactEmail))
            {
                var emailValidator = new EmailAddressAttribute();
                if (!emailValidator.IsValid(sponsor.ContactEmail))
                {
                    throw new InvalidOperationException(
                        $"El formato del email {sponsor.ContactEmail} no es válido.");
                }
            }
            
            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"No se encontró el sponsor con ID: {id}");
            }

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;

            _logger.LogInformation($"Updating sponsor with ID: {id}");
            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);
            if (!exists)
            {
                _logger.LogInformation($"Sponsor with ID: {id} not founded for deletion.");
                throw new KeyNotFoundException($"No se encontró el sponsor con ID: {id}");
            }

            _logger.LogInformation($"Deleting sponsor with ID {id}");
            await _sponsorRepository.DeleteAsync(id);
        }
    }
}