Feature: Governed Group Monitoring

When a group is governed
then its license processing state is captured via logs

Background: 
Given the Azure Active Directory Tenant 'tenant-one'

And groups in tenant 'tenant-one'
| DisplayName | License Processing State |
| group-1     | ProcessingComplete       |
| group-2     | QueuedForProcessing      |
| group-3     | ProcessingInProgress     |

Given license configuration in 'Enforce' mode for group 'group-1' in tenant 'tenant-one'
| Product        | Enabled Features                                            |
| PROJECTPREMIUM | PROJECT_PROFESSIONAL,FLOW_FOR_PROJECT,EXCHANGE_S_FOUNDATION |

Scenario: Tracked Groups are logged
Given the governed group 'group-1' is tracking license processing state
When governed groups are monitored
Then there is a 'Group Monitor|Processing State' message logged with data
| Name                        | Value              |
| TenantId                    | {tenant-one}       |
| GroupId                     | {group-1}          |
| DisplayName                 | group-1            |
| TrackLicenseProcessingState | true               |
| AssignmentMode              | Enforce            |
| LicenseProcessingState      | ProcessingComplete |

Scenario: Untracked Groups are not logged
Given the governed group 'group-1' is not tracking license processing state
When governed groups are monitored
Then there is not a 'Group Monitor|Processing State' message logged