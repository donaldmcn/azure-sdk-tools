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

using Microsoft.WindowsAzure.Management.Utilities.CloudGame;
using System.Management.Automation;

namespace Microsoft.WindowsAzure.Management.CloudGame
{
    /// <summary>
    /// Remove a cloud game service.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureGameService"), OutputType(typeof(bool))]
    public class RemoveGameServiceCommand : AzureCloudGameHttpClientCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The cloud game service name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(HelpMessage = "Do not confirm deletion of game package.")]
        public SwitchParameter Force { get; set; }

        public ICloudGameClient CloudGameClient { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(Force.IsPresent,
                          string.Format("Game Service:{0} will be deleted by this action.", Name),
                          string.Empty,
                          string.Empty,
                          () =>
                          {
                              CloudGameClient = CloudGameClient ?? new CloudGameClient(CurrentSubscription, WriteDebug);
                              var result = false;
                              CatchAggregatedExceptionFlattenAndRethrow(() => { result = CloudGameClient.RemoveGameService(Name).Result; });
                              WriteObject(result);
                          });
        }
    }
}