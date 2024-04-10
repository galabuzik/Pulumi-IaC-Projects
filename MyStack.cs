using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

class MyStack : Stack
{
    public MyStack()
    {
        // Create an Azure Resource Group
        var resourceGroup = new ResourceGroup("resourceGroup", new ResourceGroupArgs
        {
            ResourceGroupName = "pulumi-rg-01"
        });

        // Create an Azure App Service Plan 
        var appServicePlan = new AppServicePlan("appServicePlan", new AppServicePlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Sku = new SkuDescriptionArgs
            {
                Name = "B1", // Basic tier, adjust as needed
                Tier = "Basic",
                Size = "B1",
                Family = "B",
                Capacity = 1
            },
            Kind = "App",
        });

        // Create an Azure App Service
        var appService = new WebApp("webApp", new WebAppArgs
        {
            ResourceGroupName = resourceGroup.Name,
            ServerFarmId = appServicePlan.Id,
            SiteConfig = new SiteConfigArgs
            {
                AppSettings = new[]
                {
                    new NameValuePairArgs
                    {
                        Name = "WEBSITE_RUN_FROM_PACKAGE",
                        Value = "1"
                    }
                },
                NetFrameworkVersion = "v6.0" // Specify .NET 6 runtime
            }
        });

        // Output the App Service URL
        this.Endpoint = Output.Format($"https://{appService.DefaultHostName}/");
    }

    [Output]
    public Output<string> Endpoint { get; set; }
}
