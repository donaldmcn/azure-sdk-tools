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
using System.Web;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract;

namespace Microsoft.WindowsAzure.Management.CloudGame
{
    /// <summary>
    /// Create cloud game package.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureGamePackage"), OutputType(typeof(CloudGameImage))]
    public class NewGamePackageCommand : AzureCloudGameHttpClientCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The cloud game id.")]
        [ValidateNotNullOrEmpty]
        public string CloudGameId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The cloud game image.")]
        [ValidateNotNullOrEmpty]
        public CloudGameImage GameImage { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The game CSPKG.")]
        [ValidateNotNullOrEmpty]
        public Stream CsPkg { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The game CSCFG.")]
        [ValidateNotNullOrEmpty]
        public Stream CsCfg { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The game asset.")]
        [ValidateNotNullOrEmpty]
        public Stream Asset { get; set; }

        public ICloudGameClient CloudGameClient { get; set; }

        public override void ExecuteCmdlet()
        {
            CloudGameClient = CloudGameClient ?? new CloudGameClient(CurrentSubscription, WriteDebug);
            CloudGameImage result = null;

            CatchAggregatedExceptionFlattenAndRethrow(() => { result = CloudGameClient.CreateGameImage(CloudGameId, GameImage, CsPkg, CsCfg, Asset).Result; });
            WriteObject(result);
        }
    }
}