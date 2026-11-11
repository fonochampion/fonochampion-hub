using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Infrastructure.Services;

public sealed class ProfessionalManagementService(ApplicationDbContext dbContext)
    : IProfessionalManagementService
{
    public async Task<IReadOnlyList<ProfessionalListItem>> GetProfessionalsAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Professionals
            .AsNoTracking()
            .OrderBy(professional => professional.FullName)
            .Select(professional => new ProfessionalListItem(
                professional.Id,
                professional.FullName,
                professional.Cpf,
                professional.IsActive,
                professional.PatientProfessionals.Count))
            .ToListAsync(cancellationToken);

    public async Task<ProfessionalManagementResult> SetActiveAsync(
        int professionalId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var professional = await dbContext.Professionals
            .SingleOrDefaultAsync(
                professional => professional.Id == professionalId,
                cancellationToken);
        if (professional is null)
        {
            return ProfessionalManagementResult.Failed(
                ProfessionalManagementFailure.ProfessionalNotFound);
        }

        professional.IsActive = isActive;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ProfessionalManagementResult.Success();
    }
}
