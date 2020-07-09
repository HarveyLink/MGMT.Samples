// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using Samples.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreateVirtualMachinesInParallel
{
    public class Program
    {
        ///**
        // * Azure compute sample for creating multiple virtual machines in parallel.
        // *  - Define 1 virtual network per region
        // *  - Define 1 storage account per region
        // *  - Create 5 virtual machines in 2 regions using defined virtual network and storage account
        // *  - Create a traffic manager to route traffic across the virtual machines
        // */
        //private const string Username = "tirekicker";
        //private const string Password = "12NewPA$$w0rd!";
        //private static readonly string rgName = Utilities.RandomResourceName("rgCOMV", 10);
        //private static readonly string SubscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID");

        //public static async Task RunSample(TokenCredential credential)
        //{

        //    IDictionary<string, int> virtualMachinesByLocation = new Dictionary<string, int>();

        //    virtualMachinesByLocation.Add("eastus", 5);
        //    virtualMachinesByLocation.Add("southcentralus", 5);

        //    var networkManagementClient = new NetworkManagementClient(SubscriptionId, credential);
        //    var networkSecurityGroups = networkManagementClient.NetworkSecurityGroups;
        //    var virtualNetworks = networkManagementClient.VirtualNetworks;
        //    var publicIPAddresses = networkManagementClient.PublicIPAddresses;
        //    var networkInterfaces = networkManagementClient.NetworkInterfaces;
        //    var computeManagementClient = new ComputeManagementClient(SubscriptionId, credential);
        //    var virtualMachines = computeManagementClient.VirtualMachines;
        //    var storageManagementClient = new StorageManagementClient(SubscriptionId, credential);
        //    var storageAccounts = storageManagementClient.StorageAccounts;

        //    try
        //    {
        //        //=============================================================
        //        // Create a resource group (Where all resources gets created)
        //        //
        //        await ResourceGroupHelper.CreateOrUpdateResourceGroup(rgName, "westus");

        //        var publicIpCreatableKeys = new List<string>();
        //        // Prepare a batch of Creatable definitions
        //        //
        //        var creatableVirtualMachines = new List<VirtualMachine>();

        //        foreach (var entry in virtualMachinesByLocation)
        //        {
        //            var region = entry.Key;
        //            var vmCount = entry.Value;

        //            //=============================================================
        //            // Create 1 network creatable per region
        //            // Prepare Creatable Network definition (Where all the virtual machines get added to)
        //            //
        //            var networkName = Utilities.RandomResourceName("vnetCOPD-", 20);
        //            var virtualNetworkParameters = new VirtualNetwork
        //            {
        //                Location = region,
        //                AddressSpace = new AddressSpace { AddressPrefixes = new List<string> { "172.16.0.0/16" } },
        //            };
        //            var networkCreatable = (await (await virtualNetworks
        //                .StartCreateOrUpdateAsync(rgName, networkName, virtualNetworkParameters)).WaitForCompletionAsync()).Value;

        //            //=============================================================
        //            // Create 1 storage creatable per region (For storing VMs disk)
        //            //
        //            var storageAccountName = Utilities.RandomResourceName("stgcopd", 20);
        //            var storageAccountParameters = new StorageAccountCreateParameters(new Sku(SkuName.StandardGRS), Kind.Storage, region);
        //            var storageAccountCreatable = (await (await storageAccounts
        //                .StartCreateAsync(rgName, storageAccountName, storageAccountParameters)).WaitForCompletionAsync()).Value;
        //            string containerName = Utilities.RandomResourceName("cndisk", 20);
        //            var vhdContainer = "https://" + storageAccountName + ".blob.core.windows.net/" + containerName;
        //            var vhduri = vhdContainer + string.Format("/{0}.vhd", Utilities.RandomResourceName("vhduri", 15));
        //            var osVhduri = vhdContainer + string.Format("/os{0}.vhd", Utilities.RandomResourceName("osvhduri", 15));

        //            var linuxVMNamePrefix = Utilities.RandomResourceName("vm-", 15);
        //            for (int i = 1; i <= vmCount; i++)
        //            {
        //                //=============================================================
        //                // Create 1 public IP address creatable
        //                //
        //                var ipAddress = new PublicIPAddress
        //                {
        //                    PublicIPAddressVersion = Azure.ResourceManager.Network.Models.IPVersion.IPv4,
        //                    PublicIPAllocationMethod = IPAllocationMethod.Dynamic,
        //                    Location = region,
        //                    DnsSettings = new PublicIPAddressDnsSettings
        //                    {
        //                        DomainNameLabel = $"{linuxVMNamePrefix}-{i}"
        //                    }
        //                };

        //                ipAddress = (await publicIPAddresses.StartCreateOrUpdate(rgName, $"{linuxVMNamePrefix}-{i}", ipAddress)
        //                    .WaitForCompletionAsync()).Value;

        //                publicIpCreatableKeys.Add(ipAddress.IpAddress);

        //                // Create VNet
        //                var vnet = new VirtualNetwork
        //                {
        //                    Location = region,
        //                    AddressSpace = new AddressSpace { AddressPrefixes = new List<string>() { "10.0.0.0/16" } },
        //                    Subnets = new List<Subnet>
        //                    {
        //                        new Subnet
        //                        {
        //                            Name = "mySubnet",
        //                            AddressPrefix = "10.0.0.0/24",
        //                        }
        //                    },
        //                };
        //                vnet = await virtualNetworks.StartCreateOrUpdate(rgName, $"{linuxVMNamePrefix}-{i}", vnet).WaitForCompletionAsync();

        //                // Create Network Interface
        //                Console.WriteLine("--------Start create Network Interface--------");
        //                var nic = new NetworkInterface
        //                {
        //                    Location = region,
        //                    IpConfigurations = new List<NetworkInterfaceIPConfiguration>
        //                    {
        //                        new NetworkInterfaceIPConfiguration
        //                        {
        //                            Name = "Primary",
        //                            Primary = true,
        //                            Subnet = new Subnet { Id = vnet.Subnets.First().Id },
        //                            PrivateIPAllocationMethod = IPAllocationMethod.Dynamic,
        //                            PublicIPAddress = new PublicIPAddress { Id = ipAddress.Id }
        //                        }
        //                    }
        //                };
        //                nic = await networkInterfaces.StartCreateOrUpdate(rgName, $"{linuxVMNamePrefix}-{i}", nic).WaitForCompletionAsync();

        //                //=============================================================
        //                // Create 1 virtual machine creatable
        //                var virtualMachineCreatable = new VirtualMachine("eastus")
        //                {
        //                    NetworkProfile = new Azure.ResourceManager.Compute.Models.NetworkProfile
        //                    {
        //                        NetworkInterfaces = new[]
        //                       {
        //                           new NetworkInterfaceReference { Id = nic.Id }
        //                       }
        //                    },
        //                    OsProfile = new OSProfile
        //                    {
        //                        ComputerName = $"{linuxVMNamePrefix}-{i}",
        //                        AdminUsername = Username,
        //                        AdminPassword = Password,
        //                        LinuxConfiguration = new LinuxConfiguration
        //                        {
        //                            DisablePasswordAuthentication = false,
        //                            ProvisionVMAgent = true
        //                        }
        //                    },
        //                    StorageProfile = new StorageProfile
        //                    {
        //                        ImageReference = new ImageReference
        //                        {
        //                            Offer = "UbuntuServer",
        //                            Publisher = "Canonical",
        //                            Sku = "16.04-LTS",
        //                            Version = "latest"
        //                        },
        //                        OsDisk = new OSDisk(DiskCreateOptionTypes.FromImage)
        //                        {
        //                            Caching = CachingTypes.None,
        //                            WriteAcceleratorEnabled = true,
        //                            Name = "test",
        //                            Vhd = new VirtualHardDisk { Uri = osVhduri }
        //                        },
        //                        DataDisks = new List<DataDisk>()
        //                    },
        //                    HardwareProfile = new HardwareProfile
        //                    {
        //                        VmSize = VirtualMachineSizeTypes.StandardD3V2
        //                    }
        //                };
        //                creatableVirtualMachines.Add(virtualMachineCreatable);
        //            }
        //        }

        //        //=============================================================
        //        // Create !!

        //        var t1 = DateTimeOffset.Now.UtcDateTime;
        //        Utilities.Log("Creating the virtual machines");

        //        Parallel.ForEach(creatableVirtualMachines, async item =>
        //        {
        //            var result = await (await virtualMachines
        //            .StartCreateOrUpdateAsync(rgName, item.Name, item)).WaitForCompletionAsync();
        //            Utilities.Log("VM created for: " + result.Value.Id);
        //        });

        //        var t2 = DateTimeOffset.Now.UtcDateTime;
        //        Utilities.Log("Created virtual machines");

        //        Utilities.Log($"Virtual machines create: took {(t2 - t1).TotalSeconds } seconds to create == " + creatableVirtualMachines.Count + " == virtual machines");

        //        var publicIpResourceIds = new List<string>();
        //        foreach (string publicIpCreatableKey in publicIpCreatableKeys)
        //        {
        //            var pip = (IPublicIPAddress)virtualMachines.CreatedRelatedResource(publicIpCreatableKey);
        //            publicIpResourceIds.Add(pip.Id);
        //        }

        //        //=============================================================
        //        // Create 1 Traffic Manager Profile
        //        //
        //        var trafficManagerName = SdkContext.RandomResourceName("tra", 15);
        //        var profileWithEndpoint = azure.TrafficManagerProfiles.Define(trafficManagerName)
        //                .WithExistingResourceGroup(resourceGroup)
        //                .WithLeafDomainLabel(trafficManagerName)
        //                .WithPerformanceBasedRouting();

        //        int endpointPriority = 1;
        //        Microsoft.Azure.Management.TrafficManager.Fluent.TrafficManagerProfile.Definition.IWithCreate profileWithCreate = null;
        //        foreach (var publicIpResourceId in publicIpResourceIds)
        //        {
        //            var endpointName = $"azendpoint-{endpointPriority}";
        //            if (endpointPriority == 1)
        //            {
        //                profileWithCreate = profileWithEndpoint.DefineAzureTargetEndpoint(endpointName)
        //                        .ToResourceId(publicIpResourceId)
        //                        .WithRoutingPriority(endpointPriority)
        //                        .Attach();
        //            }
        //            else
        //            {
        //                profileWithCreate = profileWithCreate.DefineAzureTargetEndpoint(endpointName)
        //                        .ToResourceId(publicIpResourceId)
        //                        .WithRoutingPriority(endpointPriority)
        //                        .Attach();
        //            }
        //            endpointPriority++;
        //        }

        //        var trafficManagerProfile = profileWithCreate.Create();
        //        Utilities.Log("Created a traffic manager profile - " + trafficManagerProfile.Id);


        //    }
        //    finally
        //    {

        //    }
        //}

        //public static TResult Synchronize<TResult>(Func<Task<TResult>> function)
        //{
        //    return Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
        //        .Unwrap().GetAwaiter().GetResult();
        //}

        //public static async Task Main(string[] args)
        //{
        //    try
        //    {
        //        //=================================================================
        //        // Authenticate
        //        var credentials = new DefaultAzureCredential();

        //        await RunSample(credentials);
        //    }
        //    catch (Exception ex)
        //    {
        //        Utilities.Log(ex);
        //    }
        //}
    }
}
