Feature: Fetch ReqIF Files

  Background:
    Given I have a ReqIF file located at "C:\Dev\GitLocalFolderTest\Test1.reqif"
    And I have a ReqIF file located at "C:\Dev\GitLocalFolderTest\Test2.reqif"
    And I have a Git local folder located at "C:\Dev\GitLocalFolderTest"

  Scenario: Fetch ReqIF Files from Git Local Folder
    When I fetch ReqIF files from the Git local folder
    Then I should get the ReqIF file path "C:\Dev\GitLocalFolderTest\Test1.reqif"
    And I should get the ReqIF file path "C:\Dev\GitLocalFolderTest\Test2.reqif"
