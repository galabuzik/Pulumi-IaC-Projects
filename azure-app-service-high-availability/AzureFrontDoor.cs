﻿using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.AzureNative.Cdn.Inputs;
using Pulumi.AzureNative.Resources;
using AzureNative = Pulumi.AzureNative;

public class AzureFrontDoor
{
    public AzureFrontDoor(Output<string> webAppEastUsEndpoint, Output<string> webAppUkSouthEndpoint)
    {
        var resourceGroupEastUS = new ResourceGroup("pulumi-rg-frontdoor", new ResourceGroupArgs
        {
            ResourceGroupName = "pulumi-rg-frontdoor",
            Location = "East US"
        });

        var afdProfile01 = new AzureNative.Cdn.Profile("afdProfile01", new()
        {
            ResourceGroupName = resourceGroupEastUS.Name,
            Sku = new AzureNative.Cdn.Inputs.SkuArgs
            {
                Name = "Standard_AzureFrontDoor",
            },
            Location = "global",
            OriginResponseTimeoutSeconds = 30,
            ProfileName = "cln-frontdoor",
        });
        var afdEndpoint01 = new AzureNative.Cdn.AFDEndpoint("afdEndpoint01", new()
        {
            ProfileName = afdProfile01.Name,
            ResourceGroupName = resourceGroupEastUS.Name,
            AutoGeneratedDomainNameLabelScope = "TenantReuse",
            EnabledState = "Enabled",
            EndpointName = "endpoint01",
            Location = "global",
        });

        var afdOriginGroup01 = new AzureNative.Cdn.AFDOriginGroup("afdOriginGroup01", new()
        {
            HealthProbeSettings = new AzureNative.Cdn.Inputs.HealthProbeParametersArgs
            {
                ProbeIntervalInSeconds = 10,
                ProbePath = "/",
                ProbeProtocol = AzureNative.Cdn.ProbeProtocol.Http,
                ProbeRequestType = AzureNative.Cdn.HealthProbeRequestType.HEAD,
            },
            LoadBalancingSettings = new AzureNative.Cdn.Inputs.LoadBalancingSettingsParametersArgs
            {
                AdditionalLatencyInMilliseconds = 1000,
                SampleSize = 3,
                SuccessfulSamplesRequired = 3,
            },
            OriginGroupName = "originGroup01",
            ProfileName = afdProfile01.Name,
            ResourceGroupName = resourceGroupEastUS.Name,
            TrafficRestorationTimeToHealedOrNewEndpointsInMinutes = 2,
        });

        var originGroup01 = afdOriginGroup01.Id;

        var afdOrigin01 = new AzureNative.Cdn.AFDOrigin("afdOrigin01", new()
        {
            EnabledState = AzureNative.Cdn.EnabledState.Enabled,
            HostName = "cln-webapp-eastus.azurewebsites.net",
            HttpPort = 80,
            HttpsPort = 443,
            Priority = 1,
            Weight = 1,
            OriginGroupName = afdOriginGroup01.Name,
            OriginHostHeader = "cln-webapp-eastus.azurewebsites.net",
            OriginName = "origin01",
            ProfileName = afdProfile01.Name,
            ResourceGroupName = resourceGroupEastUS.Name,
        });

        var afdOrigin02 = new AzureNative.Cdn.AFDOrigin("afdOrigin02", new()
        {
            EnabledState = AzureNative.Cdn.EnabledState.Enabled,
            HostName = "cln-webapp-uksouth.azurewebsites.net",
            HttpPort = 80,
            HttpsPort = 443,
            Priority = 1,
            Weight = 1,
            OriginGroupName = afdOriginGroup01.Name,
            OriginHostHeader = "cln-webapp-uksouth.azurewebsites.net",
            OriginName = "origin02",
            ProfileName = afdProfile01.Name,
            ResourceGroupName = resourceGroupEastUS.Name,
        });

        var afdCustomDomain01 = new AzureNative.Cdn.AFDCustomDomain("afdCustomDomain01", new()
        {
            CustomDomainName = "azfd01",
            HostName = "azfd01.cloud-nodelab.com",
            ProfileName = afdProfile01.Name,
            ResourceGroupName = resourceGroupEastUS.Name,
            TlsSettings = new AzureNative.Cdn.Inputs.AFDDomainHttpsParametersArgs
            {
                CertificateType = AzureNative.Cdn.AfdCertificateType.ManagedCertificate,
                MinimumTlsVersion = AzureNative.Cdn.AfdMinimumTlsVersion.TLS12,
            },
        });

        var customDomain01 = afdCustomDomain01.Id;

        var route01 = new AzureNative.Cdn.Route("route01", new()
        {

            CustomDomains = new[]
        {
            new AzureNative.Cdn.Inputs.ActivatedResourceReferenceArgs
            {
                Id = customDomain01,
            },
        },

            EnabledState = AzureNative.Cdn.EnabledState.Enabled,
            EndpointName = afdEndpoint01.Name,
            ForwardingProtocol = AzureNative.Cdn.ForwardingProtocol.MatchRequest,
            HttpsRedirect = AzureNative.Cdn.HttpsRedirect.Enabled,
            LinkToDefaultDomain = AzureNative.Cdn.LinkToDefaultDomain.Enabled,
            OriginGroup = new AzureNative.Cdn.Inputs.ResourceReferenceArgs
            {
                Id = originGroup01,
            },
            PatternsToMatch = new[]
        {
            "/*",
        },
            ProfileName = afdProfile01.Name,
            ResourceGroupName = resourceGroupEastUS.Name,
            RouteName = "route1",

        });

    }
}