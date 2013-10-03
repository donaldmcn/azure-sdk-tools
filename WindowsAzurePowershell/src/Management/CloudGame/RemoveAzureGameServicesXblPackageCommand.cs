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
    using System.Management.Automation;
    using Microsoft.WindowsAzure.Management.Utilities.XblCompute;

    /// <summary>
    /// Remove the cloud game package.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureGameServicesXblPackage"), OutputType(typeof(bool))]
    public class RemoveAzureGameServicesXblPackageCommand : AzureGameServicesHttpClientCommandBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Xbox Live compute instance name.")]
        [ValidateNotNullOrEmpty]
        public string XblComputeName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The XblPackage Id.")]
        [ValidateNotNullOrEmpty]
        public System.Guid PackageId { get; set; }

        [Parameter(HelpMessage = "Do not confirm deletion of game package.")]
        public SwitchParameter Force { get; set; }

        public IXblComputeClient Client { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(Force.IsPresent,
                          string.Format("GsiId:{0} will be deleted by this action.", PackageId),
                          string.Empty,
                          string.Empty,
                          () =>
                          {
                              Client = Client ?? new XblComputeClient(CurrentSubscription, WriteDebug);
                              var result = false;

                              CatchAggregatedExceptionFlattenAndRethrow(() => { result = Client.RemoveXblPackage(XblComputeName, PackageId).Result; });
                              WriteObject(result);
                          });
        }
    }
}