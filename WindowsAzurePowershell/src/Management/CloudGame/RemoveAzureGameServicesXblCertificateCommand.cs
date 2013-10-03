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
    using System.Management.Automation;

    /// <summary>
    /// Remove the cloud game certificate.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureGameServicesXblCertificate"), OutputType(typeof(bool))]
    public class RemoveAzureGameServicesXblCertificateCommand : AzureGameServicesHttpClientCommandBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Xbox Live compute instance name.")]
        [ValidateNotNullOrEmpty]
        public string XblComputeName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The XblCertificate id.")]
        [ValidateNotNullOrEmpty]
        public System.Guid CertificateId { get; set; }

        [Parameter(HelpMessage = "Do not confirm deletion of XblCertificate.")]
        public SwitchParameter Force { get; set; }

        public IXblComputeClient Client { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(Force.IsPresent,
                          string.Format("CertificateId:{0} will be deleted by this action.", CertificateId),
                          string.Empty,
                          string.Empty,
                          () =>
                          {
                              Client = Client ?? new XblComputeClient(CurrentSubscription, WriteDebug);
                              var result = false;

                              CatchAggregatedExceptionFlattenAndRethrow(() => { result = Client.RemoveXblCertificate(XblComputeName, CertificateId).Result; });
                              WriteObject(result);
                          });
        }
    }
}