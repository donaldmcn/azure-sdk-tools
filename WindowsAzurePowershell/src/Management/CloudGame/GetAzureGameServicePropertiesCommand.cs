﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.IO;
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract;

namespace Microsoft.WindowsAzure.Management.CloudGame
{
    /// <summary>
    /// Gets the resource properties.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureGameServiceProperties"), OutputType(typeof(PublisherCloudGameInfoResponse))]
    public class GetAzureGameServicePropertiesCommand : AzureCloudGameHttpClientCommandBase
    {
        public ICloudGameClient CloudGameClient { get; set; }

        public override void ExecuteCmdlet()
        {
            CloudGameClient = CloudGameClient ?? new CloudGameClient(CurrentSubscription, WriteDebug);
            PublisherCloudGameInfoResponse result = null;

            CatchAggregatedExceptionFlattenAndRethrow(() => { result = CloudGameClient.GetResourceProperties().Result; });
            WriteObject(result);
        }
    }
}