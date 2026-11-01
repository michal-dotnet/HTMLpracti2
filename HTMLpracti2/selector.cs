using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HTMLpracti2
{

public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new();

        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public static Selector ConvertToSelector(string query)
        {
            var helper = HtmlHelper.Instance;

            // פיצול השאילתה לפי רווחים
            string[] parts = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // יצירת הסלקטור הראשון
            Selector root = new Selector();
            Selector current = root;

            foreach (var part in parts)
            {
                // פירוק החלק לפי TagName, # ו-.
                var matches = Regex.Matches(part, @"([#.])?([^#.]+)");

                foreach (Match match in matches)
                {
                    string prefix = match.Groups[1].Value;
                    string value = match.Groups[2].Value;

                    if (prefix == "#")
                    {
                        current.Id = value;
                    }
                    else if (prefix == ".")
                    {
                        current.Classes.Add(value);
                    }
                    else if (helper.HtmlTags.Contains(value))
                    {
                        current.TagName = value;
                    }
                }

                // יצירת הסלקטור הבא בשרשרת
                if (part != parts.Last())
                {
                    Selector newSelector = new Selector();

                    current.Child = newSelector;
                    newSelector.Parent = current;

                    current = newSelector;
                }
            }

            return root;
        }
    }
}
