using Gherkin.Ast;
using SuperTestWPF.Models;
using System.Collections.ObjectModel;

namespace SuperTestWPF.Helper
{
    public static class GetSpecFlowFeatureFileModel
    {
        public static SpecFlowFeatureFileModel ConvertSpecFlowFeatureFileResponse(KeyValuePair<string, string> featureFile, GherkinDocument gherkinDocument)
        {
            string featureFileTitle = gherkinDocument.Feature.Name;

            var scenarios = new ObservableCollection<ScenarioModel>();

            if (gherkinDocument?.Feature?.Children != null)
            {
                foreach (var child in gherkinDocument.Feature.Children)
                {
                    if (child is Scenario scenario)
                    {
                        ObservableCollection<StepModel> steps = [];
                        foreach (var step in scenario.Steps)
                        {
                            steps.Add(new StepModel(step.Keyword, step.Text));
                        }
                        scenarios.Add(new ScenarioModel
                        {
                            Name = scenario.Name,
                            Keyword = scenario.Keyword,
                            IsAccepted = true,
                            Steps = steps
                        });
                    }
                }
            }

            return new SpecFlowFeatureFileModel(featureFile.Key, featureFile.Value)
            {
                FeatureFileTitle = featureFileTitle,
                GherkinDocument = gherkinDocument,
                Scenarios = scenarios
            };
        }
    }
}
