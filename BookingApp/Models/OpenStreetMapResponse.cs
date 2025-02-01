using System.Text.Json.Serialization;

public class OpenStreetMapResponse
{
    [JsonPropertyName("lat")]
    public string Latitudine { get; set; }

    [JsonPropertyName("lon")]
    public string Longitudine { get; set; }
}
