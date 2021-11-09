﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace AzureAdLicenseGovernor.Worker.Tests.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class GroupLicenseGovernanceFeature : object, Xunit.IClassFixture<GroupLicenseGovernanceFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "GroupLicenseGovernance.feature"
#line hidden
        
        public GroupLicenseGovernanceFeature(GroupLicenseGovernanceFeature.FixtureData fixtureData, AzureAdLicenseGovernor_Worker_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Group License Governance", "When a group has assigned licenses defined \r\nthen it is only assigned in AAD to t" +
                    "he products and service plans defined", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line hidden
#line 7
testRunner.Given("the Azure Active Directory Tenant \'tenant-one\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "SkuId",
                        "SkuPartNumber"});
            table1.AddRow(new string[] {
                        "c5928f49-12ba-48f7-ada3-0d743a3601d5",
                        "VISIOCLIENT"});
            table1.AddRow(new string[] {
                        "09015f9f-377f-4538-bbb5-f75ceb09358a",
                        "PROJECTPREMIUM"});
#line 9
testRunner.And("licensed products in \'tenant-one\'", ((string)(null)), table1, "And ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "ServicePlanId",
                        "ServicePlanName"});
            table2.AddRow(new string[] {
                        "da792a53-cbc0-4184-a10d-e544dd34b3c1",
                        "ONEDRIVE_BASIC"});
            table2.AddRow(new string[] {
                        "2bdbaf8f-738f-4ac7-9234-3c3ee2ce7d0f",
                        "VISIOONLINE"});
            table2.AddRow(new string[] {
                        "113feb6c-3fe4-4440-bddc-54d774bf0318",
                        "EXCHANGE_S_FOUNDATION"});
            table2.AddRow(new string[] {
                        "663a804f-1c30-4ff0-9915-9db84f0d1cea",
                        "VISIO_CLIENT_SUBSCRIPTION"});
#line 13
testRunner.And("service plans in \'tenant-one\' for product \'VISIOCLIENT\'", ((string)(null)), table2, "And ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "ServicePlanId",
                        "ServicePlanName"});
            table3.AddRow(new string[] {
                        "818523f5-016b-4355-9be8-ed6944946ea7",
                        "PROJECT_PROFESSIONAL"});
            table3.AddRow(new string[] {
                        "fa200448-008c-4acb-abd4-ea106ed2199d",
                        "FLOW_FOR_PROJECT"});
            table3.AddRow(new string[] {
                        "50554c47-71d9-49fd-bc54-42a2765c555c",
                        "DYN365_CDS_PROJECT"});
            table3.AddRow(new string[] {
                        "113feb6c-3fe4-4440-bddc-54d774bf0318",
                        "EXCHANGE_S_FOUNDATION"});
            table3.AddRow(new string[] {
                        "e95bec33-7c88-4a70-8e19-b10bd9d0c014",
                        "SHAREPOINTWAC"});
            table3.AddRow(new string[] {
                        "fe71d6c3-a2ea-4499-9778-da042bf08063",
                        "SHAREPOINT_PROJECT"});
            table3.AddRow(new string[] {
                        "5dbe027f-2339-4123-9542-606e4d348a72",
                        "SHAREPOINTENTERPRISE"});
            table3.AddRow(new string[] {
                        "fafd7243-e5c1-4a3a-9e40-495efcb1d3c3",
                        "PROJECT_CLIENT_SUBSCRIPTION"});
#line 19
testRunner.And("service plans in \'tenant-one\' for product \'PROJECTPREMIUM\'", ((string)(null)), table3, "And ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "DisplayName"});
            table4.AddRow(new string[] {
                        "group-1"});
            table4.AddRow(new string[] {
                        "group-2"});
            table4.AddRow(new string[] {
                        "group-3"});
#line 30
testRunner.And("groups in tenant \'tenant-one\'", ((string)(null)), table4, "And ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Product",
                        "Enabled Features"});
            table5.AddRow(new string[] {
                        "PROJECTPREMIUM",
                        "PROJECT_PROFESSIONAL,FLOW_FOR_PROJECT,EXCHANGE_S_FOUNDATION"});
#line 36
testRunner.And("license configuration in \'Enforce\' mode for group \'group-1\' in tenant \'tenant-one" +
                    "\'", ((string)(null)), table5, "And ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Product",
                        "Enabled Features"});
            table6.AddRow(new string[] {
                        "VISIOCLIENT",
                        "ONEDRIVE_BASIC,VISIOONLINE,EXCHANGE_S_FOUNDATION,VISIO_CLIENT_SUBSCRIPTION"});
#line 39
testRunner.And("license configuration in \'Enforce\' mode for group \'group-2\' in tenant \'tenant-one" +
                    "\'", ((string)(null)), table6, "And ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Happy Path")]
        [Xunit.TraitAttribute("FeatureTitle", "Group License Governance")]
        [Xunit.TraitAttribute("Description", "Happy Path")]
        public virtual void HappyPath()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Happy Path", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 43
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
#line 44
 testRunner.When("the license configuration is applied", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                            "Product",
                            "Disabled Features"});
                table7.AddRow(new string[] {
                            "PROJECTPREMIUM",
                            "DYN365_CDS_PROJECT,SHAREPOINTWAC,SHAREPOINT_PROJECT,SHAREPOINTENTERPRISE,PROJECT_" +
                                "CLIENT_SUBSCRIPTION"});
#line 45
 testRunner.Then("the group \'group-1\' in tenant \'tenant-one\' has license assignments", ((string)(null)), table7, "Then ");
#line hidden
                TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                            "Product",
                            "Disabled Features"});
                table8.AddRow(new string[] {
                            "VISIOCLIENT",
                            ""});
#line 48
 testRunner.And("the group \'group-2\' in tenant \'tenant-one\' has license assignments", ((string)(null)), table8, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                GroupLicenseGovernanceFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                GroupLicenseGovernanceFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
