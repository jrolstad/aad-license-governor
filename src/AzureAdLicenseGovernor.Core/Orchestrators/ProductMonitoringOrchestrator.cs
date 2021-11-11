using AzureAdLicenseGovernor.Core.Mappers;
using AzureAdLicenseGovernor.Core.Models;
using AzureAdLicenseGovernor.Core.Repositories;
using AzureAdLicenseGovernor.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAdLicenseGovernor.Core.Orchestrators
{
    public class ProductMonitoringOrchestrator
    {
        private readonly DirectoryOrchestrator _directoryOrchestrator;
        private readonly LicensedProductService _productService;
        private readonly ProductRepository _productRepository;
        private readonly ProductComparer _productComparer;
        private readonly LoggingService _logger;

        public ProductMonitoringOrchestrator(DirectoryOrchestrator directoryOrchestrator, 
            LicensedProductService productService,
            ProductRepository productRepository,
            ProductComparer productComparer,
            LoggingService loggingService
            )
        {
            _directoryOrchestrator = directoryOrchestrator;
            _productService = productService;
            _productRepository = productRepository;
            _productComparer = productComparer;
            _logger = loggingService;
        }

        public async Task Monitor()
        {
            var directories = await _directoryOrchestrator.Get();

            var directoryTasks = directories
                .Select(Monitor);

            await Task.WhenAll(directoryTasks);
        }

        private async Task Monitor(Directory directory)
        {
            var products = await _productService.Get(directory);

            var monitoringTasks = new List<Task>
            {
                MonitorProductChanges(directory,products),
                MonitorProductUsage(directory,products)
            };

            await Task.WhenAll(monitoringTasks);
        }

        private async Task MonitorProductChanges(Directory directory, List<Product> current)
        {
            var snapshot = await _productRepository.GetSnapshot(directory.TenantId);

            if (!snapshot.Any())
            {
                var differences = _productComparer.Compare(snapshot, current);
                LogChanges(directory.TenantId, differences);
            }

            await _productRepository.SaveSnapshot(directory.TenantId, current);

        }

        private void LogChanges(string tenantId, TenantProductComparisonResult differences)
        {
            LogSummary(tenantId, differences);
            LogAdded(tenantId, differences);
            LogRemoved(tenantId, differences);
            LogUpdated(tenantId, differences);
        }

        private void LogSummary(string tenantId, TenantProductComparisonResult differences)
        {
            var data = new Dictionary<string, string>
                {
                    {"TenantId",tenantId },
                    {"Added",differences.Added.Count.ToString() },
                    {"Removed",differences.Removed.Count.ToString() },
                    {"Updated",differences.Updated.Count.ToString() },
                };

            if (differences.Added.Any() ||
                differences.Removed.Any() ||
                differences.Updated.Any())
            {
                _logger.LogInfo(LogMessages.ProductMonitoringChangeSummary, data);
            }
            else
            {
                _logger.LogInfo(LogMessages.ProductMonitoringNoChange, data);
            }
        }

        private void LogAdded(string tenantId, TenantProductComparisonResult differences)
        {
            foreach (var item in differences.Added)
            {
                var data = GetProductAttributes(tenantId, item);
                _logger.LogInfo(LogMessages.ProductMonitoringNewProduct, data);
            }
        }

        private void LogRemoved(string tenantId, TenantProductComparisonResult differences)
        {
            foreach (var item in differences.Removed)
            {
                var data = GetProductAttributes(tenantId, item);
                _logger.LogInfo(LogMessages.ProductMonitoringRemovedProduct, data);
            }
        }

        private void LogUpdated(string tenantId, TenantProductComparisonResult differences)
        {
            foreach (var item in differences.Updated)
            {
                var data = GetProductAttributes(tenantId, item.Product);
                _logger.LogInfo(LogMessages.ProductMonitoringUpdatedProduct, data);

                LogAddedServicePlans(tenantId, item);
                LogRemovedServicePlans(tenantId, item);
            }
        }

        private void LogAddedServicePlans(string tenantId,  ProductComparisonResult differences)
        {
            foreach (var item in differences.ServicePlans.Added)
            {
                var data = GetProductAttributes(tenantId, differences.Product, item);

                _logger.LogInfo(LogMessages.ProductMonitoringNewServicePlan, data);
            }
        }

        private void LogRemovedServicePlans(string tenantId, ProductComparisonResult differences)
        {
            foreach (var item in differences.ServicePlans.Added)
            {
                var data = GetProductAttributes(tenantId, differences.Product, item);

                _logger.LogInfo(LogMessages.ProductMonitoringRemovedServicePlan, data);
            };
        }

        private Dictionary<string,string> GetProductAttributes(string tenantId, Product product, ServicePlan servicePlan=null)
        {
            var data =  new Dictionary<string, string>
                {
                     {"TenantId",tenantId },
                     {"ProductId",product?.Id },
                     {"ProductName",product?.Name },
                };

            if(servicePlan!=null)
            {
                data.Add("ServicePlanId", servicePlan.Id);
                data.Add("ServicePlanName", servicePlan.Name);
            }

            return data;
        }

        private Task MonitorProductUsage(Directory directory, List<Product> products)
        {
            foreach(var item in products)
            {
                var data = GetProductAttributes(directory.TenantId, item);
                data.Add("Units-Total", item.Units.Total.ToString());
                data.Add("Units-Assigned", item.Units.Assigned.ToString());
                data.Add("Units-Available", item.Units.Available.ToString());
                data.Add("Units-PercentUsed", GetPercentUsage(item).ToString());
                data.Add("Units-Warning", item.Units.Warning.ToString());
                data.Add("Units-Sunspended", item.Units.Suspended.ToString());

                _logger.LogInfo(LogMessages.ProductMonitoringLicenseUsage, data);
            }

            return Task.CompletedTask;
        }

        private double GetPercentUsage(Product product)
        {
            if (product.Units.Total == 0) return 1;

            return product.Units.Assigned / product.Units.Total;
        }

    }
}
