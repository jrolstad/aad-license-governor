# Azure Active Directory License Governor
The Azure Active Directory License Governor enables managment of user license assignment in Azure Active Directory so the license configuration can be managed as code and any configuration drift is mitigated through continuous enforcement.  Using [Group Based Licensing](https://docs.microsoft.com/en-us/azure/active-directory/fundamentals/active-directory-licensing-whatis-azure-portal) as a foundation, this solution configures the assigned licenses and service plans to groups in multiple defined domains.

## Configuration (Infrastructure) as Code
Using the principles of [Infrastructure as Code](https://docs.microsoft.com/en-us/devops/deliver/what-is-infrastructure-as-code), assigned licenses and their configuration are defined as code deployed to the solution to be implemented and enforced.  This enables all license configuration to be managed by whomever needs to and not only by Directory Admins or the engineering team who builds a custom licensing solution.

A sample configuration for a group that assigns Microsoft Enterprise Mobility licenses to members of the defined group is:
```
{
    "ObjectId": "4cc31dc9-96b4-464e-af1a-0e75e4bde0f8",
    "TenantId": "335776b5-3fba-4122-bcef-84458b1b8201",
    "Mode": "Enforce",
    "TrackLicenseProcessingState": true,
    "LicensedProducts": [
        {
            "Id": "EMSPREMIUM",
            "EnabledServicePlans": [
                "EXCHANGE_S_FOUNDATION",
                "ATA"
            ]
        }
    ]
}

```

## Managing Configuration Drift
Licensing configuration drift occurs when licenses are added, removed, or updated from specific groups outside of standard processes.  This can occur when a person manually updates the assigned licneses for a group, a rogue user changes configuration programatically without others knowing, or when Microsoft pushes out new features (service plans) that are enabled by default for a product.  For any of these situations, what products and features expected to be licensed by an organization can drift away from what is licensed in reality, causing drift.

To solve the problem of drift, this solution applies the configuration to the specific groups on a continual basis, the intervals which are configurable.  When ran frequently enough, any changes made by outside events are immediately snapped back into the defined configuration, thus ensuring that only the expected products and features are enabled for the users in the group

## Monitoring
In addition to applying license configuration and managing configuration drift, there is a need for monitoring licensing related events.  To enable this, the following events are monitored with results written to the underlying logging solution (Application Insights) so actionable alerts can then be configured when desired actions occur.  All items are in the customEvents or customMetrics areas of Application Insights.

### Group License Processing
For each group that is configured to manage licenses for, there is the option to monitor the license processing state.  Available values can be found under the [licenseProcessingState](https://docs.microsoft.com/en-us/graph/api/resources/group?view=graph-rest-1.0#properties) in the graph api documentation.

This functionality is enabled by default and runs on the schedule defined for the _license-governance-monitor_ function.

### Group License Assignment Errors
For all groups using the group based licensing functionality of Azure to apply licenses to users, there is a chance for licensing conflicts or other failures.  This is visible in the Azure Portal or via graph api, but not easily searchable over time.  To enable searchability and tracing, all groups with license assignment failures are monitored 

This functionality is disabled by default and runs on the schedule defined for the _license-governance-monitor_ function.  To enable this monitoring, it is done on a tenant by tenant basis by setting _TrackGroupLicenseAssignmentFailures_ on the directory configuration to true.

```
{
    ...
    "Monitoring:{
        "TrackGroupLicenseAssignmentFailures":true
        ...
    }
}
```

### Tenant Available Product Changes
When products are added / removed from the tenant or their this of available service plans modified, these are often events that require some type of action to be taken.  To enable these actions and to allow tracking of products available over time, any changes to the licensed products in a tenant are monitored.

This functionality is disabled by default and runs on the schedule defined for the _licensed-product-monitor_ function.  To enable this monitoring, it is done on a tenant by tenant basis by setting _TrackProductChanges_ on the directory configuration to true.

```
{
    ...
    "Monitoring:{
        "TrackProductChanges":true
        ...
    }
}
```

### Tenant Available Product Usage
When licensed products are assigned, over time there is a risk that the number of assigned licenses will exceed those that are available.  If this occurs, license assignment will fail and more license seats will need to be purchased.  To ensure that there is proper time to react to these events, the usage of each product can be monitored.  This captures the number assigned, number available, and percent usage for each product enabled in the tenant.

This functionality is disabled by default and runs on the schedule defined for the _licensed-product-monitor_ function.  To enable this monitoring, it is done on a tenant by tenant basis by setting _TrackProductUsage_ on the directory configuration to true.

```
{
    ...
    "Monitoring:{
        "TrackProductUsage":true
        ...
    }
}
```

## Solution Architecture
This solution is broken up into two main components, an API and a background worker.  The API enables [CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete) operations on the configuration, and the background worker is the component that applies and enforces the configuration.  As this is a native Azure application, all components use Azure PaaS services such as Azure Functions, Key Vault, CosmosDb, and Managed Identities.

### Component Diagram
For more detail, a [Threat model](https://docs.microsoft.com/en-us/azure/security/develop/threat-modeling-tool) for the solution can be found at [docs/threat-model.tm7](docs/threat-model.tm7) that shows all the components and their interactions.  For a more detailed view of the individual components, underlying code, and their interactions see [src/README.md](src/README.md)

### Infrastructure
The infrastructure that is needed to run this service can be found at https://github.com/jrolstad/aad-license-governor-infra where it is defined using [Terraform](https://www.terraform.io/)

## How to use
If this solution is something that you want to implement for Azure Active Directory tenants that you manage, below is a high level of steps needed.  If there are further questions, contact the Administrators of this repository who will be able to give more guidance.
1. Create the infrastucture in Azure using https://github.com/jrolstad/aad-license-governor-infra or your own setup.
2. Deploy the Azure Active Directory Licensing Governor to those components using your continuous delivery tool of choice (GitHub Actions, Azure DevOps, Jenkins, etc).  As a part of this deployment be sure to set the interval for the background worker to the expected value so configuration is continually applied when needed.
3. Define configurations for each group and post to the /api/group/license endpoint.
4. Once the configurations are posted, review the application of these configurations in the Application Insights logs used by the solution