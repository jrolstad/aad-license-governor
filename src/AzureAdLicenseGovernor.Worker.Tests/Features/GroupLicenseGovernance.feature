Feature: Group License Governance

When a group has assigned licenses defined 
then it is only assigned in AAD to the products and service plans defined

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
And service plans in 'tenant-one' for product 'PROJECTPREMIUM'
| ServicePlanId                        | ServicePlanName             |
| 818523f5-016b-4355-9be8-ed6944946ea7 | PROJECT_PROFESSIONAL        |
| fa200448-008c-4acb-abd4-ea106ed2199d | FLOW_FOR_PROJECT            |
| 50554c47-71d9-49fd-bc54-42a2765c555c | DYN365_CDS_PROJECT          |
| 113feb6c-3fe4-4440-bddc-54d774bf0318 | EXCHANGE_S_FOUNDATION       |
| e95bec33-7c88-4a70-8e19-b10bd9d0c014 | SHAREPOINTWAC               |
| fe71d6c3-a2ea-4499-9778-da042bf08063 | SHAREPOINT_PROJECT          |
| 5dbe027f-2339-4123-9542-606e4d348a72 | SHAREPOINTENTERPRISE        |
| fafd7243-e5c1-4a3a-9e40-495efcb1d3c3 | PROJECT_CLIENT_SUBSCRIPTION |

And groups in tenant 'tenant-one'
| DisplayName |
| group-1     |
| group-2     |
| group-3     |

Given license configuration in 'Enforce' mode for group 'group-1' in tenant 'tenant-one'
| Product        | Enabled Features                                            |
| PROJECTPREMIUM | PROJECT_PROFESSIONAL,FLOW_FOR_PROJECT,EXCHANGE_S_FOUNDATION |
And license configuration in 'Enforce' mode for group 'group-2' in tenant 'tenant-one'
| Product     | Enabled Features                                                           |
| VISIOCLIENT | ONEDRIVE_BASIC,VISIOONLINE,EXCHANGE_S_FOUNDATION,VISIO_CLIENT_SUBSCRIPTION |

Scenario: New Assignments Are Added to the Group
	Given the group 'group-1' in tenant 'tenant-one' has no license assignments
	Given the group 'group-2' in tenant 'tenant-one' has no license assignments
	When the license configuration is applied
	Then the group 'group-1' in tenant 'tenant-one' has license assignments
	| Product        | Disabled Features                                                                                    |
	| PROJECTPREMIUM | DYN365_CDS_PROJECT,SHAREPOINTWAC,SHAREPOINT_PROJECT,SHAREPOINTENTERPRISE,PROJECT_CLIENT_SUBSCRIPTION |
	And the group 'group-2' in tenant 'tenant-one' has license assignments
	| Product     | Disabled Features |
	| VISIOCLIENT |                   |

Scenario: Existing Assignments Who Match Do Nothing
	Given the group 'group-1' in tenant 'tenant-one' has license assignments
	| Product        | Disabled Features                                                                                    |
	| PROJECTPREMIUM | DYN365_CDS_PROJECT,SHAREPOINTWAC,SHAREPOINT_PROJECT,SHAREPOINTENTERPRISE,PROJECT_CLIENT_SUBSCRIPTION |
	And the group 'group-2' in tenant 'tenant-one' has license assignments
	| Product     | Disabled Features |
	| VISIOCLIENT |                   |
	When the license configuration is applied
	Then the group 'group-1' in tenant 'tenant-one' has license assignments
	| Product        | Disabled Features                                                                                    |
	| PROJECTPREMIUM | DYN365_CDS_PROJECT,SHAREPOINTWAC,SHAREPOINT_PROJECT,SHAREPOINTENTERPRISE,PROJECT_CLIENT_SUBSCRIPTION |
	And the group 'group-2' in tenant 'tenant-one' has license assignments
	| Product     | Disabled Features |
	| VISIOCLIENT |                   |

Scenario: Existing Assignments Not Matching Configured Remove Service Plans
	Given the group 'group-1' in tenant 'tenant-one' has license assignments
	| Product        | Disabled Features  |  
	| PROJECTPREMIUM | DYN365_CDS_PROJECT |
	And the group 'group-2' in tenant 'tenant-one' has license assignments
	| Product     | Disabled Features |
	| VISIOCLIENT |                   |
	When the license configuration is applied
	Then the group 'group-1' in tenant 'tenant-one' has license assignments
	| Product        | Disabled Features                                                                                    |
	| PROJECTPREMIUM | DYN365_CDS_PROJECT,SHAREPOINTWAC,SHAREPOINT_PROJECT,SHAREPOINTENTERPRISE,PROJECT_CLIENT_SUBSCRIPTION |
	And the group 'group-2' in tenant 'tenant-one' has license assignments
	| Product     | Disabled Features |
	| VISIOCLIENT |                   |

Scenario: Unrecognized Product does nothing
	Given license configuration in 'Enforce' mode for group 'group-1' in tenant 'tenant-one'
	| Product | Enabled Features                                            |
	| OTHER   | PROJECT_PROFESSIONAL,FLOW_FOR_PROJECT,EXCHANGE_S_FOUNDATION |
	When the license configuration is applied
	Then the group 'group-1' in tenant 'tenant-one' has no license assignments

Scenario: Unrecognized Service Plan Is Not Disabled
	Given license configuration in 'Enforce' mode for group 'group-1' in tenant 'tenant-one'
	| Product | Enabled Features                                            |
	| PROJECTPREMIUM   | PROJECT_PROFESSIONAL,FLOW_FOR_PROJECT,EXCHANGE_S_FOUNDATION,OTHER |
	When the license configuration is applied
	Then the group 'group-1' in tenant 'tenant-one' has license assignments
	| Product        | Disabled Features                                                                                    |
	| PROJECTPREMIUM | DYN365_CDS_PROJECT,SHAREPOINTWAC,SHAREPOINT_PROJECT,SHAREPOINTENTERPRISE,PROJECT_CLIENT_SUBSCRIPTION |

	
