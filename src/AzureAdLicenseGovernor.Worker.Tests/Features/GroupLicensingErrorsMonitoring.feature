Feature: Group License Assignment Errors Are Monitored

Given a group has assigned licenses defined 
And there are errors in assigning those licenses to its members
Then there are messages logged indicating there is a failure

Background: 
Given the Azure Active Directory Tenant 'tenant-one'
And the Tenant 'tenant-one' is configured to track group license assignment errors

And groups in tenant 'tenant-one'
| DisplayName | License Processing State |
| group-1     | ProcessingComplete       |
| group-2     | QueuedForProcessing      |
| group-3     | ProcessingInProgress     |

Scenario: Groups with licensing errors are logged
Given the group 'group-1' has license assignment errors
When governed groups are monitored
Then there is a 'Group Monitor|Licensing Errors' message logged with data
| Name                        | Value              |
| TenantId                    | {tenant-one}       |
| GroupId                     | {group-1}          |
| DisplayName                 | group-1            |
| LicenseProcessingState      | ProcessingComplete |

Scenario: Groups without licensing errors are not logged
Given the group 'group-1' does not have license assignment errors
When governed groups are monitored
Then there is not a 'Group Monitor|Licensing Errors' message logged

Scenario: Groups with licensing errors in a tenant that is not tracking failures are not logged
Given the group 'group-1' has license assignment errors
And the Tenant 'tenant-one' is not configured to track group license assignment errors
When governed groups are monitored
Then there is not a 'Group Monitor|Licensing Errors' message logged