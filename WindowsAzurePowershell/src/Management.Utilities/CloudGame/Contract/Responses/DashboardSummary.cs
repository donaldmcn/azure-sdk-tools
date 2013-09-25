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

    /// <summary>
    /// A collection of cloud game certificates
    /// </summary>
    [DataContract(Namespace = "")]
    public class DashboardSummary
    {
        /// <summary>
        /// Gets or sets the chart data.
        /// </summary>
        [DataMember(Name = "chartData")]
        public ChartData ChartData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the usage data.
        /// </summary>
        [DataMember(Name = "usageData")]
        public UsageData UsageData
        {
            get;
            set;
        }
    }
}