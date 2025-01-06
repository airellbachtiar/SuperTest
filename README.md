# Introduction 
The SuperTest project is a graduation internship project. SuperTest is a tool that can generate test cases (SpecFlow) from defined requirements. The goal of this project is to have a tool that can automate the creation SpecFlow test cases to accelerate development.

![architecture image](img/Architecture%20Image%20V4.png)

# Getting Started
## Setup
Make sure you have a valid Open AI, Anthropic, and Gemini keys. If you do have, put the key in the User environment variable by navigating from Search -> Edit the systems environment variables -> Environment Variables... -> and add these on the user an system variables:
|Variables|Value|
|---|---|
|SUPERTEST_ANTHROPIC_API_KEY|Your valid API key|
|SUPERTEST_GEMINI_API_KEY|Your valid API key|
|SUPERTEST_OPENAI_API_KEY|Your valid API key|

### First time setup
1. Ensure you can run an unsigned powershell script by using the command **"Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass"** in a powershell prompt before doing the next step.
2. In the same powershell session, run **".\SetupScript.ps1"**. It will clone the necessary repository at "C:\Dev\SuperRequirementsStorage" for shared git database and copy 2 files (Test1.reqif and Test2.reqif) to "C:\Dev\GitLocalFolderTest" for test purpose.