using Gherkin;
using Moq;
using LargeLanguageModelLibrary;
using SuperTestLibrary.Storages;
using SuperTestLibrary.UnitTests.TestData;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SuperTestLibrary.Models;
using SuperTestLibrary.Models.SpecFlowScenarioEvaluationCriteria;
using LargeLanguageModelLibrary.Enums;
using LargeLanguageModelLibrary.Models;

namespace SuperTestLibrary.UnitTests
{
    public sealed class SuperTestControllerTests
    {
        private Mock<IReqIFStorage> _mockReqIFStorage = new();
        private Mock<ILargeLanguageModel> _mockLLM = new();
        private SuperTestController _controller;

        [SetUp]
        public void Setup()
        {
            _mockReqIFStorage = new Mock<IReqIFStorage>();
            _mockLLM = new Mock<ILargeLanguageModel>();
            _controller = new SuperTestController(_mockReqIFStorage.Object, _mockLLM.Object, new Mock<ILogger<SuperTestController>>().Object);
            _controller.SelectedLLM = ModelName.GPT4o;
        }

        #region GenerateSpecFlowFeatureFileAsync Tests
        [Test]
        public async Task GenerateSpecFlowFeatureFileAsync_ValidInput_ReturnsFeatureFileResponse()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.ValidSpecFlowFeatureFileResponse);

            // Act
            SpecFlowFeatureFileResponse result = await _controller.GenerateSpecFlowFeatureFileAsync(Requirement.ValidRequirement);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.FeatureFiles, Is.Not.Empty);
                Assert.That(result.FeatureFiles, Has.Count.EqualTo(1));
                Assert.That(result.GherkinDocuments, Is.Not.Empty);
                Assert.That(result.GherkinDocuments, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void GenerateSpecFlowFeatureFileAsync_InvalidInput_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidRequirements = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.GenerateSpecFlowFeatureFileAsync(invalidRequirements));
        }

        [Test]
        public void GenerateSpecFlowFeatureFileAsync_IncompleteLlmResponse_ThrowsJsonException()
        {
            //Arrange
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.IncompleteFeatureFile);

            // Act & Assert
            Assert.ThrowsAsync<JsonException>(async () =>
                await _controller.GenerateSpecFlowFeatureFileAsync(Requirement.ValidRequirement));
        }

        [Test]
        public void GenerateSpecFlowFeatureFileAsync_InvalidSpecFlowFileResponse_ThrowsCompositeParserException()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.InvalidSpecFlowFeatureFileResponse);

            // Act & Assert
            Assert.ThrowsAsync<CompositeParserException>(async () =>
                await _controller.GenerateSpecFlowFeatureFileAsync(Requirement.ValidRequirement));
        }
        #endregion

        #region EvaluateSpecFlowFeatureFileAsync Tests
        [Test]
        public async Task EvaluateSpecFlowFeatureFileAsync_ValidInput_ReturnsSpecFlowFeatureFileResponse()
        {
            // Arrange
            var expectedResult = new EvaluateSpecFlowFeatureFileResponse
            {
                Structure = -1,
                Readability = 4,
                Consistency = 4,
                Focus = 4,
                Maintainability = -1,
                Coverage = 4,
                Summary = "The feature file is generally well-written and meets most of the evaluation criteria. However, there are a few recommendations for improvement. The feature name is descriptive and provides a clear understanding of its purpose, resulting in high readability and focus. It maintains consistent naming and format, which adds to its clarity. There is no use of Background steps or step reusability in this feature file, which might reduce maintainability and the potential for streamlined steps across scenarios. Coverage is good, as it considers various scenarios including error conditions. Consider using Background steps if there are common preconditions across scenarios and explore step reuse opportunities to enhance maintainability.",
                Score = new EvaluationScore
                {
                    MaximumScore = 20,
                    TotalScore = 16,
                    Percentage = 80d
                }
            };
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.ValidEvaluateSpecFlowFeatureFileResponse);

            // Act
            EvaluateSpecFlowFeatureFileResponse result = await _controller.EvaluateSpecFlowFeatureFileAsync(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Readability, Is.EqualTo(expectedResult.Readability));
                Assert.That(result.Consistency, Is.EqualTo(expectedResult.Consistency));
                Assert.That(result.Focus, Is.EqualTo(expectedResult.Focus));
                Assert.That(result.Structure, Is.EqualTo(expectedResult.Structure));
                Assert.That(result.Maintainability, Is.EqualTo(expectedResult.Maintainability));
                Assert.That(result.Coverage, Is.EqualTo(expectedResult.Coverage));
                Assert.That(result.Summary, Is.EqualTo(expectedResult.Summary));
                Assert.That(result.Score.MaximumScore, Is.EqualTo(expectedResult.Score.MaximumScore));
                Assert.That(result.Score.TotalScore, Is.EqualTo(expectedResult.Score.TotalScore));
                Assert.That(result.Score.Percentage, Is.EqualTo(expectedResult.Score.Percentage));
            });
        }

        [Test]
        public void EvaluateSpecFlowFeatureFileAsync_InvalidInput_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidRequirements = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowFeatureFileAsync(invalidRequirements, FeatureFile.ValidSpecFlowFeatureFile));
        }

        [Test]
        public void EvaluateSpecFlowFeatureFileAsync_InvalidFeatureFile_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidFeatureFile = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowFeatureFileAsync(Requirement.ValidRequirement, invalidFeatureFile));
        }

        [Test]
        public void EvaluateSpecFlowFeatureFileAsync_IncompleteLlmResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.IncompleteEvaluateSpecFlowFeatureFileResponse);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowFeatureFileAsync(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile));
        }

        [Test]
        public void EvaluateSpecFlowFeatureFileAsync_InvalidEvaluateSpecFlowFeatureFileResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.MismatchEvaluateSpecFlowFeatureFileResponse);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowFeatureFileAsync(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile));
        }
        #endregion

        #region EvaluateSpecFlowScenarioAsync Tests
        [Test]
        public async Task EvaluateSpecFlowScenarioAsync_ValidInput_ReturnsSpecFlowScenarioResponse()
        {
            // Arrange
            var expectedResult = new EvaluateSpecFlowScenarioResponse
            {
                ScenarioEvaluations = [
                    new SpecFlowScenarioEvaluation {
                        ScenarioName = "Generate evaluation score for a SpecFlow feature file",
                        ClarityAndReadability = new ClarityAndReadabilityScenarioCriteria {
                            HumanFriendlyLanguage = 4,
                            ConciseAndRelevantScenarios = 4,
                            LogicalFlow = 4
                        },
                        StructureAndFocus = new StructureAndFocusScenarioCriteria {
                            FocusedScenario = 5,
                            ScenarioStructure = 5,
                            ScenarioOutlines = -1
                        },
                        Maintainability = new MaintainabilityScenarioCriteria {
                            MinimalCouplingToImplementation = 5,
                            IndependentScenarios = 5,
                            TestDataManagement = 4
                        },
                        Traceability = new TraceabilityScenarioCriteria {
                            TraceabilityToRequirements = 5
                        },
                        Summary = "The scenario is well-constructed, with clear and understandable language appropriate for both technical and non-technical stakeholders. It logically follows the Given-When-Then structure, focusing on a single business behavior. The scenario lacks the need for a Scenario Outline, so its exclusion is appropriate. Test data is not cluttered, maintaining readability and relevance, and there's excellent traceability to the requirement REQ-FR-3.1. Overall, it’s a strong scenario with minor improvements possible in the clarity of the steps and a slight increase in conciseness.",
                        Score = new EvaluationScore{
                            MaximumScore = 45,
                            TotalScore = 41,
                            Percentage = 91.11
                        }
                    }
                    ]
            };
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.ValidEvaluateSpecFlowScenarioResponse);

            // Act
            EvaluateSpecFlowScenarioResponse result = await _controller.EvaluateSpecFlowScenarioAsync(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ScenarioEvaluations, Is.Not.Empty);
                Assert.That(result.ScenarioEvaluations, Has.Count.EqualTo(1));
                Assert.That(result.ScenarioEvaluations[0].ScenarioName, Is.EqualTo(expectedResult.ScenarioEvaluations[0].ScenarioName));
                Assert.That(result.ScenarioEvaluations[0].ClarityAndReadability.HumanFriendlyLanguage, Is.EqualTo(expectedResult.ScenarioEvaluations[0].ClarityAndReadability.HumanFriendlyLanguage));
                Assert.That(result.ScenarioEvaluations[0].ClarityAndReadability.ConciseAndRelevantScenarios, Is.EqualTo(expectedResult.ScenarioEvaluations[0].ClarityAndReadability.ConciseAndRelevantScenarios));
                Assert.That(result.ScenarioEvaluations[0].ClarityAndReadability.LogicalFlow, Is.EqualTo(expectedResult.ScenarioEvaluations[0].ClarityAndReadability.LogicalFlow));
                Assert.That(result.ScenarioEvaluations[0].StructureAndFocus.FocusedScenario, Is.EqualTo(expectedResult.ScenarioEvaluations[0].StructureAndFocus.FocusedScenario));
                Assert.That(result.ScenarioEvaluations[0].StructureAndFocus.ScenarioStructure, Is.EqualTo(expectedResult.ScenarioEvaluations[0].StructureAndFocus.ScenarioStructure));
                Assert.That(result.ScenarioEvaluations[0].StructureAndFocus.ScenarioOutlines, Is.EqualTo(expectedResult.ScenarioEvaluations[0].StructureAndFocus.ScenarioOutlines));
                Assert.That(result.ScenarioEvaluations[0].Maintainability.MinimalCouplingToImplementation, Is.EqualTo(expectedResult.ScenarioEvaluations[0].Maintainability.MinimalCouplingToImplementation));
                Assert.That(result.ScenarioEvaluations[0].Maintainability.IndependentScenarios, Is.EqualTo(expectedResult.ScenarioEvaluations[0].Maintainability.IndependentScenarios));
                Assert.That(result.ScenarioEvaluations[0].Maintainability.TestDataManagement, Is.EqualTo(expectedResult.ScenarioEvaluations[0].Maintainability.TestDataManagement));
                Assert.That(result.ScenarioEvaluations[0].Traceability.TraceabilityToRequirements, Is.EqualTo(expectedResult.ScenarioEvaluations[0].Traceability.TraceabilityToRequirements));
                Assert.That(result.ScenarioEvaluations[0].Summary, Is.EqualTo(expectedResult.ScenarioEvaluations[0].Summary));
                Assert.That(result.ScenarioEvaluations[0].Score.MaximumScore, Is.EqualTo(expectedResult.ScenarioEvaluations[0].Score.MaximumScore));
                Assert.That(result.ScenarioEvaluations[0].Score.TotalScore, Is.EqualTo(expectedResult.ScenarioEvaluations[0].Score.TotalScore));
                Assert.That(result.ScenarioEvaluations[0].Score.Percentage, Is.EqualTo(expectedResult.ScenarioEvaluations[0].Score.Percentage));
            });
        }

        [Test]
        public void EvaluateSpecFlowScenarioAsync_InvalidInput_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidRequirements = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowScenarioAsync(invalidRequirements, FeatureFile.ValidSpecFlowFeatureFile));
        }

        [Test]
        public void EvaluateSpecFlowScenarioAsync_InvalidFeatureFile_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidFeatureFile = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowScenarioAsync(Requirement.ValidRequirement, invalidFeatureFile));
        }

        [Test]
        public void EvaluateSpecFlowScenarioAsync_IncompleteLlmResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.IncompleteEvaluateSpecFlowScenarioResponse);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowScenarioAsync(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile));
        }

        [Test]
        public void EvaluateSpecFlowScenarioAsync_InvalidEvaluateSpecFlowScenarioResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.MismatchEvaluateSpecFlowScenarioResponse);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.EvaluateSpecFlowScenarioAsync(Requirement.ValidRequirement, FeatureFile.ValidSpecFlowFeatureFile));
        }
        #endregion

        #region GenerateSpecFlowBindingFileAsync Tests
        [Test]
        public async Task GenerateSpecFlowBindingFileAsync_ValidInput_ReturnsFeatureFileResponse()
        {
            // Arrange
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.ValidSpecFlowBindingFileResponse);

            // Act
            SpecFlowBindingFileResponse result = await _controller.GenerateSpecFlowBindingFileAsync(FeatureFile.ValidSpecFlowFeatureFile, []);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.BindingFiles, Is.Not.Empty);
                Assert.That(result.BindingFiles, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void GenerateSpecFlowBindingFileAsync_InvalidInput_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidRequirements = string.Empty;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _controller.GenerateSpecFlowBindingFileAsync(invalidRequirements, []));
        }

        [Test]
        public void GenerateSpecFlowBindingFileAsync_IncompleteLlmResponse_ThrowsJsonException()
        {
            //Arrange
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.IncompleteSpecFlowBindingFileResponse);

            // Act & Assert
            Assert.ThrowsAsync<JsonException>(async () =>
                await _controller.GenerateSpecFlowBindingFileAsync(Requirement.ValidRequirement, []));
        }
        #endregion

        #region Integration Tests
        [Test]
        public async Task GenerateSpecFlowFeatureFileAsync_ValidInput_IntegrationTest()
        {
            // Arrange
            var realReqIFStorage = new GitReqIFStorage("C:\\Dev\\GitLocalFolderTest");
            var controller = new SuperTestController(realReqIFStorage, _mockLLM.Object, new Mock<ILogger<SuperTestController>>().Object)
            {
                SelectedLLM = ModelName.GPT4o
            };
            _mockLLM.Setup(llm => llm.ChatAsync(It.IsAny<ModelName>(), It.IsAny<MessageRequest>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(LLMResponse.ValidSpecFlowFeatureFileResponse);

            // Act
            var reqIfFiles = await controller.GetAllReqIFFilesAsync();
            SpecFlowFeatureFileResponse result = await controller.GenerateSpecFlowFeatureFileAsync(reqIfFiles.First());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.FeatureFiles, Is.Not.Empty);
                Assert.That(result.GherkinDocuments, Is.Not.Empty);
            });
        }
        #endregion
    }
}
