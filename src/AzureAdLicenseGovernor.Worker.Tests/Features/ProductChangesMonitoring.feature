Feature: Product Changes are Monitored

Given a tenant is configured to monitor product usage
Then the product usage is logged

Background: 
Given the Azure Active Directory Tenant 'tenant-one'

And licensed products in 'tenant-one'
| SkuId                                | SkuPartNumber  |
| c5928f49-12ba-48f7-ada3-0d743a3601d5 | VISIOCLIENT    |
| 09015f9f-377f-4538-bbb5-f75ceb09358a | PROJECTPREMIUM |
And service plans in 'tenant-one' for product 'VISIOCLIENT'
| ServicePlanId                        | ServicePlanName           |
| da792a53-cbc0-4184-a10d-e544dd34b3c1 | ONEDRIVE_BASIC            |
| 2bdbaf8f-738f-4ac7-9234-3c3ee2ce7d0f | VISIOONLINE               |
| 113feb6c-3fe4-4440-bddc-54d774bf0318 | EXCHANGE_S_FOUNDATION     |
| 663a804f-1c30-4ff0-9915-9db84f0d1cea | VISIO_CLIENT_SUBSCRIPTION |

Scenario: Tenant Does Not Tracks Changes
Given the Tenant 'tenant-one' is configured to not track product changes
And products are previously monitored
When products are monitored
Then there is not a 'Product Monitoring|Change Summary' message logged
And there is not a 'Product Monitoring|Change Summary:None' message logged

Scenario: Tenant Tracks Changes - No Changes Found
Given the Tenant 'tenant-one' is configured to track product changes
And the snapshot for licensed products in 'tenant-one' has products
| SkuId                                | SkuPartNumber  |
| c5928f49-12ba-48f7-ada3-0d743a3601d5 | VISIOCLIENT    |
| 09015f9f-377f-4538-bbb5-f75ceb09358a | PROJECTPREMIUM |
And the snapshot for product 'VISIOCLIENT' in 'tenant-one' has service plans
| ServicePlanId                        | ServicePlanName           |
| da792a53-cbc0-4184-a10d-e544dd34b3c1 | ONEDRIVE_BASIC            |
| 2bdbaf8f-738f-4ac7-9234-3c3ee2ce7d0f | VISIOONLINE               |
| 113feb6c-3fe4-4440-bddc-54d774bf0318 | EXCHANGE_S_FOUNDATION     |
| 663a804f-1c30-4ff0-9915-9db84f0d1cea | VISIO_CLIENT_SUBSCRIPTION |
When products are monitored
Then there is a 'Product Monitoring|Change Summary:None' message logged with data
| Name     | Value        |
| TenantId | {tenant-one} |
| Added    | 0            |
| Removed  | 0            |
| Updated  | 0            |
And there is not a 'Product Monitoring|New Product' message logged
And there is not a 'Product Monitoring|Removed Product' message logged
And there is not a 'Product Monitoring|Updated Product' message logged

Scenario: Tenant Tracks Changes - Products Added
Given the Tenant 'tenant-one' is configured to track product changes
And the snapshot for licensed products in 'tenant-one' has products
| SkuId                                | SkuPartNumber  |
| c5928f49-12ba-48f7-ada3-0d743a3601d5 | VISIOCLIENT    |
And the snapshot for product 'VISIOCLIENT' in 'tenant-one' has service plans
| ServicePlanId                        | ServicePlanName           |
| da792a53-cbc0-4184-a10d-e544dd34b3c1 | ONEDRIVE_BASIC            |
| 2bdbaf8f-738f-4ac7-9234-3c3ee2ce7d0f | VISIOONLINE               |
| 113feb6c-3fe4-4440-bddc-54d774bf0318 | EXCHANGE_S_FOUNDATION     |
| 663a804f-1c30-4ff0-9915-9db84f0d1cea | VISIO_CLIENT_SUBSCRIPTION |
When products are monitored
Then there is a 'Product Monitoring|Change Summary' message logged with data
| Name     | Value        |
| TenantId | {tenant-one} |
| Added    | 1            |
| Removed  | 0            |
| Updated  | 0            |
And there is a 'Product Monitoring|New Product' message logged with data
| Name        | Value                                |
| TenantId    | {tenant-one}                         |
| ProductId   | 09015f9f-377f-4538-bbb5-f75ceb09358a |
| ProductName | PROJECTPREMIUM                       |
And there is not a 'Product Monitoring|Removed Product' message logged
And there is not a 'Product Monitoring|Updated Product' message logged

Scenario: Tenant Tracks Changes - Products Removed
Given the Tenant 'tenant-one' is configured to track product changes
And the snapshot for licensed products in 'tenant-one' has products
| SkuId                                | SkuPartNumber  |
| c5928f49-12ba-48f7-ada3-0d743a3601d5 | VISIOCLIENT    |
| 09015f9f-377f-4538-bbb5-f75ceb09358a | PROJECTPREMIUM |
| d4cc2f49-c31e-4613-b8fc-a4c5d46da8e2 | DYNAMICS       |
And the snapshot for product 'VISIOCLIENT' in 'tenant-one' has service plans
| ServicePlanId                        | ServicePlanName           |
| da792a53-cbc0-4184-a10d-e544dd34b3c1 | ONEDRIVE_BASIC            |
| 2bdbaf8f-738f-4ac7-9234-3c3ee2ce7d0f | VISIOONLINE               |
| 113feb6c-3fe4-4440-bddc-54d774bf0318 | EXCHANGE_S_FOUNDATION     |
| 663a804f-1c30-4ff0-9915-9db84f0d1cea | VISIO_CLIENT_SUBSCRIPTION |
And the snapshot for product 'DYNAMICS' in 'tenant-one' has service plans
| ServicePlanId                        | ServicePlanName       |
| 113feb6c-3fe4-4440-bddc-54d774bf0318 | EXCHANGE_S_FOUNDATION |
When products are monitored
Then there is a 'Product Monitoring|Change Summary' message logged with data
| Name     | Value        |
| TenantId | {tenant-one} |
| Added    | 0            |
| Removed  | 1            |
| Updated  | 0            |
And there is not a 'Product Monitoring|New Product' message logged
And there is a 'Product Monitoring|Removed Product' message logged with data
| Name        | Value                                |
| TenantId    | {tenant-one}                         |
| ProductId   | d4cc2f49-c31e-4613-b8fc-a4c5d46da8e2 |
| ProductName | DYNAMICS                             |
And there is not a 'Product Monitoring|Updated Product' message logged
And there is not a 'Product Monitoring|New Service Plan' message logged
And there is not a 'Product Monitoring|Removed Service Plan' message logged

Scenario: Tenant Tracks Changes - Service Plans Removed
Given the Tenant 'tenant-one' is configured to track product changes
And the snapshot for licensed products in 'tenant-one' has products
| SkuId                                | SkuPartNumber  |
| c5928f49-12ba-48f7-ada3-0d743a3601d5 | VISIOCLIENT    |
| 09015f9f-377f-4538-bbb5-f75ceb09358a | PROJECTPREMIUM |
And the snapshot for product 'VISIOCLIENT' in 'tenant-one' has service plans
| ServicePlanId                        | ServicePlanName           |
| da792a53-cbc0-4184-a10d-e544dd34b3c1 | ONEDRIVE_BASIC            |
| 2bdbaf8f-738f-4ac7-9234-3c3ee2ce7d0f | VISIOONLINE               |
| 113feb6c-3fe4-4440-bddc-54d774bf0318 | EXCHANGE_S_FOUNDATION     |
| 663a804f-1c30-4ff0-9915-9db84f0d1cea | VISIO_CLIENT_SUBSCRIPTION |
| 7edc25a2-bbb7-443e-9221-14704e7e45ac | SOMETHING_ELSE            |
When products are monitored
Then there is a 'Product Monitoring|Change Summary' message logged with data
| Name     | Value        |
| TenantId | {tenant-one} |
| Added    | 0            |
| Removed  | 0            |
| Updated  | 1            |
And there is not a 'Product Monitoring|New Product' message logged
And there is not a 'Product Monitoring|Removed Product' message logged
And there is a 'Product Monitoring|Updated Product' message logged with data
| Name        | Value                                |
| TenantId    | {tenant-one}                         |
| ProductId   | c5928f49-12ba-48f7-ada3-0d743a3601d5 |
| ProductName | VISIOCLIENT                          |
And there is not a 'Product Monitoring|New Service Plan' message logged
And there is a 'Product Monitoring|Removed Service Plan' message logged with data
| Name            | Value                                |
| TenantId        | {tenant-one}                         |
| ProductId       | c5928f49-12ba-48f7-ada3-0d743a3601d5 |
| ProductName     | VISIOCLIENT                          |
| ServicePlanId   | 7edc25a2-bbb7-443e-9221-14704e7e45ac |
| ServicePlanName | SOMETHING_ELSE                       |

Scenario: Tenant Tracks Changes - Service Plans Added
Given the Tenant 'tenant-one' is configured to track product changes
And the snapshot for licensed products in 'tenant-one' has products
| SkuId                                | SkuPartNumber  |
| c5928f49-12ba-48f7-ada3-0d743a3601d5 | VISIOCLIENT    |
| 09015f9f-377f-4538-bbb5-f75ceb09358a | PROJECTPREMIUM |
And the snapshot for product 'VISIOCLIENT' in 'tenant-one' has service plans
| ServicePlanId                        | ServicePlanName           |
| da792a53-cbc0-4184-a10d-e544dd34b3c1 | ONEDRIVE_BASIC            |
| 2bdbaf8f-738f-4ac7-9234-3c3ee2ce7d0f | VISIOONLINE               |
| 113feb6c-3fe4-4440-bddc-54d774bf0318 | EXCHANGE_S_FOUNDATION     |
When products are monitored
Then there is a 'Product Monitoring|Change Summary' message logged with data
| Name     | Value        |
| TenantId | {tenant-one} |
| Added    | 0            |
| Removed  | 0            |
| Updated  | 1            |
And there is not a 'Product Monitoring|New Product' message logged
And there is not a 'Product Monitoring|Removed Product' message logged
And there is a 'Product Monitoring|Updated Product' message logged with data
| Name        | Value                                |
| TenantId    | {tenant-one}                         |
| ProductId   | c5928f49-12ba-48f7-ada3-0d743a3601d5 |
| ProductName | VISIOCLIENT                          |
And there is a 'Product Monitoring|New Service Plan' message logged with data
| Name            | Value                                |
| TenantId        | {tenant-one}                         |
| ProductId       | c5928f49-12ba-48f7-ada3-0d743a3601d5 |
| ProductName     | VISIOCLIENT                          |
| ServicePlanId   | 663a804f-1c30-4ff0-9915-9db84f0d1cea |
| ServicePlanName | VISIO_CLIENT_SUBSCRIPTION            |
And there is not a 'Product Monitoring|Removed Service Plan' message logged