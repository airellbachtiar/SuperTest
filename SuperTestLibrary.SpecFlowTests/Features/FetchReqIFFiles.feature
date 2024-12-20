Feature: Fetch ReqIF Files

Scenario: Fetch ReqIF files from specified directory
  Given the application is running
  When I request to fetch ReqIF files
  Then the application should fetch "Test1.reqif" from "C:\Dev\GitLocalFolderTest"
  And the application should fetch "Test2.reqif" from "C:\Dev\GitLocalFolderTest"

Scenario Outline: Verify fetched ReqIF files
  Given the application has fetched ReqIF files
  When I check the fetched files
  Then the file "<FileName>" should exist in the application's data
  And the file "<FileName>" should have been fetched from "C:\Dev\GitLocalFolderTest"

  Examples:
    | FileName    |
    | Test1.reqif |
    | Test2.reqif |

Scenario: Attempt to fetch non-existent ReqIF file
  Given the application is running
  When I request to fetch a non-existent ReqIF file "NonExistent.reqif" from "C:\Dev\GitLocalFolderTest"
  Then the application should report an error
  And the error message should indicate that the file was not found

Scenario: Fetch ReqIF files with invalid directory path
  Given the application is running
  When I request to fetch ReqIF files from an invalid directory "C:\InvalidPath"
  Then the application should report an error
  And the error message should indicate that the directory is invalid or inaccessible