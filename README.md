# Azure Active Directory License Governor
The Azure Active Directory License Governor enables managment of user license assignment in Azure Active Directory so the license configuration can be managed as code and any configuration drift is mitigated through continuous enforcement.  Using [Group Based Licensing](https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/active-directory-licensing-whatis-azure-portal) as a foundation, this solution configures the assigned licenses and service plans to groups in multiple defined domains.

## Configuration (Infrastructure) as Code
Using the principles of [Infrastructure as Code](https://docs.microsoft.com/en-us/devops/deliver/what-is-infrastructure-as-code), assigned licenses and their configuration are defined as code deployed to the solution to be implemented and enforced.  This enables all license configuration to be managed by whomever needs to and not only by Directory Admins or the engineering team who builds a custom licensing solution.

A sample configuration for a group that assigns Microsoft Visio licenses to members of the defined group is:
```
{
    "GroupId":"62edbd7b-8d46-4d2c-a5a1-da5b78ba1d38",
    "TenantId":"72f988bf-86f1-41af-91ab-2d7cd011db47",
    "Mode":"Enforce",
    "Products":{
        "SkuId":"VISIOCLIENT",
        "ServicePlans:{
            "Enabled":[
                "VISIOONLINE",
                "VISIO_CLIENT_SUBSCRIPTION",
            ]
        }
    }
}
```

## Managing Configuration Drift
Licensing configuration drift occurs when licenses are added, removed, or updated from specific groups outside of standard processes.  This can occur when a person manually updates the assigned licneses for a group, a rogue user changes configuration programatically without others knowing, or when Microsoft pushes out new features (service plans) that are enabled by default for a product.  For any of these situations, what products and features expected to be licensed by an organization can drift away from what is licensed in reality, causing drift.

To solve the problem of drift, this solution applies the configuration to the specific groups on a continual basis, the intervals which are configurable.  When ran frequently enough, any changes made by outside events are immediately snapped back into the defined configuration, thus ensuring that only the expected products and features are enabled for the users in the group

## Solution Architecture
This solution is broken up into two main components, an API and a background worker.  The API enables [CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete) operations on the configuration, and the background worker is the component that applies and enforces the configuration.  As this is a native Azure application, all components use Azure PaaS services such as Azure Functions, Key Vault, CosmosDb, and Managed Identities.

### Component Diagram
For more detail, a [Threat model](https://owasp.org/www-community/Threat_Modeling) for the solution can be found at [docs/threat-model.tm7](docs/threat-model.tm7) that shows all the components and their interactions.

### Infrastructure
The infrastructure that is needed to run this service can be found at https://github.com/jrolstad/aad-license-governor-infra where it is defined using [Terraform](https://www.terraform.io/)

## How to use
If this solution is something that you want to implement for Azure Active Directory tenants that you manage, below is a high level of steps needed.  If there are further questions, contact the Administrators of this repository who will be able to give more guidance.
1. Create the infrastucture in Azure using https://github.com/jrolstad/aad-license-governor-infra or your own setup.
2. Deploy the Azure Active Directory Licensing Governor to those components using your continuous delivery tool of choice (GitHub Actions, Azure DevOps, Jenkins, etc).  As a part of this deployment be sure to set the interval for the background worker to the expected value so configuration is continually applied when needed.
3. Define configurations for each group and post to the /api/group/license endpoint.
4. Once the configurations are posted, review the application of these configurations in the Application Insights logs used by the solution