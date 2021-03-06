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

namespace Microsoft.WindowsAzure.Management.Utilities.Websites.Services.WebEntities
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Namespace = UriElements.ServiceNamespace)]
    public class StampCapacity
    {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long AvailableCapacity { get; set; }

        [DataMember]
        public long TotalCapacity { get; set; }

        [DataMember]
        public string Unit { get; set; }

        [DataMember(IsRequired = false)]
        public ComputeModeOptions? ComputeMode { get; set; }

        [DataMember(IsRequired = false)]
        public WorkerSizeOptions? WorkerSize { get; set; }

        [DataMember(IsRequired = false)]
        public bool ExcludeFromCapacityAllocation { get; set; }


        [DataMember]
        public bool IsApplicableForAllComputeModes { get; set; }

        [DataMember(IsRequired = false)]
        public string SiteMode { get; set; }

        public StampCapacity()
        {
        }

        public StampCapacity(string name, string unit, long available, long total)
            : this()
        {
            Name = name;
            AvailableCapacity = available;
            TotalCapacity = total;
            Unit = unit;
        }
    }

    [CollectionDataContract(Namespace = UriElements.ServiceNamespace)]
    public class StampCapacities : List<StampCapacity>
    {

        public StampCapacities()
        {
        }

        public StampCapacities(List<StampCapacity> capacities)
            : base(capacities)
        {
        }
    }

}
