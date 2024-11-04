Feature: Generate SpecFlow Feature File Using LLM

  As a user
  I want to generate SpecFlow feature files using an LLM
  So that I can create test scenarios based on requirements

  Scenario: Generate SpecFlow feature file using chosen LLM
    Given I have a set of requirements
    And I have chosen an LLM for generation
    When I request to generate a SpecFlow feature file
    Then the application should generate a valid SpecFlow feature file
    And the generated file should reflect the provided requirements

  Scenario: Choose LLM for SpecFlow feature file generation
    Given I am using the application
    When I select an LLM from the available options
    Then the application should use the selected LLM for feature file generation

  Scenario: Generate feature file with empty requirements
    Given I have an empty set of requirements
    When I request to generate a SpecFlow feature file
    Then the application should handle the situation appropriately
    And provide feedback that no feature file can be generated