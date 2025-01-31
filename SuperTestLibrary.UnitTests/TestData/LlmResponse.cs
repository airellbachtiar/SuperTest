using LargeLanguageModelLibrary.Models;

namespace SuperTestLibrary.UnitTests.TestData
{
    public static class LLMResponse
    {
        public static MessageResponse ValidSpecFlowFeatureFileResponse => new()
        {
            Messages =
            [
                new()
                {
                    Text = "{\r\n  \"FeatureFiles\": {\r\n    \"GenerateSpecFlowFeatureFile.feature\": \"Feature: Generate SpecFlow Feature File Using LLM\\n\\n  As a user\\n  I want to generate SpecFlow feature files using an LLM\\n  So that I can automate the creation of test scenarios based on requirements\\n\\n  Scenario: Generate SpecFlow feature file from requirements\\n    Given I have a set of requirements\\n    When I use the chosen LLM to generate a SpecFlow feature file\\n    Then a valid SpecFlow feature file should be created\\n    And the feature file should accurately reflect the given requirements\\n\\n  Scenario: Choose an LLM for feature file generation\\n    Given I have access to multiple LLMs\\n    When I select a specific LLM for feature file generation\\n    Then the selected LLM should be used to generate the SpecFlow feature file\\n\\n  Scenario: Choose SpecFlow feature file generator\\n    Given I have multiple SpecFlow feature file generators available\\n    When I select a specific SpecFlow feature file generator\\n    Then the selected generator should be used to create the feature file\\n\\n  Scenario Outline: Generate feature file with different LLMs\\n    Given I have requirements for a feature\\n    And I have selected <LLM> as the language model\\n    When I generate a SpecFlow feature file\\n    Then the feature file should be created using <LLM>\\n\\n    Examples:\\n      | LLM       |\\n      | GPT-3     |\\n      | GPT-4     |\\n      | BERT      |\\n      | T5        |\\n\\n  Scenario: Validate generated feature file\\n    Given a SpecFlow feature file has been generated\\n    When I review the generated feature file\\n    Then it should contain valid Gherkin syntax\\n    And it should include scenarios covering the provided requirements\\n\\n  # The following scenarios are recommendations and not directly derived from the given requirements\\n  # Scenario: Handle invalid requirements input\\n  #   Given I have an invalid set of requirements\\n  #   When I attempt to generate a SpecFlow feature file\\n  #   Then the system should provide an error message\\n  #   And no feature file should be generated\\n\\n  # Scenario: Generate feature file with empty requirements\\n  #   Given I have no requirements provided\\n  #   When I attempt to generate a SpecFlow feature file\\n  #   Then the system should prompt for requirements input\\n  #   And no feature file should be generated until requirements are provided\\n\\n  # Scenario: Compare output from different LLMs\\n  #   Given I have a set of requirements\\n  #   When I generate SpecFlow feature files using different LLMs\\n  #   Then I should be able to compare the outputs\\n  #   And select the most suitable generated feature file\"\r\n  }\r\n}"
                }
            ]
        };

        public static MessageResponse IncompleteFeatureFile => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\r\n  \"FeatureFiles\": {\r\n    \"GenerateSpecFlowFeatureFile.feature\": \"Feature: Generate SpecFlow Feature File Using LLM\\n\\n"
                }
            ]
        };

        public static MessageResponse InvalidSpecFlowFeatureFileResponse => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\r\n  \"FeatureFiles\": {\r\n    \"GenerateSpecFlowFeatureFile.feature\": \"Using LLM\\n\\n  As a user\\n  I want to generate Specg an LLM\\n  So that I can automate the creation of test scenarios based on requirements\\n\\n  enerate SpecFlow feature file from requirements\\n    Givet of requirements\\n    When I use the chosen LLM to gener generated feature file\"\r\n  }\r\n}"
                }
            ]
        };

        public static string ValidId => "Claude 3.5 Sonnet";

        public static MessageResponse ValidEvaluateSpecFlowFeatureFileResponse => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\"Readability\": 4, \"Consistency\": 4, \"Focus\": 4, \"Structure\": -1, \"Maintainability\": -1, \"Coverage\": 4, \"Score\": {\"MaximumScore\": 20, \"TotalScore\": 16}, \"Summary\": \"The feature file is generally well-written and meets most of the evaluation criteria. However, there are a few recommendations for improvement. The feature name is descriptive and provides a clear understanding of its purpose, resulting in high readability and focus. It maintains consistent naming and format, which adds to its clarity. There is no use of Background steps or step reusability in this feature file, which might reduce maintainability and the potential for streamlined steps across scenarios. Coverage is good, as it considers various scenarios including error conditions. Consider using Background steps if there are common preconditions across scenarios and explore step reuse opportunities to enhance maintainability.\"}"
                }
            ]
        };

        public static MessageResponse IncompleteEvaluateSpecFlowFeatureFileResponse => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\"Readability\": 4, \"Consistency\": 4, \"Focus\": 4, \"Structure\": 0, \"Maintainability\": 0, \"Coverage\": 4, \"Score\": {\"Maximum"
                }
            ]
        };
        public static MessageResponse MismatchEvaluateSpecFlowFeatureFileResponse => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\"Read\": 4, \"Consist\": 4, \"Focuses\": 4, \"Struct\": 0, \"Maintainan\": 0, \"Cover\": 4, \"Scores\": {\"MaximumScores\": 20, \"TotalScores\": 16}, \"Summarize\": \"The feature fi\"}"
                }
            ]
        };

        public static MessageResponse ValidEvaluateSpecFlowScenarioResponse => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\"ScenarioEvaluations\":[{\"ScenarioName\":\"Generate evaluation score for a SpecFlow feature file\",\"ClarityAndReadability\":{\"HumanFriendlyLanguage\":4,\"ConciseAndRelevantScenarios\":4,\"LogicalFlow\":4},\"StructureAndFocus\":{\"FocusedScenario\":5,\"ScenarioStructure\":5,\"ScenarioOutlines\":-1},\"Maintainability\":{\"MinimalCouplingToImplementation\":5,\"IndependentScenarios\":5,\"TestDataManagement\":4},\"Traceability\":{\"TraceabilityToRequirements\":5},\"Summary\":\"The scenario is well-constructed, with clear and understandable language appropriate for both technical and non-technical stakeholders. It logically follows the Given-When-Then structure, focusing on a single business behavior. The scenario lacks the need for a Scenario Outline, so its exclusion is appropriate. Test data is not cluttered, maintaining readability and relevance, and there's excellent traceability to the requirement REQ-FR-3.1. Overall, it’s a strong scenario with minor improvements possible in the clarity of the steps and a slight increase in conciseness.\"}]}"
                }
            ]
        };

        public static MessageResponse IncompleteEvaluateSpecFlowScenarioResponse => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\"ScenarioEvaluations\":[{\"ScenarioName\":\"Generate evaluation score for a SpecFlow feature file\",\"ClarityAndReadability\":{\"HumanFriendlyLanguage\":4,\"ConciseAndRelev"
                }
            ]
        };

        public static MessageResponse MismatchEvaluateSpecFlowScenarioResponse => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\"ScenarioEvaluations\":[{\"Scenario\":\"Generate evaluation score for a SpecFlow feature file\",\"Clarity\":{\"FriendlyLanguage\":4,\"RelevantScenarios\":4,\"Flow\":4},\"Structure\":{\"Focus\":5,\"ScenarioStruct\":5,\"Scenario\":-1},\"Maintain\":{\"MinimalCoupling\":5,\"IndependentScenario\":5,\"TestData\":4},\"Trace\":{\"Traceabilities\":5},\"Summarize\":\"The scenario\"}]}"
                }
            ]
        };

        // Binding file response
        public static MessageResponse ValidSpecFlowBindingFileResponse => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\r\n  \"BindingFiles\": {\r\n    \"PedestrianLightSystemSteps.cs\": \"using FluentAssertions;\\nusing NUnit.Framework;\\nusing TechTalk.SpecFlow;\\nusing TrafficTest;\\n\\nnamespace TrafficTest.Steps\\n{\\n    [Binding]\\n    [Scope(Feature = \\\"Pedestrian Light System\\\")]\\n    public class PedestrianLightSystemSteps\\n    {\\n        [Given(@\\\"the system is in idle state\\\")]\\n        public void GivenTheSystemIsInIdleState()\\n        {\\n            // Assuming the system starts in idle state\\n            // No action needed\\n        }\\n\\n        [When(@\\\"the start button is pressed\\\")]\\n        public void WhenTheStartButtonIsPressed()\\n        {\\n            TrafficTestHooks.ClickButton(\\\"StartButton\\\");\\n        }\\n\\n        [Then(@\\\"the red pedestrian light should be turned on\\\")]\\n        public void ThenTheRedPedestrianLightShouldBeTurnedOn()\\n        {\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianRed\\\", \\\"On\\\");\\n        }\\n\\n        [Given(@\\\"the system is operational\\\")]\\n        public void GivenTheSystemIsOperational()\\n        {\\n            // Assuming the system is operational after pressing the start button\\n            TrafficTestHooks.ClickButton(\\\"StartButton\\\");\\n        }\\n\\n        [Given(@\\\"the red pedestrian light is on\\\")]\\n        public void GivenTheRedPedestrianLightIsOn()\\n        {\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianRed\\\", \\\"On\\\");\\n        }\\n\\n        [When(@\\\"the pedestrian button is pressed\\\")]\\n        public void WhenThePedestrianButtonIsPressed()\\n        {\\n            TrafficTestHooks.Client.PressRequestPedestrianWalkButton(new TestBus.Empty());\\n        }\\n\\n        [Then(@\\\"the green pedestrian light should be turned on within (\\\\d+) seconds\\\")]\\n        public void ThenTheGreenPedestrianLightShouldBeTurnedOnWithinSeconds(int seconds)\\n        {\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianGreen\\\", \\\"On\\\", seconds);\\n        }\\n\\n        [Given(@\\\"the green pedestrian light is on\\\")]\\n        public void GivenTheGreenPedestrianLightIsOn()\\n        {\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianGreen\\\", \\\"On\\\");\\n        }\\n\\n        [When(@\\\"the green pedestrian light on time has elapsed\\\")]\\n        public void WhenTheGreenPedestrianLightOnTimeHasElapsed()\\n        {\\n            // Assuming the green light on time is handled by the system\\n            // We'll wait for the green light to start blinking\\n        }\\n\\n        [Then(@\\\"the green pedestrian light should start blinking\\\")]\\n        public void ThenTheGreenPedestrianLightShouldStartBlinking()\\n        {\\n            // Assuming blinking is represented by rapid on/off changes\\n            // We'll check for alternating states within a short timeframe\\n            var initialState = TrafficTestHooks.TrafficLightStates.Last().PedestrianGreen.LightState;\\n            TrafficTestHooks.WaitUntilTheLightIsInThatState(\\\"PedestrianGreen\\\", initialState == \\\"On\\\" ? \\\"Off\\\" : \\\"On\\\", 5);\\n        }\\n\\n        [When(@\\\"the green pedestrian light on time and blinking time have elapsed\\\")]\\n        public void WhenTheGreenPedestrianLightOnTimeAndBlinkingTimeHaveElapsed()\\n        {\\n            // Assuming the system handles the timing\\n            // We'll wait for the red light to turn on\\n        }\\n\\n        [When(@\\\"any pedestrian light is on\\\")]\\n        public void WhenAnyPedestrianLightIsOn()\\n        {\\n            var lastState = TrafficTestHooks.TrafficLightStates.Last();\\n            (lastState.PedestrianRed.LightState == \\\"On\\\" || lastState.PedestrianGreen.LightState == \\\"On\\\")\\n                .Should().BeTrue(\\\"At least one pedestrian light should be on\\\");\\n        }\\n\\n        [Then(@\\\"only one pedestrian light should be on\\\")]\\n        public void ThenOnlyOnePedestrianLightShouldBeOn()\\n        {\\n            var lastState = TrafficTestHooks.TrafficLightStates.Last();\\n            (lastState.PedestrianRed.LightState == \\\"On\\\" ^ lastState.PedestrianGreen.LightState == \\\"On\\\")\\n                .Should().BeTrue(\\\"Exactly one pedestrian light should be on\\\");\\n        }\\n    }\\n}\\n\"\r\n  }\r\n}"
                }
            ]
        };

        public static MessageResponse IncompleteSpecFlowBindingFileResponse => new ()
        {
            Messages =
            [
                new ()
                {
                    Text = "{\r\n  \"BindingFiles\": {\r\n    \"PedestrianLightSystemSteps.cs\": \"using FluentAssertions;"
                }
            ]
        };

    }
}
