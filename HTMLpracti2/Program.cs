using HTMLpracti2;
using HTMLpracti2.HTMLpracti2;
using System.Text.RegularExpressions;
using System.Xml.Linq;

var html = await Load("file:///C:/Users/user1/Downloads/preview.html");

var htmlLines = Regex.Matches(html, @"<[^>]+>|[^<]+").Select(m => m.Value.Trim()) .Where(s => !string.IsNullOrWhiteSpace(s))
    .ToList(); async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
var helper = HtmlHelper.Instance;

HtmlElement root = new HtmlElement { Name = "root" };
HtmlElement current = root;
foreach (var line in htmlLines)
{
    if (line.StartsWith("</"))
    {
        if (current.Parent != null)
        {
            current = current.Parent;
        }
    }

    else if (line.StartsWith("<"))
    {
        if (line.StartsWith("<!--") || line.StartsWith("<!"))
            continue;

        string content = line.Trim('<', '>');

        bool selfClosing = content.TrimEnd().EndsWith("/");

        content = content.TrimEnd().TrimEnd('/').Trim();

        Match tagMatch = Regex.Match(content, @"^([^\s]+)");

        if (!tagMatch.Success)
            continue;

        string tagName = tagMatch.Value;

        if (helper.HtmlTags.Contains(tagName))
        {
            HtmlElement newElement = new HtmlElement
            {
                Name = tagName,
                Parent = current
            };

            current.Children.Add(newElement);

            string attributesText = content.Substring(tagName.Length);

            var attributes = Regex.Matches(
                attributesText,
                @"([\w:-]+)\s*=\s*(?:""([^""]*)""|'([^']*)'|([^\s/>]+))"
            );

            foreach (Match attribute in attributes)
            {
                string name = attribute.Groups[1].Value;

                string value =
                    attribute.Groups[2].Success ? attribute.Groups[2].Value :
                    attribute.Groups[3].Success ? attribute.Groups[3].Value :
                    attribute.Groups[4].Value;

                newElement.Attributes.Add($"{name}={value}");

                if (name.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    newElement.Id = value;
                }

                if (name.Equals("class", StringComparison.OrdinalIgnoreCase))
                {
                    newElement.Classes.AddRange(
                        value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    );
                }
            }
            if (!selfClosing && !helper.VoidTags.Contains(tagName))
            {
                current = newElement;
            }
        }
    }

    else
    {
        if (current != root)
        {
            current.InnerHtml += line;
        }
    }
}

Console.WriteLine("העץ נבנה בהצלחה!");

while (true)
{
    Console.WriteLine("הכניסי Selector (או exit ליציאה):");

    string query = Console.ReadLine();

    if (query == "exit")
        break;

    Selector selector = Selector.ConvertToSelector(query);

    var results = root.FindBySelector(selector).ToList();

    Console.WriteLine($"נמצאו {results.Count} אלמנטים:");

    foreach (var element in results)
    {
        Console.WriteLine("--------------------");
        Console.WriteLine($"Name: {element.Name}");
        Console.WriteLine($"Id: {element.Id}");
        Console.WriteLine($"Classes: {string.Join(", ", element.Classes)}");
        Console.WriteLine($"InnerHtml: {element.InnerHtml}");
    }
}
