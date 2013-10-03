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

namespace Microsoft.WindowsAzure.Management.Utilities.XblCompute.Contract
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "")]
    public class XblCertificateRequest
    {
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        [DataMember(Name = "fileName")]
        public string Filename
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [DataMember(Name = "password")]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }
    }
}