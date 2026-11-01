using System.Text.Json;

public class HtmlHelper
{
    public string[] HtmlTags { get; set; }
    public string[] VoidTags { get; set; }

    private static readonly HtmlHelper _instance = new HtmlHelper();

    public static HtmlHelper Instance => _instance;

    private HtmlHelper()
    {
        string htmlJson = File.ReadAllText("HtmlTags.json");
        string voidJson = File.ReadAllText("HtmlVoidTags.json");

        HtmlTags = JsonSerializer.Deserialize<string[]>(htmlJson)!;
        VoidTags = JsonSerializer.Deserialize<string[]>(voidJson)!;
    }
}