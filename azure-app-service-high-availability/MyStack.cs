using Pulumi;

class MyStack : Stack
{
    public MyStack()
    {
        var AzureWebApps = new AzureWebApps();
        var AzureFrontDoor = new AzureFrontDoor(AzureWebApps.WebAppEndpointEastUs, AzureWebApps.WebAppEndpointUkSouth);
        var AzureAppServicePlanAutoScaling = new AzureAppServicePlanAutoScaling(
            AzureWebApps.ResourceGroupNameEastUS,
            AzureWebApps.AppServicePlanIdEastUS,
            AzureWebApps.ResourceGroupEastUSLocation
        );
    }
}
