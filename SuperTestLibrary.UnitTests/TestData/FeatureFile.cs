namespace SuperTestLibrary.UnitTests.TestData
{
    public static class FeatureFile
    {
        public static string InvalidSpecFlowFeatureFile => @"Feature: TestFeature";
        public static string ValidSpecFlowFeatureFile => "Feature: Enhanced Traffic Light and Pedestrian Light System\r\n\r\n  @REQ-001\r\n  Scenario: Traffic light system starts in red light\r\n    Given the traffic light system starts\r\n    When the system initializes\r\n    Then the traffic light should be red\r\n\r\n  @REQ-008\r\n  Scenario: Pedestrian light system starts in red light\r\n    Given the pedestrian light system starts\r\n    When the system initializes\r\n    Then the pedestrian light should be red";
    }
}
