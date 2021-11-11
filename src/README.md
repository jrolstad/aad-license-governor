# Azure Active Directory License Governor

# Requirements
* Visual Studio 2022 or later
* .NET 6
* Azure Functions Runtime v4 or later
* Specflow for Visual Studio Extension (Used for editing AzureAdLicenseGovernor.Worker tests)

# Projects
## Applications
| Project                       | Type            | Purpose                                     |
| ----------------------------- | --------------- | ------------------------------------------- |
| AzureAdLicenseGovernor.Api    | Azure Functions | API Layer for the application               |
| AzureAdLicenseGovernor.Worker | Azure Functions | Background worker tasks for the application |

## Libraries
| Project                     | Type          | Purpose                                             |
| --------------------------- | ------------- | --------------------------------------------------- |
| AzureAdLicenseGovernor.Core | .NET Standard | Shared library that contains the core functionality |

## Tests
| Project                             | Type        | Purpose                                  |
| ----------------------------------- | ----------- | ---------------------------------------- |
| AzureAdLicenseGovernor.Api.Tests    | xUnit Tests | Unit Tests for the API project           |
| AzureAdLicenseGovernor.Worker.Tests | xUnit Tests | Unit Tests for the Worker project        |
| AzureAdLicenseGovernor.TestUtility  | .NET Core   | Shared test setup library for unit tests |

# Pipelines
GitHub Actions are used for continuous integration builds
## Builds
| Build                                                       | Purpose                                                                                                       |
| ----------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------- |
| [continuous-integration](../.github/workflows/ci-build.yml) | Continuous integration build.  Builds the solution, runs tests, and passes only when all tests are successful |

# API Documentation
Swagger is used to document our endpoints and make them available to be manually invoked if needed.
| Environment         | Location                            |
| ------------------- | ----------------------------------- |
| Development (local) | http://locahost:7071/api/swagger/ui |

# Development Environment Setup
To run the solution in your development environment, use the following steps.

## Azure Function Settings File
### Api
The AzureAdLicenseGovernor.Api is a set of HTTP based Azure Functions.  Be sure you have a local.settings.json file in the function root directory.  A sample file is below:
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "KeyVault_BaseUri": "https://aadlicense-dev.vault.azure.net/",
    "Cosmos_BaseUri": "https://aadlicense-dev.documents.azure.com:443/"
  },
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "*"
  }
}
```

### Worker
The AzureAdLicenseGovernor.Worker is a set of background worker Azure Functions.  Be sure you have a local.settings.json file in the function root directory.  A sample file is below:
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "KeyVault_BaseUri": "https://aadlicense-dev.vault.azure.net/",
    "Cosmos_BaseUri": "https://aadlicense-dev.documents.azure.com:443/"
    "GroupLicenseFunction_ApplyCron": "0 */1 * * * *",
    "GroupLicenseFunction_MonitorCron": "0 */1 * * * *",
    "ProductFunction_MonitorCron": "0 */1 * * * *"
  }
}
```