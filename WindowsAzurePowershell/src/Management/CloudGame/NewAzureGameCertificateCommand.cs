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
using Microsoft.WindowsAzure.Management.Utilities.CloudGame;
using Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract;

namespace Microsoft.WindowsAzure.Management.CloudGame
{
    /// <summary>
    /// Create the cloud game certificate.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureGameCertificate"), OutputType(typeof(PostCloudGameCertificateResponse))]
    public class NewAzureGameCertificateCommand : AzureCloudGameHttpClientCommandBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The cloud game id.")]
        [ValidateNotNullOrEmpty]
        public string CloudGameId { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The could game certificate.")]
        [ValidateNotNullOrEmpty]
        public GameCertificate Certificate { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The could game certificate stream.")]
        [ValidateNotNullOrEmpty]
        public Stream CertificateStream { get; set; }

        public ICloudGameClient CloudGameClient { get; set; }

        public override void ExecuteCmdlet()
        {
            CloudGameClient = CloudGameClient ?? new CloudGameClient(CurrentSubscription, WriteDebug);
            PostCloudGameCertificateResponse result = null;

            CatchAggregatedExceptionFlattenAndRethrow(() => { result = CloudGameClient.CreateGameCertificate(CloudGameId, Certificate, CertificateStream).Result; });
            WriteObject(result);
        }
    }
}