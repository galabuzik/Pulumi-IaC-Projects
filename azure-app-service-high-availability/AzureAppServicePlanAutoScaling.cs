using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Insights.Inputs;

public class AzureAppServicePlanAutoScaling
{
    public AzureAppServicePlanAutoScaling(Output<string> resourceGroupNameEastUSName, Output<string> appServicePlanIdEastUS, Output<string> resourceGroupEastUSLocation)
    {
        var autoscaleSettingResource = new Pulumi.AzureNative.Insights.AutoscaleSetting("AutoScaleBasedOnCPU", new()
        {
            ResourceGroupName = resourceGroupNameEastUSName,
            TargetResourceUri = appServicePlanIdEastUS,
            Location = resourceGroupEastUSLocation,
            Enabled = true,
            Profiles = new[]
{
        new Pulumi.AzureNative.Insights.Inputs.AutoscaleProfileArgs
        {
            Capacity = new Pulumi.AzureNative.Insights.Inputs.ScaleCapacityArgs
            {
                Default = "3",
                Maximum = "5",
                Minimum = "3",
            },
            Name = "AutoScaleBasedOnCPU",
            Rules = new[]
            {
                new Pulumi.AzureNative.Insights.Inputs.ScaleRuleArgs
                {
                    MetricTrigger = new Pulumi.AzureNative.Insights.Inputs.MetricTriggerArgs
                    {
                        MetricName = "CpuPercentage",
                        MetricResourceUri = appServicePlanIdEastUS,
                        Operator = ComparisonOperationType.GreaterThan,
                        Statistic = MetricStatisticType.Average,
                        Threshold = 50,
                        TimeAggregation = TimeAggregationType.Average,
                        TimeGrain = "PT1M",
                        TimeWindow = "PT5M",
                        DividePerInstance = false,
                    },
                    ScaleAction = new Pulumi.AzureNative.Insights.Inputs.ScaleActionArgs
                    {
                        Cooldown = "PT5M",
                        Direction = Pulumi.AzureNative.Insights.ScaleDirection.Increase,
                        Type = Pulumi.AzureNative.Insights.ScaleType.ChangeCount,
                        Value = "1",
                    },
                },
            },
        },
    },
            Notifications = new[]
{
        new Pulumi.AzureNative.Insights.Inputs.AutoscaleNotificationArgs
        {
            Operation = Pulumi.AzureNative.Insights.OperationType.Scale,
            Email = new Pulumi.AzureNative.Insights.Inputs.EmailNotificationArgs
            {
                CustomEmails = new[]
                {
                    "kelvin@cloud-nodelab.com",
                },
            },
        },
    },
        });

    }
}