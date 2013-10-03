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

namespace Microsoft.WindowsAzure.Management.XblCompute
{
    using Microsoft.WindowsAzure.Management.Utilities.XblCompute;
    using Microsoft.WindowsAzure.Management.Utilities.XblCompute.Contract;
    using System.Management.Automation;

    /// <summary>
    /// Get the cloud game package.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureGameServicesXblPackages"), OutputType(typeof(XblPackageCollectionResponse))]
    public class GetAzureGameServicesXblPackagesCommand : AzureGameServicesHttpClientCommandBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Xbox Live compute instance name.")]
        [ValidateNotNullOrEmpty]
        public string XblComputeName { get; set; }

        public IXblComputeClient Client { get; set; }

        public override void ExecuteCmdlet()
        {
            Client = Client ?? new XblComputeClient(CurrentSubscription, WriteDebug);
            XblPackageCollectionResponse result = null;

            CatchAggregatedExceptionFlattenAndRethrow(() => { result = Client.GetXblPackages(XblComputeName).Result; });
            WriteObject(result);
        }
    }
}