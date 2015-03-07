﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.DataMapper;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.QueryFactory;

namespace PSoC.ManagementService.Data.Repositories
{
    public class UserRepository : Repository<UserDto, UserQuery, UserDataMapper, Guid>, IUserRepository
    {
        
    }
}