﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSoC.ManagementService.Core;
using PSoC.ManagementService.Core.SearchFilter;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Interfaces
{
    public interface IDeviceService
    {
        Task SaveDownloadStatusAsync(Guid deviceId, List<Course> courses, LogRequest logRequest);

        /// <summary>
        /// Get both 1)a list of AccessPoint-Device Status based on user's admin type (on given page) and 2) total number of records
        /// </summary>
        /// <param name="adminType">User's admin type: GlobalAdmin, DistrictAdmin or SchoolAdmin</param>
        /// <param name="id">
        /// Global Admin: id = null; 
        /// District Admin: id = district id(not nullable); 
        /// School Admin: id = school id(not nullable)
        /// </param>
        /// <param name="pageSize">Number of status items on each page</param>
        /// <param name="startIndex">Index of the record to be displayed at the beginning (0 based)</param>
        /// <param name="filters">additional search filters injected from UI layer</param>
        /// <param name="logRequest">logging helper</param>
        /// <returns>
        /// List<AccessPointDeviceStatus>: List of AccessPoint-Device Status objects 
        /// int: total number of records before pagination
        /// </returns>
        Task<Tuple<List<AccessPointDeviceStatus>, int>> GetAccessPointDeviceStatusAsync(AdminType adminType,
                                                                                        Guid? id,
                                                                                        int pageSize,
                                                                                        int startIndex,
                                                                                        IReadOnlyCollection<SearchFilter> filters, 
                                                                                        LogRequest logRequest);
    }
}
