using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.ServiceFabricMesh;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public class AzureWebApps
{
    public Output<string> WebAppEndpointEastUs { get; set; }
    public Output<string> WebAppEndpointUkSouth { get; set; }
    public Output<string> ResourceGroupNameEastUS { get; set; }
    public Output<string> AppServicePlanIdEastUS { get; set; }
    public Output<string> ResourceGroupEastUSLocation { get; set; }
    public AzureWebApps()
    {
        var resourceGroupEastUS = new ResourceGroup("pulumi-rg-eastus", new ResourceGroupArgs
        {
            ResourceGroupName = "pulumi-rg-eastus",
            Location = "East US"
        });

        var resourceGroupNameEastUSName = resourceGroupEastUS.Name;
        var resourceGroupEastUSLocation = resourceGroupEastUS.Location;

        var resourceGroupUKSouth = new ResourceGroup("pulumi-rg-uksouth", new ResourceGroupArgs
        {
            ResourceGroupName = "pulumi-rg-uksouth",
            Location = "UK South"
        });

        var appServicePlanEastUS = new AppServicePlan("cln-asp-eastus", new AppServicePlanArgs
        {
            Name = "cln-asp-eastus",
            ResourceGroupName = resourceGroupEastUS.Name,
            Location = resourceGroupEastUS.Location,
            Sku = new SkuDescriptionArgs
            {
                Name = "P0v3",
                Size = "P0v3",
                Capacity = 1,
                Tier = "PremiumV3"
            },
            Reserved = true, // This is required for Linux
            ZoneRedundant = true
        });

        var appServicePlanEastUSId = appServicePlanEastUS.Id;

        var appServicePlanUKSouth = new AppServicePlan("cln-asp-uksouth", new AppServicePlanArgs
        {
            Name =  "cln-asp-uksouth",
            ResourceGroupName = resourceGroupUKSouth.Name,
            Location = resourceGroupUKSouth.Location,
            Sku = new SkuDescriptionArgs
            {
                Name = "P0v3",
                Size = "P0v3",
                Capacity = 1,
                Tier = "PremiumV3"
            },
            Reserved = true, // This is required for Linux
            ZoneRedundant = true
        });


        var webAppEastUS = new WebApp("cln-webapp-eastus", new WebAppArgs
        {
            Name = "cln-webapp-eastus",
            ResourceGroupName = resourceGroupEastUS.Name,
            Location = resourceGroupEastUS.Location,
            ServerFarmId = appServicePlanEastUS.Id,
            SiteConfig = new SiteConfigArgs
            {
                LinuxFxVersion = "DOTNETCORE|8.0"
            }
        });

        var webAppUKSouth = new WebApp("cln-webapp-uksouth", new WebAppArgs
        {
            Name =  "cln-webapp-uksouth",
            ResourceGroupName = resourceGroupUKSouth.Name,
            Location = resourceGroupUKSouth.Location,
            ServerFarmId = appServicePlanUKSouth.Id,
            SiteConfig = new SiteConfigArgs
            {
                LinuxFxVersion = "DOTNETCORE|8.0"
            }
        });

        // Outputs
        WebAppEndpointEastUs = webAppEastUS.DefaultHostName.Apply(hostName => $"https://{hostName}");
        WebAppEndpointUkSouth = webAppUKSouth.DefaultHostName.Apply(hostName => $"https://{hostName}");
        ResourceGroupNameEastUS = resourceGroupEastUS.Name;
        AppServicePlanIdEastUS = appServicePlanEastUS.Id;
        ResourceGroupEastUSLocation = resourceGroupEastUS.Location;

    }

}