Feature: Traffic Light and Pedestrian Light System

  Scenario: Traffic light system starts in red light
    Given the traffic light system starts
    When the system initializes
    Then the traffic light should be red

  Scenario: Pedestrian light system starts in red light
    Given the pedestrian light system starts
    When the system initializes
    Then the pedestrian light should be red