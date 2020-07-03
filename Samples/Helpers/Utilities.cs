// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure.ResourceManager.EventHubs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Helpers
{
    public static class Utilities
    {
        public static bool IsRunningMocked { get; set; }
        public static Action<string> LoggerMethod { get; set; }
        public static Func<string> PauseMethod { get; set; }

        public static string ProjectPath { get; set; }

        static Utilities()
        {
            LoggerMethod = Console.WriteLine;
            PauseMethod = Console.ReadLine;
            ProjectPath = ".";
        }
        public static void Log(string message)
        {
            LoggerMethod.Invoke(message);
        }

        public static void Log(object obj)
        {
            if (obj != null)
            {
                LoggerMethod.Invoke(obj.ToString());
            }
            else
            {
                LoggerMethod.Invoke("(null)");
            }
        }

        public static void Log()
        {
            Utilities.Log("");
        }

        public static string GetArmTemplate(string templateFileName)
        {
            //var adminUsername = "tirekicker";
            //var adminPassword = "12NewPA$$w0rd!";
            var hostingPlanName = RandomResourceName("hpRSAT", 24);
            var webAppName = RandomResourceName("wnRSAT", 24);
            var armTemplateString = File.ReadAllText(Path.Combine(Utilities.ProjectPath, "Asset", templateFileName));

            if (String.Equals("ArmTemplate.json", templateFileName, StringComparison.OrdinalIgnoreCase))
            {
                var index = armTemplateString.IndexOf("\"hostingPlanName\": {\r\n      \"type\": \"string\",\r\n      \"defaultValue\": \"\"");
                armTemplateString = armTemplateString.Replace("\"hostingPlanName\": {\r\n      \"type\": \"string\",\r\n      \"defaultValue\": \"\"",
                   "\"hostingPlanName\": {\r\n      \"type\": \"string\",\r\n      \"defaultValue\": \"" + hostingPlanName + "\"");
                armTemplateString = armTemplateString.Replace("\"webSiteName\": {\r\n      \"type\": \"string\",\r\n      \"defaultValue\": \"\"",
                    "\"webSiteName\": {\r\n      \"type\": \"string\",\r\n      \"defaultValue\": \"" + webAppName + "\"");
            }
            else if (String.Equals("ArmTemplateVM.json", templateFileName, StringComparison.OrdinalIgnoreCase))
            { 
            
            }
            return armTemplateString;
        }


        public static void Print(EHNamespace resource)
        {
            StringBuilder eh = new StringBuilder("Eventhub Namespace: ")
                .Append("Eventhub Namespace: ").Append(resource.Id)
                    .Append("\n\tName: ").Append(resource.Name)
                    .Append("\n\tLocation: ").Append(resource.Location)
                    .Append("\n\tTags: ").Append(resource.Tags.ToString())
                    .Append("\n\tAzureInsightMetricId: ").Append(resource.MetricId)
                    .Append("\n\tIsAutoInflate enabled: ").Append(resource.IsAutoInflateEnabled)
                    .Append("\n\tServiceBus endpoint: ").Append(resource.ServiceBusEndpoint)
                    .Append("\n\tMaximum Throughput Units: ").Append(resource.MaximumThroughputUnits)
                    .Append("\n\tCreated time: ").Append(resource.CreatedAt)
                    .Append("\n\tUpdated time: ").Append(resource.UpdatedAt);
            Utilities.Log(eh.ToString());
        }

        public static void Print(Eventhub resource)
        {
            StringBuilder info = new StringBuilder();
            info.Append("Eventhub: ").Append(resource.Id)
                    .Append("\n\tName: ").Append(resource.Name)
                    .Append("\n\tMessage retention in Days: ").Append(resource.MessageRetentionInDays)
                    .Append("\n\tPartition ids: ").Append(resource.PartitionIds);
            if (resource.CaptureDescription != null)
            {
                info.Append("\n\t\t\tSize limit in Bytes: ").Append(resource.CaptureDescription.SizeLimitInBytes);
                info.Append("\n\t\t\tInterval in seconds: ").Append(resource.CaptureDescription.IntervalInSeconds);
                if (resource.CaptureDescription.Destination != null)
                {
                    info.Append("\n\t\t\tData capture storage account: ").Append(resource.CaptureDescription.Destination.StorageAccountResourceId);
                    info.Append("\n\t\t\tData capture storage container: ").Append(resource.CaptureDescription.Destination.BlobContainer);
                }
            }
            Utilities.Log(info.ToString());
        }

        public static void Print(ConsumerGroup resource)
        {
            StringBuilder info = new StringBuilder();
            info.Append("Event hub consumer group: ").Append(resource.Id)
                    .Append("\n\tName: ").Append(resource.Name)
                    .Append("\n\tUser metadata: ").Append(resource.UserMetadata);
            Utilities.Log(info.ToString());
        }

        public static void Print(ArmDisasterRecovery resource)
        {
            StringBuilder info = new StringBuilder();
            info.Append("DisasterRecoveryPairing: ").Append(resource.Id)
                    .Append("\n\tName: ").Append(resource.Name)
                    .Append("\n\tAlternate name: ").Append(resource.AlternateName)
                    .Append("\n\tPartner namespace: ").Append(resource.PartnerNamespace)
                    .Append("\n\tNamespace role: ").Append(resource.Role);
            Utilities.Log(info.ToString());
        }

        public static void Print(AccessKeys resource)
        {
            StringBuilder info = new StringBuilder();
            info.Append("DisasterRecoveryPairing auth key: ")
                    .Append("\n\t Alias primary connection string: ").Append(resource.AliasPrimaryConnectionString)
                    .Append("\n\t Alias secondary connection string: ").Append(resource.AliasSecondaryConnectionString)
                    .Append("\n\t Primary key: ").Append(resource.PrimaryKey)
                    .Append("\n\t Secondary key: ").Append(resource.SecondaryKey)
                    .Append("\n\t Primary connection string: ").Append(resource.PrimaryConnectionString)
                    .Append("\n\t Secondary connection string: ").Append(resource.SecondaryConnectionString);
            Utilities.Log(info.ToString());
        }

        /// <summary>
        /// Generates the specified number of random resource names with the same prefix.
        /// </summary>
        /// <param name="prefix">the prefix to be used if possible</param>
        /// <param name="maxLen">the maximum length for the random generated name</param>
        /// <param name="count">the number of names to generate</param>
        /// <returns>random names</returns>
        public static string[] RandomResourceNames(string prefix, int maxLen, int count)
        {
            string[] names = new string[count];
            var resourceNamer = new ResourceNamer("");
            for (int i = 0; i < count; i++)
            {
                names[i] = resourceNamer.RandomName(prefix, maxLen);
            }
            return names;
        }

        public static string RandomResourceName(string prefix, int maxLen)
        {
            var namer = new ResourceNamer("");
            return namer.RandomName(prefix, maxLen);
        }
        public static string RandomGuid()
        {
            var namer = new ResourceNamer("");
            return namer.RandomGuid();
        }

        public static async Task<List<T>> ToEnumerableAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            List<T> list = new List<T>();
            await foreach (T item in asyncEnumerable)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
