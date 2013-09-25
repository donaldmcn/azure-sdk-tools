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

namespace Microsoft.WindowsAzure.Management.Utilities.CloudGame.Contract
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The cloud game image
    /// </summary>
    [DataContract(Namespace = "")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Used by JavaScriptSerializer")]
    public class CloudGameImage
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember(Name = "name")]
        public string name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the max players.
        /// </summary>
        /// <value>
        /// The max players.
        /// </value>
        [DataMember(Name = "maxPlayers")]
        public string maxPlayers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [DataMember(Name = "status")]
        public string status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DataMember(Name = "id")]
        public string id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [DataMember(Name = "displayName")]
        public string displayName
        {
            get
            {
                return this.name;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the name of the package file.
        /// </summary>
        [DataMember(Name = "packageFile")]
        public string packageFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the package config file.
        /// </summary>
        [DataMember(Name = "configFile")]
        public string configFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the package asset file.
        /// </summary>
        [DataMember(Name = "assetFile")]
        public string assetFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember(Name = "type")]
        public string type
        {
            get
            {
                return EntityTypeConstants.GamePackage;
            }

            set
            {
            }
        }
    }
}
