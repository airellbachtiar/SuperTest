using Gherkin;
using SuperTestWPF.Helper;
using SuperTestWPF.Models;

namespace SuperTestWPF.UnitTests
{
    public class GetReviewedFeatureFileTests
    {
        SpecFlowFeatureFileModel? specFlowFeatureFileModel = null;
        [SetUp]
        public void Setup()
        {
            string featureFile = @"Feature: Traffic Light System Management

  The system controls traffic lights at intersections according to predefined timing sequences and transitions.

  @SFR-001
  Scenario Outline: Traffic light transitions based on timing sequence
    Given the traffic light is <CurrentState>
    And the time since entering the state is <ElapsedTime> seconds
    When the timing sequence is evaluated
    Then the traffic light should transition to <NextState>

    Examples:
      | CurrentState | ElapsedTime | NextState  |
      | red          | 15          | green      |
      | green        | 20          | yellow     |
      | yellow       | 5           | red        |

  @SFR-001a
  Scenario: Red traffic light turns on after yellow light duration
    Given the traffic light is yellow
    When the yellow light duration has elapsed
    Then the traffic light should transition to red

  # Recommendation: Edge Case
  # Scenario: Red traffic light transitions earlier than configured due to miscalculation
  #   Given the traffic light is yellow
  #   When the yellow light duration is improperly configured
  #   Then an alert should be raised and the system should correct the duration

  @SFR-001b
  Scenario: Yellow traffic light turns on when pedestrian button is pressed
    Given the traffic light is green
    When the pedestrian button is pressed
    Then the traffic light should transition to yellow";
        var parser = new Parser();
            var gherkinDocument = parser.Parse(new StringReader(featureFile));

            specFlowFeatureFileModel = GetSpecFlowFeatureFileModel.ConvertSpecFlowFeatureFileResponse(new KeyValuePair<string, string>("TrafficLightSystem.feature", featureFile), gherkinDocument);
        }

        [Test]
        public void GetAcceptedScenarios_RemoveLastScenario()
        {
            specFlowFeatureFileModel!.Scenarios[2].IsAccepted = false;
            var expectedResult = @"Feature: Traffic Light System Management

  The system controls traffic lights at intersections according to predefined timing sequences and transitions.

  @SFR-001
  Scenario Outline: Traffic light transitions based on timing sequence
    Given the traffic light is <CurrentState>
    And the time since entering the state is <ElapsedTime> seconds
    When the timing sequence is evaluated
    Then the traffic light should transition to <NextState>

    Examples:
      | CurrentState | ElapsedTime | NextState  |
      | red          | 15          | green      |
      | green        | 20          | yellow     |
      | yellow       | 5           | red        |

  @SFR-001a
  Scenario: Red traffic light turns on after yellow light duration
    Given the traffic light is yellow
    When the yellow light duration has elapsed
    Then the traffic light should transition to red

  # Recommendation: Edge Case
  # Scenario: Red traffic light transitions earlier than configured due to miscalculation
  #   Given the traffic light is yellow
  #   When the yellow light duration is improperly configured
  #   Then an alert should be raised and the system should correct the duration";
            var actualResult = GetReviewedFeatureFile.GetAcceptedScenarios(specFlowFeatureFileModel!);
            Assert.That(actualResult, Does.Contain(expectedResult));
        }

        [Test]
        public void GetAcceptedScenarios_RemoveMiddleScenario()
        {
            specFlowFeatureFileModel!.Scenarios[1].IsAccepted = false;
            var expectedResult = @"Feature: Traffic Light System Management

  The system controls traffic lights at intersections according to predefined timing sequences and transitions.

  @SFR-001
  Scenario Outline: Traffic light transitions based on timing sequence
    Given the traffic light is <CurrentState>
    And the time since entering the state is <ElapsedTime> seconds
    When the timing sequence is evaluated
    Then the traffic light should transition to <NextState>

    Examples:
      | CurrentState | ElapsedTime | NextState  |
      | red          | 15          | green      |
      | green        | 20          | yellow     |
      | yellow       | 5           | red        |

  # Recommendation: Edge Case
  # Scenario: Red traffic light transitions earlier than configured due to miscalculation
  #   Given the traffic light is yellow
  #   When the yellow light duration is improperly configured
  #   Then an alert should be raised and the system should correct the duration

  @SFR-001b
  Scenario: Yellow traffic light turns on when pedestrian button is pressed
    Given the traffic light is green
    When the pedestrian button is pressed
    Then the traffic light should transition to yellow";
            var actualResult = GetReviewedFeatureFile.GetAcceptedScenarios(specFlowFeatureFileModel!);
            Assert.That(actualResult, Does.Contain(expectedResult));
        }

        [Test]
        public void GetAcceptedScenarios_RemoveFirstScenario()
        {
            specFlowFeatureFileModel!.Scenarios[0].IsAccepted = false;
            var expectedResult = @"Feature: Traffic Light System Management

  The system controls traffic lights at intersections according to predefined timing sequences and transitions.

  @SFR-001a
  Scenario: Red traffic light turns on after yellow light duration
    Given the traffic light is yellow
    When the yellow light duration has elapsed
    Then the traffic light should transition to red

  # Recommendation: Edge Case
  # Scenario: Red traffic light transitions earlier than configured due to miscalculation
  #   Given the traffic light is yellow
  #   When the yellow light duration is improperly configured
  #   Then an alert should be raised and the system should correct the duration

  @SFR-001b
  Scenario: Yellow traffic light turns on when pedestrian button is pressed
    Given the traffic light is green
    When the pedestrian button is pressed
    Then the traffic light should transition to yellow";
            var actualResult = GetReviewedFeatureFile.GetAcceptedScenarios(specFlowFeatureFileModel!);
            Assert.That(actualResult, Does.Contain(expectedResult));
        }

        [Test]
        public void GetAcceptedScenarios_RemoveAllScenario()
        {
            specFlowFeatureFileModel!.Scenarios[0].IsAccepted = false;
            specFlowFeatureFileModel!.Scenarios[1].IsAccepted = false;
            specFlowFeatureFileModel!.Scenarios[2].IsAccepted = false;
            var expectedResult = @"Feature: Traffic Light System Management

  The system controls traffic lights at intersections according to predefined timing sequences and transitions.

  # Recommendation: Edge Case
  # Scenario: Red traffic light transitions earlier than configured due to miscalculation
  #   Given the traffic light is yellow
  #   When the yellow light duration is improperly configured
  #   Then an alert should be raised and the system should correct the duration";
            var actualResult = GetReviewedFeatureFile.GetAcceptedScenarios(specFlowFeatureFileModel!);
            Assert.That(actualResult, Does.Contain(expectedResult));
        }

        [Test]
        public void GetAcceptedScenarios_Remove2Scenario()
        {
            specFlowFeatureFileModel!.Scenarios[0].IsAccepted = false;
            specFlowFeatureFileModel!.Scenarios[2].IsAccepted = false;
            var expectedResult = @"Feature: Traffic Light System Management

  The system controls traffic lights at intersections according to predefined timing sequences and transitions.

  @SFR-001a
  Scenario: Red traffic light turns on after yellow light duration
    Given the traffic light is yellow
    When the yellow light duration has elapsed
    Then the traffic light should transition to red

  # Recommendation: Edge Case
  # Scenario: Red traffic light transitions earlier than configured due to miscalculation
  #   Given the traffic light is yellow
  #   When the yellow light duration is improperly configured
  #   Then an alert should be raised and the system should correct the duration";
            var actualResult = GetReviewedFeatureFile.GetAcceptedScenarios(specFlowFeatureFileModel!);
            Assert.That(actualResult, Does.Contain(expectedResult));
        }
    }
}