// ----------------------------------------------------------------------------------
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
using System.Management.Automation;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract;

namespace Microsoft.WindowsAzure.Management.CloudGame
{
    /// <summary>
    /// Remove the cloud game mode.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureGameMode"), OutputType(typeof(bool))]
    public class RemoveGameModeCommand : AzureCloudGameHttpClientCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The cloud game id.")]
        [ValidateNotNullOrEmpty]
        public string CloudGameId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The cloud game mode id.")]
        [ValidateNotNullOrEmpty]
        public System.Guid VariantId { get; set; }

        [Parameter(HelpMessage = "Do not confirm deletion of game mode.")]
        public SwitchParameter Force { get; set; }

        public ICloudGameClient CloudGameClient { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(Force.IsPresent,
                          string.Format("VariantId:{0} will be deleted by this action.", VariantId),
                          string.Empty,
                          string.Empty,
                          () =>
                          {
                              CloudGameClient = CloudGameClient ?? new CloudGameClient(CurrentSubscription, WriteDebug);
                              bool result = false;

                              CatchAggregatedExceptionFlattenAndRethrow(() => { result = CloudGameClient.RemoveGameMode(CloudGameId, VariantId).Result; });
                              WriteObject(result);
                          });
        }
    }
}