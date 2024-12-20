Feature: Pedestrian Light System

  Scenario: Pedestrian light turns red when system starts
    Given the system is in idle state
    When the start button is pressed
    Then the red pedestrian light should be turned on

  Scenario: Pedestrian light transitions from red to green
    Given the system is operational
    And the red pedestrian light is on
    When the pedestrian button is pressed
    Then the green pedestrian light should be turned on within 10 seconds

  Scenario: Pedestrian light blinks green before turning red
    Given the system is operational
    And the green pedestrian light is on
    When the green pedestrian light on time has elapsed
    Then the green pedestrian light should start blinking

  Scenario: Pedestrian light transitions from green to red
    Given the system is operational
    And the green pedestrian light is on
    When the green pedestrian light on time and blinking time have elapsed
    Then the red pedestrian light should be turned on

  Scenario: Only one pedestrian light is on at a time
    Given the system is operational
    When any pedestrian light is on
    Then only one pedestrian light should be on

# Recommended scenario:
# Scenario: System handles rapid pedestrian button presses
#   Given the system is operational
#   When the pedestrian button is pressed multiple times in quick succession
#   Then the system should only register one button press
#   And respond accordingly

# Recommended scenario:
# Scenario: Pedestrian light remains red during traffic light malfunction
#   Given the system is operational
#   When a traffic light malfunction is detected
#   Then the pedestrian light should remain red
#   Until the malfunction is resolved