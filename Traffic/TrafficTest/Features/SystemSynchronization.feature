Feature: System Synchronization

  Scenario: Traffic and pedestrian lights are synchronized
    Given the system is operational
    When the traffic light is green or yellow
    Then the pedestrian light should be red

  Scenario: Pedestrian light turns green when traffic light is red
    Given the system is operational
    And the traffic light is red
    When the pedestrian button has been pressed
    Then the pedestrian light should turn green

  Scenario: Traffic light turns red when pedestrian light is green
    Given the system is operational
    And the pedestrian light is green
    Then the traffic light should be red

# Recommended scenario:
# Scenario: System handles conflicting inputs
#   Given the system is operational
#   When conflicting inputs are received simultaneously
#   Then the system should prioritize safety
#   And maintain a consistent state for both traffic and pedestrian lights

# Recommended scenario:
# Scenario: System transitions through a complete cycle
#   Given the system is operational
#   When a complete cycle of traffic and pedestrian lights is observed
#   Then all states should be correctly sequenced
#   And timing requirements should be met for each state