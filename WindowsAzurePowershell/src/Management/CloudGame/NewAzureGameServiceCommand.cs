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
using System.IO;
using System.Management.Automation;
//using System.Web;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract;

namespace Microsoft.WindowsAzure.Management.CloudGame
{
    /// <summary>
    /// Create cloud game package.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureGameService"), OutputType(typeof(bool))]
    public class NewGameServiceCommand : AzureCloudGameHttpClientCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Title Id.")]
        [ValidateNotNullOrEmpty]
        public string TitleId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The sandboxes to use (comma seperated list.)")]
        [ValidateNotNullOrEmpty]
        public string Sandboxes { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resourceSetIds to use (comma seperated list.)")]
        [ValidateNotNullOrEmpty]
        public string ResourceSetIds { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Cloud Game name")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Cloud mode schema name")]
        [ValidateNotNullOrEmpty]
        public string SchemaName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Cloud mode schema local filename")]
        [ValidateNotNullOrEmpty]
        public string SchemaFileName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The cloud game schema file stream.")]
        [ValidateNotNullOrEmpty]
        public Stream SchemaStream { get; set; }

        public ICloudGameClient CloudGameClient { get; set; }

        public override void ExecuteCmdlet()
        {
            CloudGameClient = CloudGameClient ?? new CloudGameClient(CurrentSubscription, WriteDebug);
            var result = false;

            CatchAggregatedExceptionFlattenAndRethrow(() => { result = CloudGameClient.CreateGameService(
                TitleId,
                Sandboxes,
                ResourceSetIds,
                Name,
                SchemaName,
                SchemaFileName,
                SchemaStream).Result; });
            WriteObject(result);
        }
    }
}