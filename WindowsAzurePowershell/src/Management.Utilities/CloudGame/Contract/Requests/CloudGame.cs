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
    /// The cloud game definition.
    /// </summary>
    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Used by JavaScriptSerializer")]
    [DataContract(Namespace = "")]
    public class CloudGames
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(Name = "name")]
        public string name { get; set; }

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
        /// Gets or sets the status
        /// </summary>
        [DataMember(Name = "status")]
        public string status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game can be deployed.
        /// </summary>
        [DataMember(Name = "canDeploy")]
        public bool canDeploy { get; set; }

        /// <summary>
        /// Gets or sets the subscription id.
        /// </summary>
        [DataMember(Name = "subscriptionId")]
        public string subscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the resource sets.
        /// </summary>
        /// <value>
        /// The resource sets.
        /// </value>
        [DataMember(Name = "resourceSets")]
        public string resourceSets { get; set; }

        /// <summary>
        /// Gets or sets the sandboxes.
        /// </summary>
        /// <value>
        /// The sandboxes.
        /// </value>
        [DataMember(Name = "sandboxes")]
        public string sandboxes { get; set; }

        /// <summary>
        /// Gets or sets the schema id.
        /// </summary>
        /// <value>
        /// The schema id.
        /// </value>
        [DataMember(Name = "schemaId")]
        public string schemaId { get; set; }

        /// <summary>
        /// Gets or sets the name of the schema.
        /// </summary>
        /// <value>
        /// The name of the schema.
        /// </value>
        [DataMember(Name = "schemaName")]
        public string schemaName { get; set; }

        /// <summary>
        /// Gets or sets the gsi set id.
        /// </summary>
        /// <value>
        /// The gsi set id.
        /// </value>
        [DataMember(Name = "gsiSetId")]
        public string gsiSetId { get; set; }

        /// <summary>
        /// Gets or sets the title id.
        /// </summary>
        /// <value>
        /// The title id.
        /// </value>
        [DataMember(Name = "titleId")]
        public string titleId { get; set; }

        /// <summary>
        /// Gets or sets the gsi set id.
        /// </summary>
        [DataMember(Name = "publisherId")]
        public string publisherId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember(Name = "type")]
        public string type 
        { 
            get 
            { 
                return EntityTypeConstants.CloudGame; 
            } 
            
            set 
            { 
            } 
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember(Name = "id")]
        public string id
        {
            get
            {
                return this.gsiSetId;
            }

            set
            {
            }
        }
    }
}
