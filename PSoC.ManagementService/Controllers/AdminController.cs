using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Core.Extensions;
using PSoC.ManagementService.Core.SearchFilter;
using PSoC.ManagementService.Filter;
using PSoC.ManagementService.Models;
using PSoC.ManagementService.Responses;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Controllers
{
    [Authorize(Roles = "GlobalAdmin, DistrictAdmin, SchoolAdmin")]
    public class AdminController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly IDistrictService _districtService;
        private readonly ISchoolService _schoolService;
        private AdminAccessPointViewModel _viewModel;

        #region UserData

        private bool IsAuthenticated
        {
            get { return (User != null && User.Identity != null && User.Identity.IsAuthenticated); }
        }

        private ClaimsIdentity UserIdentity
        {
            get
            {
                if (IsAuthenticated) return (ClaimsIdentity) User.Identity;
                var ex = new Exception("User not authenticated or failed to retrieve identity");
                PEMSEventSource.Log.ApplicationException(ex.Message);
                throw ex;
            }
        }

        private AdminType UserType
        {
            get
            {
                var role = UserIdentity.FindFirst(ClaimTypes.Role);
                if (role != null && !string.IsNullOrEmpty(role.Value)) return Enum<AdminType>.Parse(role.Value);
                var ex = new Exception("User Role not found");
                PEMSEventSource.Log.ApplicationException(ex.Message);
                throw ex;
            }
        }

        private string Username
        {
            get
            {
                var username = UserIdentity.FindFirst(ClaimTypes.Name);
                if (username != null && !string.IsNullOrEmpty(username.Value)) return username.Value;
                var ex = new Exception("Username not found");
                PEMSEventSource.Log.ApplicationException(ex.Message);
                throw ex;
            }
        }

        private Guid DistrictId
        {
            get
            {
                Guid districtId;
                var district = UserIdentity.FindFirst(x => x.Type == "District");
                if ((district == null) || (!Guid.TryParse(district.Value, out districtId)))
                {
                    var ex = new Exception("District Id not found");
                    PEMSEventSource.Log.ApplicationException(ex.Message);
                    throw ex;
                }
                return districtId;
            }
        }

        private Guid SchoolId
        {
            get
            {
                Guid schoolId;
                var school = UserIdentity.FindFirst(x => x.Type == "School");
                if ((school == null) || (!Guid.TryParse(school.Value, out schoolId)))
                {
                    var ex = new Exception("School Id not found");
                    PEMSEventSource.Log.ApplicationException(ex.Message);
                    throw ex;
                }
                return schoolId;
            }
        }

        private Guid? InstitutionEntityId
        {
            get
            {
                Guid? institutionEntityId = null;
                switch (UserType)
                {
                    case AdminType.DistrictAdmin:
                        institutionEntityId = DistrictId;
                        break;
                    case AdminType.SchoolAdmin:
                        institutionEntityId = SchoolId;
                        break;
                }
                return institutionEntityId;
            }
        }

        #endregion

        public AdminController(IDeviceService deviceService,
                               IDistrictService districtService,
                               ISchoolService schoolService)
        {
            _deviceService = deviceService;
            _districtService = districtService;
            _schoolService = schoolService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.IsAuthenticated = IsAuthenticated;
            ViewBag.Username = Username;
            ViewBag.UserType = UserType.ToString();
            await LoadFiltersAndInitViewModel().ConfigureAwait(false);
            return View(_viewModel);
        }

        /// <summary>
        /// Invoked as AJAX call
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxRequest]
        public async Task<ActionResult> Dashboard(DataTablePageRequestModel jQueryDataTablesModel)
        {
            if (jQueryDataTablesModel == null)
            {
                jQueryDataTablesModel = new DataTablePageRequestModel
                {
                    Start = 0,   // Index (based on list before filtering) of the first Record on the page
                    Length = 10, // Page Size (# of Records Per Page)
                    DistrictFilter = null,
                    Draw = 1,
                };
            }
            else if (jQueryDataTablesModel.Length <= 0)
            {
                jQueryDataTablesModel.Length = 10;
            }
            else if (jQueryDataTablesModel.Start < 0)
            {
                jQueryDataTablesModel.Start = 0;
            }

            var filters = ExtractSearchFilters(jQueryDataTablesModel);
                
            var result = await _deviceService.GetAccessPointDeviceStatusAsync(
                                                                            UserType,
                                                                            InstitutionEntityId,
                                                                            jQueryDataTablesModel.Length,
                                                                            jQueryDataTablesModel.Start,
                                                                            filters,
                                                                            null).ConfigureAwait(false);  //Inject Search Filters

            return Json(new DataTablesResponse<AccessPointDeviceStatus>(result.Item1,
                                                                        result.Item2,
                                                                        result.Item2,
                                                                        jQueryDataTablesModel.Draw));
        }

        private static List<SearchFilter> ExtractSearchFilters(DataTablePageRequestModel jQueryDataTablesModel)
        {
            List<SearchFilter> filters;
            if (jQueryDataTablesModel.DistrictFilter != null)
            {
                var guidList = new List<Guid>();
                foreach (var item in jQueryDataTablesModel.DistrictFilter)
                {
                    Guid guidId;
                    if (Guid.TryParse(item, out guidId))
                    {
                        guidList.Add(guidId);
                    }
                }
                filters = new List<SearchFilter>
                {
                    new DistrictFilter(guidList, DistrictFilterOperator.Contains)
                };
            }
            else
            {
                filters = null;
            }
            return filters;
        }

        /// <summary>
        /// Load District, School and Accesspoint Filters DropDown For Admin Dashboard
        /// </summary>
        /// <returns></returns>
        private async Task LoadFiltersAndInitViewModel()
        {
            IList<District> districtList;
            if (UserType != AdminType.SchoolAdmin)
            {
                districtList = await _districtService.GetAsync(Username).ConfigureAwait(false);
            }
            else
            {
                districtList = new List<District>();
                var school = await _schoolService.GetByIdAsync(SchoolId).ConfigureAwait(false);
                districtList.Add(school.District);
                
            }
            _viewModel = new AdminAccessPointViewModel(districtList);
        }
    }
}