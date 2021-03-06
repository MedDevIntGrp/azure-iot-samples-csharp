﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client.Exceptions;

namespace Microsoft.Azure.Devices.Client.Samples
{
    public class KeyRolloverSample
    {
        private readonly string _connectionString1;
        private readonly string _connectionString2;
        private readonly TransportType _transportType;

        public KeyRolloverSample(string connectionString1, string connectionString2, TransportType transportType)
        {
            _connectionString1 = connectionString1;
            _connectionString2 = connectionString2;
            _transportType = transportType;
        }

        public async Task RunSampleAsync()
        {
            Console.WriteLine("Update device's first connection string (using the ServiceClient SDK or DeviceExplorer) while this sample is running.");

            try
            {
                await RunSampleWithConnectionStringAsync(_connectionString1).ConfigureAwait(false);
            }
            catch (UnauthorizedException ex)
            {
                Console.WriteLine($"UnauthorizedExpception:\n{ex.Message}");
                Console.WriteLine("Assuming key roll-over. ConnectionString1 should be reconfigured on this device.");

                await RunSampleWithConnectionStringAsync(_connectionString2).ConfigureAwait(false);
            }
        }

        private async Task RunSampleWithConnectionStringAsync(string connectionString)
        {
            string usedConnectionString = connectionString == _connectionString1 ? "PRIMARY" : "SECONDARY";
            int delaySeconds = 5;
            int loopSeconds = 30;
            Console.WriteLine($"Sending one message every {delaySeconds} seconds");

            using (var deviceClient = DeviceClient.CreateFromConnectionString(connectionString, _transportType))
            {
                using (var testMessage = new Message(Encoding.UTF8.GetBytes("message from key rollover sample")))
                {
                    for (int i = 0; i * delaySeconds < loopSeconds; i++)
                    {
                        Console.WriteLine($"\t {DateTime.Now.ToLocalTime()} Sending message [{usedConnectionString} connection string].");
                        await deviceClient.SendEventAsync(testMessage).ConfigureAwait(false);
                        await Task.Delay(delaySeconds * 1000);
                    }
                }
            }
        }
    }
}
