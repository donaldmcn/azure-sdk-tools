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
    /// Create the cloud game asset.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureGameAsset"), OutputType(typeof(PostCloudGameAssetResponse))]
    public class NewAzureGameAssetCommand : AzureCloudGameHttpClientCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The cloud game Name.")]
        [ValidateNotNullOrEmpty]
        public string CloudGameName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the game asset.")]
        [ValidateNotNullOrEmpty]
        public string GameAssetName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The original filename of the game asset file.")]
        [ValidateNotNullOrEmpty]
        public string GameAssetFileName { get; set; }
        
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The asset file.")]
        [ValidateNotNullOrEmpty]
        public Stream GameAssetStream { get; set; }

        public ICloudGameClient CloudGameClient { get; set; }

        public override void ExecuteCmdlet()
        {
            CloudGameClient = CloudGameClient ?? new CloudGameClient(CurrentSubscription, WriteDebug);
            PostCloudGameAssetResponse result = null;

            CatchAggregatedExceptionFlattenAndRethrow(() => { result = CloudGameClient.CreateGameAsset(CloudGameName, GameAssetName, GameAssetFileName, GameAssetStream).Result; });
            WriteObject(result);
        }
    }
}