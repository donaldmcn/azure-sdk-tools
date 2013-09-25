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

namespace Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract
{
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// The put cloud game request
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Used by JavaScriptSerializer")]
    [DataContract(Namespace = "")]
    public class CloudServiceResource
    {
        /// <summary>
        /// Gets or sets the cloud game.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the cloud game.
        /// </summary>
        [DataMember(Name = "ETag")]
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the cloud game.
        /// </summary>
        [DataMember(Name = "Plan")]
        public string Plan { get; set; }

        /// <summary>
        /// Gets or sets the cloud game.
        /// </summary>
        [DataMember(Name = "ResourceProviderNamespace")]
        public string ResourceProviderNamespace { get; set; }

        /// <summary>
        /// Gets or sets the cloud game.
        /// </summary>
        [DataMember(Name = "Type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the cloud game.
        /// </summary>
        [DataMember(Name = "SchemaVersion")]
        public string SchemaVersion { get; set; }

        /// <summary>
        /// Gets or sets the cloud game.
        /// </summary>
        [DataMember(Name = "IntrinsicSettings")]
        public XmlNode[] IntrinsicSettings { get; set; }
    }
}
