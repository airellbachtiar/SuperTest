using SuperTestLibrary.Helpers;
using SuperTestLibrary.Services.Prompts.Builders;
using SuperTestLibrary.UnitTests.TestData;

namespace SuperTestLibrary.UnitTests
{
    public sealed class PrompBuilderTests
    {
        [Test]
        public void SpecFlowFeatureFilePromptBuilder_ValidRequirementAndPath_ReturnsPrompt()
        {
            // Arrange
            SpecFlowFeatureFilePromptBuilder specFlowFeatureFileGenerator = new(Requirement.ValidRequirement);
            string validPromptPath = "TestData/ValidSpecFlowFeaturePrompt.json";

            var prompt = GetPromptFromJson.ConvertJson(validPromptPath);

            // Act
            var prompts = specFlowFeatureFileGenerator.BuildPrompt(prompt).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(prompts, Is.Not.Empty);
                Assert.That(prompts, Is.All.Not.Null);
                Assert.That(prompts[0], Contains.Substring("This is the role."));
                Assert.That(prompts[0], Contains.Substring("First instruction"));
                Assert.That(prompts[0], Contains.Substring("Second instruction"));
                Assert.That(prompts[0], Contains.Substring("This is the thinking."));
                Assert.That(prompts[0], Contains.Substring("This is an example"));
                Assert.That(prompts[0], Does.Not.Contains("\"1.\\tFirst Criteria\""));
                Assert.That(prompts[0], Does.Not.Contains("\"2.\\tSecond Criteria\""));
                Assert.That(prompts[1], Is.EqualTo("Interaction 1"));
                Assert.That(prompts[2], Is.EqualTo("Interaction 2"));
            });
        }

        [Test]
        public void EvaluateSpecFlowFeatureFilePromptBuilder_ValidRequirementAndPathAndFeatureFile_ReturnsPrompt()
        {
            // Arrange
            EvaluateSpecFlowFeatureFilePromptBuilder evaluateSpecFlowFeatureFilePromptBuilder = new(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile);
            string validPromptPath = "TestData/ValidSpecFlowFeaturePrompt.json";

            // Act
            var prompt = GetPromptFromJson.ConvertJson(validPromptPath);
            var prompts = evaluateSpecFlowFeatureFilePromptBuilder.BuildPrompt(prompt).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(prompts, Is.Not.Empty);
                Assert.That(prompts, Is.All.Not.Null);
                Assert.That(prompts[0], Contains.Substring("This is the role."));
                Assert.That(prompts[0], Contains.Substring("First instruction"));
                Assert.That(prompts[0], Contains.Substring("Second instruction"));
                Assert.That(prompts[0], Does.Not.Contains("This is an example."));
                Assert.That(prompts[0], Contains.Substring("1.\tFirst Criteria"));
                Assert.That(prompts[0], Contains.Substring("2.\tSecond Criteria"));
                Assert.That(prompts[0], Contains.Substring("•\t1 = Good"));
                Assert.That(prompts[0], Contains.Substring("•\t0 = Bad"));
                Assert.That(prompts[1], Is.EqualTo("Interaction 1"));
                Assert.That(prompts[2], Is.EqualTo("Interaction 2"));
            });
        }
    }
}
