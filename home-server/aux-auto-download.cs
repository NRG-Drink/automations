using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

/*
Fetches all podcasts and set the auto-download flag to true.
*/

const string apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXlJZCI6ImNmN2Y5NzIxLWM5ZDAtNDM4Yi05OTI5LTkyOTE5MDJkMTU1MCIsIm5hbWUiOiJsb2NhbC1zY3JpcHQtYWRtaW4iLCJ0eXBlIjoiYXBpIiwiaWF0IjoxNzcyNTUzOTQ0fQ.csRyenIExeqrtPuMnFyCRXUyU7VtsjQjwmgH5XHnnoY";
const string libraryId = "233751d0-058a-41a8-8e3e-325c8ca86b95";

var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

var response = await client.GetAsync($"https://aux.tail976322.ts.net/api/libraries/{libraryId}/items");
if (!response.IsSuccessStatusCode)
{
    System.Console.WriteLine($"Failed to get items: {response.StatusCode}");
    return;
}

var items = await response.Content.ReadFromJsonAsync(AuxContext.Default.Root);

// Print download schedule properties and title for all items.
// var sortedItems = items?.Results
//     .OrderBy(e => e.Media.AutoDownloadEpisodes)
//     .ToArray();
// foreach (var item in sortedItems ?? [])
// {
//     System.Console.WriteLine($"{item.Media.AutoDownloadEpisodes}\t{item.Media.AutoDownloadSchedule}\t'{item.Media.Metadata.Title}'");
// }
// return;

var autodownloadItems = items?.Results
    // Resson: Stopped production.
    .Where(e => !e.Media.Metadata.Title.StartsWith("TED-Ed", StringComparison.OrdinalIgnoreCase))
    // Resson: Stopped production.
    .Where(e => !e.Media.Metadata.Title.StartsWith("Wissenschaftsmagazin", StringComparison.OrdinalIgnoreCase));
// var nonAutodownloadItems = items?.Results.Except(autodownloadItems ?? []).ToArray();

foreach (var item in autodownloadItems ?? [])
{
    var r = await client.PatchAsync(
        $"https://aux.tail976322.ts.net/api/items/{item.Id}/media",
        new StringContent("""
        {
            "autoDownloadEpisodes": true
        }
        """,
            System.Text.Encoding.UTF8,
            "application/json"
        )
    );

    System.Console.WriteLine($"{r.StatusCode} for '{item.Media.Metadata.Title}'");
}

#region Models
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class Media
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }

    [JsonPropertyName("coverPath")]
    public string CoverPath { get; set; }

    [JsonPropertyName("tags")]
    public List<object> Tags { get; set; }

    [JsonPropertyName("numEpisodes")]
    public int? NumEpisodes { get; set; }

    [JsonPropertyName("autoDownloadEpisodes")]
    public bool? AutoDownloadEpisodes { get; set; }

    [JsonPropertyName("autoDownloadSchedule")]
    public string AutoDownloadSchedule { get; set; }

    [JsonPropertyName("lastEpisodeCheck")]
    public object LastEpisodeCheck { get; set; }

    [JsonPropertyName("maxEpisodesToKeep")]
    public int? MaxEpisodesToKeep { get; set; }

    [JsonPropertyName("maxNewEpisodesToDownload")]
    public int? MaxNewEpisodesToDownload { get; set; }

    [JsonPropertyName("size")]
    public object Size { get; set; }
}

public class Metadata
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("releaseDate")]
    public DateTime? ReleaseDate { get; set; }

    [JsonPropertyName("genres")]
    public List<string> Genres { get; set; }

    [JsonPropertyName("feedUrl")]
    public string FeedUrl { get; set; }

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; }

    [JsonPropertyName("itunesPageUrl")]
    public string ItunesPageUrl { get; set; }

    [JsonPropertyName("itunesId")]
    public string ItunesId { get; set; }

    [JsonPropertyName("itunesArtistId")]
    public string ItunesArtistId { get; set; }

    [JsonPropertyName("explicit")]
    public bool? Explicit { get; set; }

    [JsonPropertyName("language")]
    public string Language { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("titleIgnorePrefix")]
    public string TitleIgnorePrefix { get; set; }
}

public class Result
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("ino")]
    public string Ino { get; set; }

    [JsonPropertyName("oldLibraryItemId")]
    public object OldLibraryItemId { get; set; }

    [JsonPropertyName("libraryId")]
    public string LibraryId { get; set; }

    [JsonPropertyName("folderId")]
    public string FolderId { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("relPath")]
    public string RelPath { get; set; }

    [JsonPropertyName("isFile")]
    public bool? IsFile { get; set; }

    [JsonPropertyName("mtimeMs")]
    public object MtimeMs { get; set; }

    [JsonPropertyName("ctimeMs")]
    public object CtimeMs { get; set; }

    [JsonPropertyName("birthtimeMs")]
    public object BirthtimeMs { get; set; }

    [JsonPropertyName("addedAt")]
    public object AddedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public object UpdatedAt { get; set; }

    [JsonPropertyName("isMissing")]
    public bool? IsMissing { get; set; }

    [JsonPropertyName("isInvalid")]
    public bool? IsInvalid { get; set; }

    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; }

    [JsonPropertyName("media")]
    public Media Media { get; set; }

    [JsonPropertyName("numFiles")]
    public int? NumFiles { get; set; }

    [JsonPropertyName("size")]
    public object Size { get; set; }
}

public class Root
{
    [JsonPropertyName("results")]
    public List<Result> Results { get; set; }

    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    [JsonPropertyName("page")]
    public int? Page { get; set; }

    [JsonPropertyName("sortDesc")]
    public bool? SortDesc { get; set; }

    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; }

    [JsonPropertyName("minified")]
    public bool? Minified { get; set; }

    [JsonPropertyName("collapseseries")]
    public bool? Collapseseries { get; set; }

    [JsonPropertyName("include")]
    public string Include { get; set; }

    [JsonPropertyName("offset")]
    public int? Offset { get; set; }
}

[JsonSerializable(typeof(Root))]
[JsonSerializable(typeof(Result))]
[JsonSerializable(typeof(Media))]
[JsonSerializable(typeof(Metadata))]
[JsonSerializable(typeof(List<Result>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<object>))]
internal partial class AuxContext : JsonSerializerContext { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#endregion