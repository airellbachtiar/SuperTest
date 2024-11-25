using Gherkin.Ast;
using SuperTestWPF.Models;

namespace SuperTestWPF.Helper
{
    public static class GetReviewedFeatureFile
    {
        public static string GetAcceptedScenarios(SpecFlowFeatureFileModel reviewedFeatureFile)
        {
            if (reviewedFeatureFile.GherkinDocument == null)
            {
                return reviewedFeatureFile.FeatureFileContent;
            }
            var rows = reviewedFeatureFile.FeatureFileContent.Split("\n").ToList();

            var scenariosToDelete = reviewedFeatureFile.Scenarios
                                                .Where(s => !s.IsAccepted)
                                                .Reverse()
                                                .ToList();

            if (!scenariosToDelete.Any())
            {
                return reviewedFeatureFile.FeatureFileContent;
            }

            foreach (var scenarioToDelete in scenariosToDelete)
            {
                foreach (var scenarioGherkin in reviewedFeatureFile.GherkinDocument.Feature.Children.Reverse())
                {
                    if (scenarioGherkin is Scenario scenario && scenario.Name == scenarioToDelete.Name)
                    {
                        RemoveScenarioContent(scenario, rows);
                    }
                }
            }

            CleanUpDuplicateEmptyStrings(rows);

            string updatedContent = string.Join("\n", rows);

            return updatedContent;
        }

        private static void RemoveScenarioContent(Scenario scenario, List<string> rows)
        {
            RemoveLines(scenario.Examples.Select(e => e.Location.Line), rows);
            RemoveLines(scenario.Steps.Select(s => s.Location.Line), rows);
            RemoveLine(scenario.Location.Line, rows);
            RemoveLines(scenario.Tags.Select(t => t.Location.Line), rows);
        }

        private static void RemoveLines(IEnumerable<int> lineNumbers, List<string> rows)
        {
            foreach (var lineNumber in lineNumbers.Reverse())
            {
                RemoveLine(lineNumber, rows);
            }
        }

        private static void RemoveLine(int lineNumber, List<string> rows)
        {
            int index = lineNumber - 1;
            if (index >= 0 && index < rows.Count)
            {
                rows.RemoveAt(index);
            }
        }

        private static void CleanUpDuplicateEmptyStrings(List<string> rows)
        {
            for (int i = 1; i < rows.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(rows[i]) && string.IsNullOrWhiteSpace(rows[i - 1]))
                {
                    rows.RemoveAt(i - 1);
                }
            }
        }
    }
}
