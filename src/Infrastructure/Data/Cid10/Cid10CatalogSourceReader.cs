using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace FonoChampionHub.Infrastructure.Data.Cid10;

internal sealed class Cid10CatalogSourceReader
{
    private const string ChaptersFile = "CID-10-CAPITULOS.CSV";
    private const string GroupsFile = "CID-10-GRUPOS.CSV";
    private const string CategoriesFile = "CID-10-CATEGORIAS.CSV";
    private const string SubcategoriesFile = "CID-10-SUBCATEGORIAS.CSV";
    private static readonly string[] SourceFileNames =
    [
        ChaptersFile,
        GroupsFile,
        CategoriesFile,
        SubcategoriesFile
    ];

    public Cid10CatalogSource Read(string sourceDirectory)
    {
        var directory = Path.GetFullPath(sourceDirectory);
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException($"CID-10 source directory was not found: {directory}");
        }

        var sourceFiles = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        var chapters = ReadRows(directory, ChaptersFile, sourceFiles, "NUMCAP", "CATINIC", "CATFIM", "DESCRICAO")
            .Select((row, index) => new Cid10ChapterSource(
                ParseChapterNumber(row["NUMCAP"], ChaptersFile, index),
                ParseCode(row["CATINIC"], 3, ChaptersFile, index),
                ParseCode(row["CATFIM"], 3, ChaptersFile, index),
                ParseDescription(row["DESCRICAO"], ChaptersFile, index),
                index))
            .ToArray();

        var groups = ReadRows(directory, GroupsFile, sourceFiles, "CATINIC", "CATFIM", "DESCRICAO")
            .Select((row, index) => new Cid10GroupSource(
                ParseCode(row["CATINIC"], 3, GroupsFile, index),
                ParseCode(row["CATFIM"], 3, GroupsFile, index),
                ParseDescription(row["DESCRICAO"], GroupsFile, index),
                index))
            .ToArray();

        var categories = ReadRows(directory, CategoriesFile, sourceFiles, "CAT", "DESCRICAO")
            .Select((row, index) => new Cid10CodeSource(
                ParseCode(row["CAT"], 3, CategoriesFile, index),
                ParseDescription(row["DESCRICAO"], CategoriesFile, index),
                index))
            .ToArray();

        var subcategories = ReadRows(directory, SubcategoriesFile, sourceFiles, "SUBCAT", "DESCRICAO")
            .Select((row, index) => new Cid10CodeSource(
                ParseCode(row["SUBCAT"], 3, SubcategoriesFile, index, allowedLength: 4),
                ParseDescription(row["DESCRICAO"], SubcategoriesFile, index),
                index))
            .ToArray();

        return new Cid10CatalogSource(ComputeFingerprint(sourceFiles), chapters, groups, categories, subcategories);
    }

    private static IReadOnlyList<Dictionary<string, string>> ReadRows(
        string directory,
        string fileName,
        IDictionary<string, byte[]> sourceFiles,
        params string[] requiredColumns)
    {
        var filePath = Path.Combine(directory, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Required CID-10 source file was not found: {filePath}", filePath);
        }

        var content = File.ReadAllBytes(filePath);
        sourceFiles.Add(fileName, content);
        using var stream = new MemoryStream(content, writable: false);
        // Quotation marks in these source files are literal description text, not CSV quoting.
        using var parser = new TextFieldParser(stream, Encoding.Latin1)
        {
            Delimiters = [";"],
            HasFieldsEnclosedInQuotes = false,
            TrimWhiteSpace = false
        };

        var headers = parser.ReadFields()
            ?? throw new InvalidDataException($"CID-10 source file has no header: {filePath}");
        if (headers.Length > 0)
        {
            headers[0] = headers[0].TrimStart('\uFEFF');
        }

        var headerIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < headers.Length; index++)
        {
            var header = headers[index].Trim();
            if (!string.IsNullOrEmpty(header) && !headerIndexes.TryAdd(header, index))
            {
                throw new InvalidDataException($"Duplicate column '{header}' in {filePath}.");
            }
        }

        foreach (var requiredColumn in requiredColumns)
        {
            if (!headerIndexes.ContainsKey(requiredColumn))
            {
                throw new InvalidDataException(
                    $"Required column '{requiredColumn}' is missing from {filePath}.");
            }
        }

        var rows = new List<Dictionary<string, string>>();
        var rowNumber = 1;
        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields();
            rowNumber++;
            if (fields is null || fields.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            if (fields.Length > headers.Length &&
                fields.Skip(headers.Length).Any(field => !string.IsNullOrWhiteSpace(field)))
            {
                throw new InvalidDataException(
                    $"Unexpected extra column in {filePath} at row {rowNumber}.");
            }

            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var column in requiredColumns)
            {
                var columnIndex = headerIndexes[column];
                row.Add(column, columnIndex < fields.Length ? fields[columnIndex].Trim() : string.Empty);
            }

            rows.Add(row);
        }

        if (rows.Count == 0)
        {
            throw new InvalidDataException($"CID-10 source file contains no data rows: {filePath}");
        }

        return rows;
    }

    private static string ComputeFingerprint(IReadOnlyDictionary<string, byte[]> sourceFiles)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (var fileName in SourceFileNames.Order(StringComparer.Ordinal))
        {
            hash.AppendData(Encoding.UTF8.GetBytes(fileName));
            hash.AppendData([0]);
            hash.AppendData(sourceFiles[fileName]);
            hash.AppendData([0]);
        }

        return Convert.ToHexString(hash.GetHashAndReset());
    }

    private static int ParseChapterNumber(string value, string fileName, int rowIndex)
    {
        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var number) || number < 1)
        {
            throw InvalidValue(fileName, rowIndex, "NUMCAP", value);
        }

        return number;
    }

    private static string ParseCode(
        string value,
        int requiredLength,
        string fileName,
        int rowIndex,
        int? allowedLength = null)
    {
        var code = value.ToUpperInvariant();
        if ((code.Length != requiredLength && code.Length != allowedLength) ||
            code.Any(character => !char.IsAsciiLetterOrDigit(character)))
        {
            throw InvalidValue(fileName, rowIndex, "code", value);
        }

        return code;
    }

    private static string ParseDescription(string value, string fileName, int rowIndex)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw InvalidValue(fileName, rowIndex, "DESCRICAO", value);
        }

        return value;
    }

    private static InvalidDataException InvalidValue(string fileName, int rowIndex, string column, string value) =>
        new($"Invalid {column} value in {fileName} at data row {rowIndex + 1}: '{value}'.");
}
