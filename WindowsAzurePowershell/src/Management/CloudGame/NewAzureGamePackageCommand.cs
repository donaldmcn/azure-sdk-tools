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
    [Cmdlet(VerbsCommon.New, "AzureGamePackage"), OutputType(typeof(bool))]
    public class NewGamePackageCommand : AzureCloudGameHttpClientCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The cloud game Name.")]
        [ValidateNotNullOrEmpty]
        public string CloudGameName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the package.")]
        [ValidateNotNullOrEmpty]
        public string PackageName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The maximum number of players allowed.")]
        [ValidateNotNull]
        public int MaxPlayers { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The id of the asset file to use")]
        public string AssetId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the local cspkg file")]
        [ValidateNotNullOrEmpty]
        public string CspkgFileName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The game CSPKG file stream.")]
        [ValidateNotNullOrEmpty]
        public Stream CspkgStream { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the local cscfg file")]
        [ValidateNotNullOrEmpty]
        public string CscfgFileName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The game CSCFG file stream.")]
        [ValidateNotNullOrEmpty]
        public Stream CscfgStream { get; set; }

        public ICloudGameClient CloudGameClient { get; set; }

        public override void ExecuteCmdlet()
        {
            CloudGameClient = CloudGameClient ?? new CloudGameClient(CurrentSubscription, WriteDebug);
            var result = false;
            CatchAggregatedExceptionFlattenAndRethrow(
                () =>
                {
                    result = CloudGameClient.CreateGamePackage(
                        CloudGameName,
                        PackageName,
                        MaxPlayers,
                        AssetId,
                        CspkgFileName,
                        CspkgStream,
                        CscfgFileName,
                        CscfgStream).Result; 
                });
            WriteObject(result);
        }
    }
}