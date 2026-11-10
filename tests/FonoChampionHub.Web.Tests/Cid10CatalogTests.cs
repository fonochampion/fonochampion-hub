using System.Text;
using FonoChampionHub.Domain.Enums;
using FonoChampionHub.Infrastructure.Data.Context;
using FonoChampionHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FonoChampionHub.Web.Tests;

public sealed class Cid10CatalogTests
{
    [Fact]
    public async Task Import_skips_matching_three_character_duplicates_and_searches_code_and_description()
    {
        using var source = new TemporarySourceDirectory();
        source.WriteValidCatalog(includeNarrowGroup: true);
        await using var dbContext = CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();

        var importer = new Cid10CatalogImporter(dbContext);
        var firstImport = await importer.ImportAsync(source.Path);

        Assert.False(firstImport.WasAlreadyCurrent);
        Assert.Equal(1, firstImport.ChapterCount);
        Assert.Equal(2, firstImport.GroupCount);
        Assert.Equal(1, firstImport.CategoryCount);
        Assert.Equal(1, firstImport.SubcategoryCount);
        Assert.Equal(5, firstImport.TotalEntryCount);

        var queryService = new Cid10CatalogQueryService(dbContext);
        var unpunctuatedCodeResults = await queryService.SearchAsync("A000");
        var punctuatedCodeResults = await queryService.SearchAsync("A00.0");
        var descriptionResults = await queryService.SearchAsync("colera classica");

        var result = Assert.Single(unpunctuatedCodeResults);
        Assert.Equal("A00.0", result.Code);
        Assert.Equal("A00.0", Assert.Single(punctuatedCodeResults).Code);
        Assert.Equal("A00.0", Assert.Single(descriptionResults).Code);
        Assert.Equal(
            [Cid10CatalogEntryLevel.Chapter, Cid10CatalogEntryLevel.Group, Cid10CatalogEntryLevel.Category],
            result.Ancestors.Select(ancestor => ancestor.Level));
        Assert.Equal("Grupo específico", result.Ancestors[1].Description);

        var repeatedImport = await importer.ImportAsync(source.Path);
        Assert.True(repeatedImport.WasAlreadyCurrent);
        Assert.Equal(5, await dbContext.Cid10CatalogEntries.CountAsync());
    }

    [Fact]
    public async Task Import_rejects_a_group_without_a_chapter_parent_before_persisting()
    {
        using var source = new TemporarySourceDirectory();
        source.WriteValidCatalog(groupStart: "B00", groupEnd: "B09");
        await using var dbContext = CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();

        var importer = new Cid10CatalogImporter(dbContext);

        await Assert.ThrowsAsync<InvalidDataException>(() => importer.ImportAsync(source.Path));
        Assert.Empty(await dbContext.Cid10CatalogVersions.ToListAsync());
        Assert.Empty(await dbContext.Cid10CatalogEntries.ToListAsync());
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private sealed class TemporarySourceDirectory : IDisposable
    {
        public string Path { get; } = Directory.CreateTempSubdirectory("cid10-import-test-").FullName;

        public void WriteValidCatalog(
            string groupStart = "A00",
            string groupEnd = "A09",
            bool includeNarrowGroup = false)
        {
            Write(
                "CID-10-CAPITULOS.CSV",
                "NUMCAP;CATINIC;CATFIM;DESCRICAO;DESCRABREV;",
                "1;A00;A09;Doenças bacterianas;Doenças;");
            var groupRows = new List<string>
            {
                $"{groupStart};{groupEnd};Infecções intestinais;Infecções;"
            };
            if (includeNarrowGroup)
            {
                groupRows.Add("A00;A02;Grupo específico;Grupo;");
            }

            Write(
                "CID-10-GRUPOS.CSV",
                "CATINIC;CATFIM;DESCRICAO;DESCRABREV;",
                [.. groupRows]);
            Write(
                "CID-10-CATEGORIAS.CSV",
                "CAT;CLASSIF;DESCRICAO;DESCRABREV;REFER;EXCLUIDOS;",
                "A00;9;Cólera;Cólera;;;");
            Write(
                "CID-10-SUBCATEGORIAS.CSV",
                "SUBCAT;CLASSIF;RESTRSEXO;CAUSAOBITO;DESCRICAO;DESCRABREV;REFER;EXCLUIDOS;",
                "A000;9;;N;Cólera clássica;Cólera;;;",
                "A00;9;;N;Cólera;Cólera;;;");
        }

        public void Dispose() => Directory.Delete(Path, recursive: true);

        private void Write(string fileName, string header, params string[] rows) =>
            File.WriteAllText(
                System.IO.Path.Combine(Path, fileName),
                string.Join(Environment.NewLine, [header, .. rows]),
                Encoding.Latin1);
    }
}
