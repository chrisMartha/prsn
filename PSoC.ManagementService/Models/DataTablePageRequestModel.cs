using System.Collections.Generic;

namespace PSoC.ManagementService.Models
{
    /// <summary>
    /// Class that encapsulates most parameters consumed by DataTables plugin
    /// </summary>
    public class DataTablePageRequestModel
    {
        /// <summary>
        /// Display start point/first record, in the current data set, to be shown
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Number of records (page size) that the table can display in the current draw. It is expected that
        /// the number of recordsreturned will be equal to this number, unless the server has fewer records to return.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Information for DataTables to use for rendering.
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// List of applied district filter. Null if district filter is not applied (none selected)
        /// </summary>
        public IEnumerable<string> DistrictFilter { get; set; } 
    }
}