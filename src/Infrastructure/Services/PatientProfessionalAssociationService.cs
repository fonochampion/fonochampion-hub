using FonoChampionHub.Application.DTOs;
using FonoChampionHub.Application.Interfaces;
using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Infrastructure.Services;

public sealed class PatientProfessionalAssociationService(ApplicationDbContext dbContext)
    : IPatientProfessionalAssociationService
{
    public async Task<IReadOnlyList<ProfessionalOption>> GetRegistrationOptionsAsync(
        CancellationToken cancellationToken = default) =>
        await GetAllProfessionalsAsync(cancellationToken);

    public async Task<IReadOnlyList<PatientListItem>> GetPatientsAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Patients
            .AsNoTracking()
            .OrderBy(patient => patient.FullName)
            .Select(patient => new PatientListItem(
                patient.Id,
                patient.FullName,
                patient.DateOfBirth,
                patient.Cid10CatalogEntry.Code!,
                patient.Cid10CatalogEntry.Description,
                patient.PatientProfessionals.Count))
            .ToListAsync(cancellationToken);

    public async Task<PatientProfessionalManagement?> GetManagementAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        var patient = await dbContext.Patients
            .AsNoTracking()
            .Where(patient => patient.Id == patientId)
            .Select(patient => new
            {
                patient.FullName,
                Associated = patient.PatientProfessionals
                    .OrderBy(association => association.Professional.FullName)
                    .Select(association => new ProfessionalOption(
                        association.ProfessionalId,
                        association.Professional.FullName,
                        association.Professional.IsActive))
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (patient is null)
        {
            return null;
        }

        var associatedIds = patient.Associated
            .Select(professional => professional.Id)
            .ToArray();
        var available = await dbContext.Professionals
            .AsNoTracking()
            .Where(professional =>
                professional.IsActive &&
                !associatedIds.Contains(professional.Id))
            .OrderBy(professional => professional.FullName)
            .Select(professional => new ProfessionalOption(
                professional.Id,
                professional.FullName,
                professional.IsActive))
            .ToListAsync(cancellationToken);

        return new PatientProfessionalManagement(
            patient.FullName,
            patient.Associated,
            available);
    }

    public async Task<IReadOnlyList<ProfessionalOption>?> GetSchedulingOptionsAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        var associated = await dbContext.PatientProfessionals
            .AsNoTracking()
            .Where(association =>
                association.PatientId == patientId &&
                association.Professional.IsActive)
            .OrderBy(association => association.Professional.FullName)
            .Select(association => new ProfessionalOption(
                association.ProfessionalId,
                association.Professional.FullName,
                association.Professional.IsActive))
            .ToListAsync(cancellationToken);

        if (associated.Count > 0)
        {
            return associated;
        }

        var patientExists = await dbContext.Patients
            .AsNoTracking()
            .AnyAsync(patient => patient.Id == patientId, cancellationToken);
        return patientExists ? await GetAllProfessionalsAsync(cancellationToken) : null;
    }

    public async Task<PatientProfessionalAssociationResult> AddToPatientAsync(
        int patientId,
        IReadOnlyCollection<int>? professionalIds,
        CancellationToken cancellationToken = default)
    {
        var ids = professionalIds?.Distinct().ToArray() ?? [];
        if (ids.Length == 0)
        {
            return PatientProfessionalAssociationResult.Failed(
                PatientProfessionalAssociationFailure.NoProfessionalsSelected);
        }

        var patient = await dbContext.Patients
            .SingleOrDefaultAsync(patient => patient.Id == patientId, cancellationToken);
        if (patient is null)
        {
            return PatientProfessionalAssociationResult.Failed(
                PatientProfessionalAssociationFailure.PatientNotFound);
        }

        var professionals = await dbContext.Professionals
            .Where(professional =>
                professional.IsActive &&
                ids.Contains(professional.Id))
            .ToListAsync(cancellationToken);
        if (professionals.Count != ids.Length)
        {
            return PatientProfessionalAssociationResult.Failed(
                PatientProfessionalAssociationFailure.InvalidProfessional);
        }

        var existingIds = await dbContext.PatientProfessionals
            .Where(association => association.PatientId == patientId)
            .Select(association => association.ProfessionalId)
            .ToListAsync(cancellationToken);
        var existingIdSet = existingIds.ToHashSet();
        foreach (var professional in professionals)
        {
            if (existingIdSet.Add(professional.Id))
            {
                dbContext.PatientProfessionals.Add(new PatientProfessional
                {
                    PatientId = patientId,
                    ProfessionalId = professional.Id
                });
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return PatientProfessionalAssociationResult.Success();
    }

    public async Task<PatientProfessionalAssociationResult> AssociateForSchedulingAsync(
        int patientId,
        IReadOnlyCollection<int>? professionalIds,
        CancellationToken cancellationToken = default)
    {
        var ids = professionalIds?.Distinct().ToArray() ?? [];
        if (ids.Length == 0)
        {
            return PatientProfessionalAssociationResult.Failed(
                PatientProfessionalAssociationFailure.NoProfessionalsSelected);
        }

        var patientExists = await dbContext.Patients
            .AsNoTracking()
            .AnyAsync(patient => patient.Id == patientId, cancellationToken);
        if (!patientExists)
        {
            return PatientProfessionalAssociationResult.Failed(
                PatientProfessionalAssociationFailure.PatientNotFound);
        }

        var associatedIds = await dbContext.PatientProfessionals
            .Where(association =>
                association.PatientId == patientId &&
                association.Professional.IsActive)
            .Select(association => association.ProfessionalId)
            .ToListAsync(cancellationToken);
        var allowedIds = associatedIds.Count > 0
            ? associatedIds
            : await dbContext.Professionals
                .Where(professional => professional.IsActive)
                .Select(professional => professional.Id)
                .ToListAsync(cancellationToken);
        var allowedIdSet = allowedIds.ToHashSet();
        if (ids.Any(id => !allowedIdSet.Contains(id)))
        {
            return PatientProfessionalAssociationResult.Failed(
                PatientProfessionalAssociationFailure.InvalidProfessional);
        }

        if (associatedIds.Count == 0)
        {
            dbContext.PatientProfessionals.AddRange(ids.Select(id => new PatientProfessional
            {
                PatientId = patientId,
                ProfessionalId = id
            }));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return PatientProfessionalAssociationResult.Success();
    }

    private async Task<IReadOnlyList<ProfessionalOption>> GetAllProfessionalsAsync(
        CancellationToken cancellationToken) =>
        await dbContext.Professionals
            .AsNoTracking()
            .Where(professional => professional.IsActive)
            .OrderBy(professional => professional.FullName)
            .Select(professional => new ProfessionalOption(
                professional.Id,
                professional.FullName,
                professional.IsActive))
            .ToListAsync(cancellationToken);
}
