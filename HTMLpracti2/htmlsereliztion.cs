using HTMLpracti2.HTMLpracti2;
using System.Text.RegularExpressions;

namespace HTMLpracti2
{
    internal class htmlsereliztion
    {
        public HtmlElement Serialize(string html)
        {
            var helper = HtmlHelper.Instance;
            List<string> htmlLines = SplitHtml(html);

            HtmlElement root = new HtmlElement { Name = "root" };
            HtmlElement current = root;

            foreach (var line in htmlLines)
            {
                if (line.StartsWith("</"))
                {
                    if (current.Parent != null)
                        current = current.Parent;
                }
                else if (line.StartsWith("<"))
                {
                    if (line.StartsWith("<!"))
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

                        ParseAttributes(newElement, content.Substring(tagName.Length));

                        if (!selfClosing && !helper.VoidTags.Contains(tagName))
                            current = newElement;
                    }
                }
                else
                {
                    if (current != root)
                        current.InnerHtml += line;
                }
            }

            return root;
        }

        private List<string> SplitHtml(string html)
        {
            return Regex.Matches(html, @"<[^>]+>|[^<]+")
                        .Select(m => m.Value.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
        }

        private void ParseAttributes(HtmlElement element, string text)
        {
            var attributes = Regex.Matches(
                text,
                @"([\w:-]+)\s*=\s*(?:""([^""]*)""|'([^']*)'|([^\s/>]+))"
            );

            foreach (Match attribute in attributes)
            {
                string name = attribute.Groups[1].Value;
                string value =
                    attribute.Groups[2].Success ? attribute.Groups[2].Value :
                    attribute.Groups[3].Success ? attribute.Groups[3].Value :
                    attribute.Groups[4].Value;

                element.Attributes.Add($"{name}={value}");

                if (name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    element.Id = value;

                if (name.Equals("class", StringComparison.OrdinalIgnoreCase))
                    element.Classes.AddRange(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }
}