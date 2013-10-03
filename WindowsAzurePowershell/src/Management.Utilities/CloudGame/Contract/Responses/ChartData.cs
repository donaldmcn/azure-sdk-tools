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
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A collection of XlCompute status data
    /// </summary>
    [DataContract(Namespace = "")]
    public class ChartData
    {
        /// <summary>
        /// Gets or sets the active instance count.
        /// </summary>
        [DataMember(Name = "activeInstancesTimeSeries")]
        public List<int> ActiveInstancesTimeSeries
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the standby instance count.
        /// </summary>
        [DataMember(Name = "standbyInstancesTimeSeries")]
        public List<int> StandbyInstancesTimeSeries
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the quarantined instance count.
        /// </summary>
        [DataMember(Name = "quarantinedInstancesTimeSeries")]
        public List<int> QuarantinedInstancesTimeSeries
        {
            get;
            set;
        }
    }
}