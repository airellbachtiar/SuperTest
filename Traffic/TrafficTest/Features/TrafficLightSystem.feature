Feature: Traffic Light System

  Scenario: System transitions from idle to operational state
    Given the system is in idle state
    When the start button is pressed
    Then the system should transition to operational state
    And the green traffic light should be turned on

  Scenario: Traffic light transitions from green to yellow when pedestrian button is pressed
    Given the system is operational
    And the green traffic light is on
    When the pedestrian button is pressed
    Then the yellow traffic light should be turned on

  Scenario: Traffic light transitions from yellow to red
    Given the system is operational
    And the yellow traffic light is on
    When the yellow traffic light on time has elapsed
    Then the red traffic light should be turned on

  Scenario: Traffic light transitions from red to green
    Given the system is operational
    And the red traffic light is on
    When the red traffic light on time has elapsed
    Then the green traffic light should be turned on

  Scenario: Only one traffic light is on at a time
    Given the system is operational
    When any traffic light is on
    Then only one traffic light should be on

  Scenario: Yellow traffic light blinks in idle state
    Given the system is in idle state
    Then the yellow traffic light should be blinking

# Recommended scenario:
# Scenario: System handles power outage
#   Given the system is operational
#   When a power outage occurs
#   Then the system should safely shut down
#   And resume in idle state when power is restored

# Recommended scenario:
# Scenario: System handles invalid button presses
#   Given the system is operational
#   When an invalid button combination is pressed
#   Then the system should ignore the input
#   And maintain its current state