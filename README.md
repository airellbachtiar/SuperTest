# Introduction 
The SuperTest project is a graduation internship project. SuperTest is a tool that can generate test cases (SpecFlow) from defined requirements. The goal of this project is to have a tool that can automate the creation SpecFlow test cases to accelerate development.

SuperTest support generation of SpecFlow feature file, binding file, requirements, and SpecFlow feature file evaluation.

SuperTest is designed to be able to support generator that uses Large Language Model (LLM) as the main part of the generation.

![architecture image](img/Architecture%20Image%20V4.png)

# Getting Started
## Setup
Make sure you have a valid Open AI, Anthropic, and Gemini keys. If you do have, put the key in the User environment variable by navigating from Search -> Edit the systems environment variables -> Environment Variables... -> and add these on the user an system variables:
|Variables|Value|
|---|---|
|ANTHROPIC_API_KEY|Your valid API key|
|GEMINI_API_KEY|Your valid API key|
|OPENAI_API_KEY|Your valid API key|

or

|Variables|Value|
|---|---|
|SUPERTEST_ANTHROPIC_API_KEY|Your valid API key|
|SUPERTEST_GEMINI_API_KEY|Your valid API key|
|SUPERTEST_OPENAI_API_KEY|Your valid API key|

### First time setup
1. Ensure you can run an unsigned powershell script by using the command **"Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass"** in a powershell prompt before doing the next step.
2. In the same powershell session, run **".\SetupScript.ps1"**. It will clone the necessary repository at "C:\Dev\SuperRequirementsStorage" for shared git database and copy 2 files (Test1.reqif and Test2.reqif) to "C:\Dev\GitLocalFolderTest" for test purpose.

# Large Language Model Library
This repository includes a Large Language Model (LLM) library with two versions: LlmLibrary (deprecated) and LargeLanguageModelLibrary.

LlmLibrary was an early attempt to create a shared LLM library that could work with different providers, making it easier to integrate LLMs into projects. It only supported basic functionality for generating SpecFlow acceptance tests. It relied on an external library to handle LLM calls, meaning it could only process text input/output and had limited control due to pre-configured options.

LargeLanguageModelLibrary is a reworked and extended version of LlmLibrary. It takes inspiration from [OpenAI .NET API library](https://github.com/openai/openai-dotnet/tree/OpenAI_2.1.0) and [Claudia](https://github.com/Cysharp/Claudia/tree/main). External libraries were removed in favor of direct HTTP calls. It allows assigning roles (e.g., developer, user, assistant) when sending messages, supports image-based messages, and enables prompt chaining by returning all responses—something regular APIs don't typically support.