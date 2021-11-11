Feature: Product Assignment Usage is Monitored

Given a tenant is configured to monitor product usage
Then the product usage is logged

Background: 
Given the Azure Active Directory Tenant 'tenant-one'

And licensed products in 'tenant-one'
| SkuId                                | SkuPartNumber  | Units Consumed | Units Enabled | Units Suspended | Units Warning |
| c5928f49-12ba-48f7-ada3-0d743a3601d5 | VISIOCLIENT    | 10             | 50            | 0               | 5             |

Scenario: Tenant Tracks Usage
Given the Tenant 'tenant-one' is configured to track product assignment usage
When products are monitored
Then there is a 'Product Monitoring|LicenseUsage' message logged with data
| Name              | Value                                |
| TenantId          | {tenant-one}                         |
| ProductId         | c5928f49-12ba-48f7-ada3-0d743a3601d5 |
| ProductName       | VISIOCLIENT                          |
| Units-Total       | 55                                   |
| Units-Assigned    | 10                                   |
| Units-Available   | 45                                   |
| Units-PercentUsed | 0.18182                              |
| Units-Warning     | 5                                    |
| Units-Suspended   | 0                                    |

Scenario: Tenant Does Not Tracks Usage
Given the Tenant 'tenant-one' is configured to not track product assignment usage
When products are monitored
Then there is not a 'Product Monitoring|LicenseUsage' message logged