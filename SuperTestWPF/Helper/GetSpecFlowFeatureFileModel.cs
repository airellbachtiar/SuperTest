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
            string featureFileDescription = gherkinDocument.Feature.Description;

            var scenarios = new ObservableCollection<ScenarioModel>();

            if (gherkinDocument?.Feature?.Children != null)
            {
                foreach (var child in gherkinDocument.Feature.Children)
                {
                    if (child is Scenario scenario)
                    {
                        scenarios.Add(new ScenarioModel
                        {
                            Name = scenario.Name,
                            Keyword = scenario.Keyword,
                            IsAccepted = true,
                            Steps = new ObservableCollection<StepModel>(scenario.Steps.Select(s => new StepModel(s.Keyword, s.Text))),
                            Tags = new ObservableCollection<TagModel>(scenario.Tags.Select(t => new TagModel { Name = t.Name })),
                        });
                    }
                }
            }

            return new SpecFlowFeatureFileModel(featureFile.Key, featureFile.Value)
            {
                FeatureFileTitle = featureFileTitle,
                GherkinDocument = gherkinDocument,
                FeatureFileDescription = featureFileDescription,
                Scenarios = scenarios
            };
        }
    }
}
