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
    using System.Collections.Generic;

    /// <summary>
    /// The game mode object.
    /// </summary>
    [DataContract]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Used by JavaScriptSerializer")]
    public sealed class GameMode
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(Name = "name")]
        public string name
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
        /// Gets or sets the type.
        /// </summary>
        [DataMember(Name = "type")]
        public string type
        {
            get
            {
                return EntityTypeConstants.GameMode;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [DataMember(Name = "fileName")]
        public string fileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [DataMember(Name = "status")]
        public string status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember(Name = "id")]
        public string id
        {
            get;
            set;
        }
    }

    /// <summary>
    /// A collection of cloud game responses
    /// </summary>
    [DataContract(Namespace = "")]
    public class GameModeCollectionResponse
    {
        /// <summary>
        /// Gets or sets the cloud game responses.
        /// </summary>
        /// <value>
        /// The cloud game responses.
        /// </value>
        [DataMember(Name = "cloudGameVariant", Order = 0)]
        public List<GameMode> CloudGameVariants
        {
            get;
            set;
        }
    }
}
