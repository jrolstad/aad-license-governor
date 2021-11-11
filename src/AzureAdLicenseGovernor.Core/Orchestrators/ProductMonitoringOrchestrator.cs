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

        private Task Monitor(Directory directory)
        {
            var monitoringTasks = new List<Task>
            {
                MonitorProductChanges(directory),
            };

            return Task.WhenAll(monitoringTasks);
        }

        private async Task MonitorProductChanges(Directory directory)
        {
            var snapshot = await _productRepository.GetSnapshot(directory.TenantId);
            var current = await _productService.Get(directory);

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
                _logger.LogInfo("Product Governance|Change Summary", data);
            }
            else
            {
                _logger.LogInfo("Product Governance|Change Summary:None", data);
            }
        }

        private void LogAdded(string tenantId, TenantProductComparisonResult differences)
        {
            foreach (var item in differences.Added)
            {
                var data = GetProductAttributes(tenantId, item);
                _logger.LogInfo("Product Governance|New Product", data);
            }
        }

        private void LogRemoved(string tenantId, TenantProductComparisonResult differences)
        {
            foreach (var item in differences.Removed)
            {
                var data = GetProductAttributes(tenantId, item);
                _logger.LogInfo("Product Governance|Removed Product", data);
            }
        }

        private void LogUpdated(string tenantId, TenantProductComparisonResult differences)
        {
            foreach (var item in differences.Updated)
            {
                var data = GetProductAttributes(tenantId, item.Product);
                _logger.LogInfo("Product Governance|Update Product", data);

                LogAdded(tenantId, item);
                LogRemoved(tenantId, item);
            }
        }

        private void LogAdded(string tenantId,  ProductComparisonResult differences)
        {
            foreach (var item in differences.ServicePlans.Added)
            {
                var data = GetProductAttributes(tenantId, differences.Product);
                data.Add("ServicePlanId", item.Id);
                data.Add("ServicePlanName", item.Name);

                _logger.LogInfo("Product Governance|New Service Plan", data);
            }
        }

        private void LogRemoved(string tenantId, ProductComparisonResult differences)
        {
            foreach (var item in differences.ServicePlans.Added)
            {
                var data = GetProductAttributes(tenantId, differences.Product);
                data.Add("ServicePlanId", item.Id);
                data.Add("ServicePlanName", item.Name);

                _logger.LogInfo("Product Governance|Removed Service Plan", data);
            };
        }

        private Dictionary<string,string> GetProductAttributes(string tenantId, Product product)
        {
            return new Dictionary<string, string>
                {
                     {"TenantId",tenantId },
                     {"ProductId",product.Id },
                     {"ProductName",product.Name },
                };
        }
    }
}
