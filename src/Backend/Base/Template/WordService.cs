
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using GC = Backend.GlobalConstants;

/// <summary>
/// Word generation methods
/// Created: June 2026
/// [*Licence*]
/// Author: John Stewart
/// </summary>
/// 
namespace Backend.Base.Template
{
    public class WordService : BaseService, WordServiceI
    {
        public WordService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }


        public void GenerateDocument()
        {
            string templatePath = @"C:\Source\templates\Test Template.docx";
            string outputPath = @"C:\Source\templates\Output\Result.docx";

            // Ensure output folder exists
            //Directory.CreateDirectory("Output");


            // Copy template to output
            File.Copy(templatePath, outputPath, true);



            var replacements = new Dictionary<string, string>
            {
                { "{CustomerName}", "Acme Ltd" },
                { "{ContactFirstName}", "John" },
                { "{ReportDate}", DateTime.Now.ToShortDateString() }
            };

            using (var doc = WordprocessingDocument.Open(outputPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                foreach (var paragraph in body.Descendants<Paragraph>())
                {
                    ReplaceTextInParagraph(paragraph, replacements);
                }

                doc.MainDocumentPart.Document.Save();
            }


        }


        private void ReplaceTextInParagraph(Paragraph paragraph, Dictionary<string, string> replacements)
        {
            var runs = paragraph.Elements<Run>().ToList();
            if (!runs.Any()) return;

            // Step 1: Combine all text from runs
            string fullText = string.Concat(runs.Select(r => r.GetFirstChild<Text>()?.Text));

            if (string.IsNullOrEmpty(fullText)) return;

            // Step 2: Perform replacements
            string updatedText = fullText;
            foreach (var kvp in replacements)
            {
                updatedText = updatedText.Replace(kvp.Key, kvp.Value);
            }

            // If nothing changed, skip
            if (updatedText == fullText) return;

            // Step 3: Redistribute text back into runs
            int currentIndex = 0;

            foreach (var run in runs)
            {
                var textElement = run.GetFirstChild<Text>();
                if (textElement == null) continue;

                int length = textElement.Text.Length;

                if (currentIndex >= updatedText.Length)
                {
                    textElement.Text = "";
                    continue;
                }

                int remaining = updatedText.Length - currentIndex;
                int take = Math.Min(length, remaining);

                textElement.Text = updatedText.Substring(currentIndex, take);
                currentIndex += take;
            }

            // Step 4: If leftover text, append it to the last run
            if (currentIndex < updatedText.Length)
            {
                runs.Last().AppendChild(new Text(updatedText.Substring(currentIndex)));
            }
        }



    }
}
