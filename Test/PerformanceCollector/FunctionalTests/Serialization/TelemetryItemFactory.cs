﻿namespace Functional.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Json;

    using FunctionalTests.Helpers;

    using Microsoft.Developer.Analytics.DataCollection.Model.v2;
    using Microsoft.ManagementServices.RealTimeDataProcessing.QuickPulseService;

    using Newtonsoft.Json;
    using TelemetryItem = Microsoft.Developer.Analytics.DataCollection.Model.v2.TelemetryItem;

    internal static class TelemetryItemFactory
    {
        public static IList<TelemetryItem> GetTelemetryItems(string content)
        {
            var items = new List<TelemetryItem>();

            if (string.IsNullOrWhiteSpace(content))
            {
                return items;
            }

            var newLines = new [] { "\r\n", "\n" };

            string[] lines = content.Split(newLines, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                var envelope = JsonConvert.DeserializeObject<Envelope>(line);
                var item = CreateTelemetryItem(envelope, line);
                items.Add(item);
            }

            return items;
        }

        public static MonitoringDataPoint[] CreateQuickPulseSamples(Stream content)
        {
            var serializerDataPointArray = new DataContractJsonSerializer(typeof(MonitoringDataPoint[]));

            return serializerDataPointArray.ReadObject(content) as MonitoringDataPoint[];
        }

        private static TelemetryItem CreateTelemetryItem(
            Envelope envelope, 
            string content)
        {
            TelemetryItem result;

            switch (envelope.Data.BaseType)
            {
                case TelemetryItem.ExceptionName:
                {
                    result = JsonConvert.DeserializeObject<TelemetryItem<ExceptionData>>(content);
                    break;
                }

                case TelemetryItem.RequestName:
                {
                    result = JsonConvert.DeserializeObject<TelemetryItem<RequestData>>(content);
                    break;
                }

                case TelemetryItem.MetricName:
                {
                    result = JsonConvert.DeserializeObject<TelemetryItem<MetricData>>(content);
                    break;
                }

                case TelemetryItem.RemoteDependencyName:
                {
                    result = JsonConvert.DeserializeObject<TelemetryItem<RemoteDependencyData>>(content);
                    break;
                }

                case TelemetryItem.MessageName:
                {
                    result = JsonConvert.DeserializeObject<TelemetryItem<MessageData>>(content);
                    break;
                }

                case TelemetryItem.SessionStateName:
                {
                    result = JsonConvert.DeserializeObject<TelemetryItem<SessionStateData>>(content);
                    break;
                }

                case TelemetryItem.PerformanceCounterName:
                {
                    result = JsonConvert.DeserializeObject<TelemetryItem<PerformanceCounterData>>(content);
                    break;
                }

                default:
                {
                    throw new InvalidDataException("Unsupported telemetry type");
                }
            }

            return result;
        }
    }
}

