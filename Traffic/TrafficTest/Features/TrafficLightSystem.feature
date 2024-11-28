Feature: Traffic Light System

  As a traffic control system
  I want to manage traffic and pedestrian lights
  So that traffic flow and pedestrian safety are ensured

  @REQ-001
  Scenario: Start Traffic Light System
    Given the traffic light system is off
    When the user presses the start button
    Then the traffic light system should start

  @REQ-002
  Scenario: Default Light State
    Given the traffic light system is not started
    Then the car's yellow light should be blinking
    And the pedestrian green light should be blinking

  @REQ-003
  Scenario: Car and Pedestrian Light Initialization
    Given the traffic light system is off
    When the user starts the traffic light system
    Then the car's green light should turn on
    And the pedestrian red light should turn on

  @REQ-004
  Scenario: Car Yellow Light Transition
    Given the traffic light system has started
    When 1 second has passed
    Then the car's green light should switch to yellow
    And the pedestrian red light should remain on

  @REQ-005
  Scenario: Pedestrian Green Light
    Given the traffic light system has started
    When 6 seconds have passed
    Then the car's yellow light should switch to red
    And the pedestrian green light should turn on

  @REQ-006
  Scenario: Return to Car Green Light
    Given the traffic light system has started
    When 10 seconds have passed
    Then the pedestrian light should switch to red
    And the car green light should turn back on