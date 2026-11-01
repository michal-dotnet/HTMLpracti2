using System.Collections.Generic;
using System.Linq;

namespace HTMLpracti2
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<string> Attributes { get; set; } = new List<string>();
        public List<string> Classes { get; set; } = new List<string>();

        public string InnerHtml { get; set; }

        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                HtmlElement current = queue.Dequeue();

                yield return current;

                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this.Parent;

            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }


    namespace HTMLpracti2
    {
        internal static class HtmlElementExtensions
        {
            public static IEnumerable<HtmlElement> FindBySelector(
                this HtmlElement element,
                Selector selector)
            {
                HashSet<HtmlElement> results = new HashSet<HtmlElement>();

                Search(element, selector, results);

                return results;
            }

            private static void Search(
                HtmlElement element,
                Selector selector,
                HashSet<HtmlElement> results)
            {
                var matches = element.Descendants()
                    .Where(e =>
                        (string.IsNullOrEmpty(selector.TagName) ||
                         e.Name == selector.TagName) &&

                        (string.IsNullOrEmpty(selector.Id) ||
                         e.Id == selector.Id) &&

                        selector.Classes.All(c => e.Classes.Contains(c))
                    )
                    .ToList();

                if (selector.Child == null)
                {
                    foreach (var match in matches)
                    {
                        results.Add(match);
                    }

                    return;
                }

                foreach (var match in matches)
                {
                    Search(match, selector.Child, results);
                }
            }
        }
    }
}