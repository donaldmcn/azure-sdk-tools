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
    /// Get the game service deployment report
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureGameServicesXblComputeDeploymentReport"), OutputType(typeof(XblComputeDeploymentData))]
    public class GetAzureGameServicesXblComputeDeploymentReportCommand : AzureGameServicesHttpClientCommandBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Xbox Live compute instance name.")]
        [ValidateNotNullOrEmpty]
        public string XblComputeName { get; set; }

        public IXblComputeClient Client { get; set; }

        public override void ExecuteCmdlet()
        {
            Client = Client ?? new XblComputeClient(CurrentSubscription, WriteDebug);
            XblComputeDeploymentData result = null;

            CatchAggregatedExceptionFlattenAndRethrow(() => { result = Client.GetXblComputeDeploymentsReport(XblComputeName).Result; });
            WriteObject(result);
        }
    }
}