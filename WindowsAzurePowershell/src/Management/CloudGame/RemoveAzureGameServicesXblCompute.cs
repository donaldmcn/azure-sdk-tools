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
    /// Remove a XblCompute instance.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureGameServicesXblCompute"), OutputType(typeof(bool))]
    public class RemoveAzureGameServicesXblComputeCommand : AzureGameServicesHttpClientCommandBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Xbox Live compute instance name.")]
        [ValidateNotNullOrEmpty]
        public string XblComputeName { get; set; }

        [Parameter(HelpMessage = "Do not confirm deletion of XblCompute instance.")]
        public SwitchParameter Force { get; set; }

        public IXblComputeClient Client { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(Force.IsPresent,
                          string.Format("Game Service:{0} will be deleted by this action.", XblComputeName),
                          string.Empty,
                          string.Empty,
                          () =>
                          {
                              Client = Client ?? new XblComputeClient(CurrentSubscription, WriteDebug);
                              var result = false;
                              CatchAggregatedExceptionFlattenAndRethrow(() => { result = Client.RemoveXblCompute(XblComputeName).Result; });
                              WriteObject(result);
                          });
        }
    }
}