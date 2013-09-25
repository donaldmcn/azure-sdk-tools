﻿// Licensed under the Apache License, Version 2.0 (the "License");
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
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class PostCloudGameAssetResponse
    {
        /// <summary>
        /// Gets or sets the game asset id.
        /// </summary>
        /// <value>
        /// The game asset id.
        /// </value>
        [DataMember(Name = "gameAssetId")]
        public string GameAssetId { get; set; }

        /// <summary>
        /// Gets or sets the game asset pre auth URL.
        /// </summary>
        /// <value>
        /// The game asset pre auth URL.
        /// </value>
        [DataMember(Name = "gameAssetUrl")]
        public string GameAssetPreAuthUrl
        {
            get;
            set;
        }
    }
}