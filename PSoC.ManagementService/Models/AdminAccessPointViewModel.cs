using System;
using System.Collections.Generic;

using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Models
{
    /// <summary>
    /// View Model for Admin Dashboard
    /// </summary>
    public class AdminAccessPointViewModel
    {
        /// <summary>
        /// Constructor with districtList (all districts)
        /// </summary>
        /// <param name="districtList"></param>
        public AdminAccessPointViewModel(IEnumerable<District> districtList)
        {
            UpdateDistrictList(districtList);
        }

        /// <summary>
        /// Get District Dropdown List
        /// </summary>
        public List<Tuple<Guid, string>> DistrictList
        {
            get { return _districtList; }
        }

        private void UpdateDistrictList(IEnumerable<District> districtList)
        {
            _districtList = new List<Tuple<Guid, string>>();
            if (districtList != null)
            {
                foreach (var district in districtList)
                {
                    _districtList.Add(new Tuple<Guid, string>(district.DistrictId, district.DistrictName));
                }
            }
        }

        private List<Tuple<Guid, string>> _districtList;
    }
}