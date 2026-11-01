using HTMLpracti2;
using HTMLpracti2.HTMLpracti2;

var html = await File.ReadAllTextAsync(@"C:\Users\user1\Downloads\preview.html");

var serializer = new htmlsereliztion();
HtmlElement root = serializer.Serialize(html);

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