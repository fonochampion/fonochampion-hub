using FonoChampionHub.Domain.Entities;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Web.Controllers;
using FonoChampionHub.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FonoChampionHub.Web.Tests;

public sealed class CancellationReasonsManagementTests
{
    [Fact]
    public async Task Create_trims_name_and_saves_an_active_reason()
    {
        await using var dbContext = CreateDbContext();
        var controller = CreateController(dbContext);

        var result = await controller.Create(
            new CreateCancellationReasonViewModel
            {
                Name = "  Motivo personalizado  ",
                Description = "Descrição opcional"
            },
            CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(result);
        var reason = await dbContext.AttendanceCancellationReasons.SingleAsync();
        Assert.Equal("Motivo personalizado", reason.Name);
        Assert.Equal("MOTIVO PERSONALIZADO", reason.NormalizedName);
        Assert.Equal("Descrição opcional", reason.Description);
        Assert.True(reason.IsActive);
    }

    [Fact]
    public async Task Create_rejects_name_that_is_empty_after_trimming()
    {
        await using var dbContext = CreateDbContext();
        var controller = CreateController(dbContext);

        var result = await controller.Create(
            new CreateCancellationReasonViewModel { Name = "   " },
            CancellationToken.None);

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.Empty(await dbContext.AttendanceCancellationReasons.ToListAsync());
    }

    [Fact]
    public async Task Create_rejects_case_and_trim_duplicates_even_when_existing_reason_is_inactive()
    {
        await using var dbContext = CreateDbContext();
        dbContext.AttendanceCancellationReasons.Add(new AttendanceCancellationReason
        {
            Name = "Razão teste",
            NormalizedName = AttendanceCancellationReason.NormalizeNameForComparison("Razão teste"),
            IsActive = false
        });
        await dbContext.SaveChangesAsync();
        var controller = CreateController(dbContext);

        var duplicate = await controller.Create(
            new CreateCancellationReasonViewModel { Name = "  razão TESTE  " },
            CancellationToken.None);
        Assert.IsType<ViewResult>(duplicate);
        Assert.Contains(
            controller.ModelState[nameof(CancellationReasonFormViewModel.Name)]!.Errors,
            error => error.ErrorMessage.Contains("Já existe", StringComparison.Ordinal));

        var differentAccent = await controller.Create(
            new CreateCancellationReasonViewModel { Name = "Razao teste" },
            CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(differentAccent);
        Assert.Equal(2, await dbContext.AttendanceCancellationReasons.CountAsync());
    }

    [Fact]
    public async Task Edit_preserves_inactive_state()
    {
        await using var dbContext = CreateDbContext();
        var reason = new AttendanceCancellationReason
        {
            Name = "Motivo antigo",
            NormalizedName = "MOTIVO ANTIGO",
            Description = "Descrição antiga",
            IsActive = false
        };
        dbContext.AttendanceCancellationReasons.Add(reason);
        await dbContext.SaveChangesAsync();
        var controller = CreateController(dbContext);

        var result = await controller.Edit(
            new EditCancellationReasonViewModel
            {
                Id = reason.Id,
                Name = " Motivo atualizado ",
                Description = "Descrição atualizada"
            },
            CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Motivo atualizado", reason.Name);
        Assert.Equal("MOTIVO ATUALIZADO", reason.NormalizedName);
        Assert.Equal("Descrição atualizada", reason.Description);
        Assert.False(reason.IsActive);
    }

    [Fact]
    public async Task Edit_rejects_a_name_used_by_another_reason()
    {
        await using var dbContext = CreateDbContext();
        var existingReason = new AttendanceCancellationReason
        {
            Name = "Motivo existente",
            NormalizedName = "MOTIVO EXISTENTE"
        };
        var editedReason = new AttendanceCancellationReason
        {
            Name = "Motivo original",
            NormalizedName = "MOTIVO ORIGINAL"
        };
        dbContext.AttendanceCancellationReasons.AddRange(existingReason, editedReason);
        await dbContext.SaveChangesAsync();
        var controller = CreateController(dbContext);

        var result = await controller.Edit(
            new EditCancellationReasonViewModel
            {
                Id = editedReason.Id,
                Name = "  MOTIVO EXISTENTE "
            },
            CancellationToken.None);

        Assert.IsType<ViewResult>(result);
        Assert.Equal("Motivo original", editedReason.Name);
        Assert.Contains(
            controller.ModelState[nameof(CancellationReasonFormViewModel.Name)]!.Errors,
            error => error.ErrorMessage.Contains("Já existe", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Inactivate_keeps_the_reason_record()
    {
        await using var dbContext = CreateDbContext();
        var reason = new AttendanceCancellationReason
        {
            Name = "Motivo",
            NormalizedName = "MOTIVO"
        };
        dbContext.AttendanceCancellationReasons.Add(reason);
        await dbContext.SaveChangesAsync();
        var controller = CreateController(dbContext);

        var result = await controller.Inactivate(reason.Id, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.False(reason.IsActive);
        Assert.Equal(1, await dbContext.AttendanceCancellationReasons.CountAsync());
    }

    [Fact]
    public async Task Index_lists_active_and_inactive_reasons()
    {
        await using var dbContext = CreateDbContext();
        dbContext.AttendanceCancellationReasons.AddRange(
            new AttendanceCancellationReason
            {
                Name = "Razão ativa",
                NormalizedName = "RAZÃO ATIVA",
                IsActive = true
            },
            new AttendanceCancellationReason
            {
                Name = "Razão inativa",
                NormalizedName = "RAZÃO INATIVA",
                IsActive = false
            });
        await dbContext.SaveChangesAsync();
        var controller = CreateController(dbContext);

        var result = await controller.Index(CancellationToken.None);

        var model = Assert.IsType<CancellationReasonListViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Equal(2, model.Reasons.Count);
        Assert.Contains(model.Reasons, reason => reason.IsActive);
        Assert.Contains(model.Reasons, reason => !reason.IsActive);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static CancellationReasonsController CreateController(ApplicationDbContext dbContext)
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddControllersWithViews()
            .Services
            .BuildServiceProvider();
        var controller = new CancellationReasonsController(dbContext)
        {
            ControllerContext = new ControllerContext
            {
                ActionDescriptor = new ControllerActionDescriptor(),
                HttpContext = new DefaultHttpContext { RequestServices = services },
                RouteData = new RouteData()
            }
        };
        return controller;
    }
}
