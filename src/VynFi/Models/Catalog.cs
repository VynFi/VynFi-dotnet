using System.Text.Json.Serialization;

namespace VynFi.Models;

public class Column
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("data_type")] public string DataType { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("nullable")] public bool Nullable { get; set; }
}

public class TableDef
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("base_rate")] public double BaseRate { get; set; } = 1.0;
    [JsonPropertyName("columns")] public List<Column> Columns { get; set; } = new();
}

public class Sector
{
    [JsonPropertyName("slug")] public string Slug { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("icon")] public string Icon { get; set; } = "";
    [JsonPropertyName("multiplier")] public double Multiplier { get; set; } = 1.0;
    [JsonPropertyName("quality_score")] public double QualityScore { get; set; }
    [JsonPropertyName("popularity")] public int Popularity { get; set; }
    [JsonPropertyName("tables")] public List<TableDef> Tables { get; set; } = new();
}

public class SectorSummary
{
    [JsonPropertyName("slug")] public string Slug { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("icon")] public string Icon { get; set; } = "";
    [JsonPropertyName("table_count")] public int TableCount { get; set; }
}

public class CatalogItem
{
    [JsonPropertyName("sector")] public string Sector { get; set; } = "";
    [JsonPropertyName("profile")] public string Profile { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("source")] public string Source { get; set; } = "";
}

public class Fingerprint
{
    [JsonPropertyName("sector")] public string Sector { get; set; } = "";
    [JsonPropertyName("profile")] public string Profile { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("source")] public string Source { get; set; } = "";
    [JsonPropertyName("columns")] public List<Column> Columns { get; set; } = new();
}
