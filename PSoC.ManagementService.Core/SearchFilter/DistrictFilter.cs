using System;
using System.Collections.Generic;

using PSoC.ManagementService.Core.Extensions;

namespace PSoC.ManagementService.Core.SearchFilter
{
    /// <summary>
    /// Represents District DropDown Filter on Global Admin dashboard  
    /// </summary>
    public class DistrictFilter : SearchFilter
    {
        /// <summary>
        /// Overloaded constructor to access List of Guid values defaulting to Filter Operator Contains for District Filter
        /// </summary>
        /// <param name="filterValues"></param>
        public DistrictFilter(IList<Guid> filterValues) : base(FilterType.DistrictId)
        {
            IdValues = filterValues;
            FilterOperator = DistrictFilterOperator.Contains;
        }

        /// <summary>
        /// Overloaded constructor to access List of Guid values and Filter Operator for District Filter
        /// </summary>
        /// <param name="filterValues"></param>
        /// <param name="filterOperator"></param>
        public DistrictFilter(IList<Guid> filterValues, DistrictFilterOperator filterOperator) : base(FilterType.DistrictId)
        {
            IdValues = filterValues;
            FilterOperator = filterOperator;
        }

        public IList<Guid> IdValues { get; private set; }

        public DistrictFilterOperator FilterOperator { get; private set; }

        public bool IsEnabled
        {
            get { return IdValues.HasElements(); }
        }
    }

    /// <summary>
    /// List of operations District filter supports. Behaviour for an operation is defined at Repository level.
    /// </summary>
    public enum DistrictFilterOperator
    {
        Contains = 1,
        Equals
    }
}
