﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Interfaces
{
    public interface ISchoolRepository : IDataRepository<SchoolDto, Guid>
    {
        Task<IList<SchoolDto>> GetByDistrictIdAsync(Guid key);
    }
}