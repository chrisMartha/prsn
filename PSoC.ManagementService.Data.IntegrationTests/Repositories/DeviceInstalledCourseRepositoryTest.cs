﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class DeviceInstalledCourseRepositoryTest
    {
        private DeviceInstalledCourseRepository _sut;

        private async Task Cleanup(Guid deviceId, Guid courseId)
        {
            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[DeviceInstalledCourse] WHERE [DeviceID] = '" + deviceId + "'").ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[Device] WHERE [DeviceID] = '" + deviceId + "'").ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[Course] WHERE [CourseLearningResourceID] = '" + courseId + "'").ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeviceInstalledCourseRepository_CourseInTable_DicInTable_ShouldSuccess()
        {
            //Set up
            var deviceId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var list = await SetupGeneralData(deviceId, courseId1).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format(
                    "INSERT INTO [dbo].[Course] ([CourseLearningResourceID]) VALUES ('{0}')",
                        courseId1)).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format(
                    "INSERT INTO [dbo].[DeviceInstalledCourse] ([DeviceID], [CourseLearningResourceID], [PercentDownloaded], [LastUpdated]) VALUES ('{0}', '{1}', {2}, '{3}')",
                        deviceId, courseId1, "3.14", DateTime.UtcNow)).ConfigureAwait(false);
            //Execute
            var result = await _sut.ImportDataAsync(deviceId, list).ConfigureAwait(false);
            //Assert
            Assert.AreEqual(true, result);
            //Clean
            await Cleanup(deviceId, courseId1).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeviceInstalledCourseRepository_CourseInTable_DicNotInTable_ShouldSuccess()
        {
            //Setup
            var deviceId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var list = await SetupGeneralData(deviceId, courseId1).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format("INSERT INTO [dbo].[Course] ([CourseLearningResourceID]) VALUES ('{0}')", courseId1)).ConfigureAwait(false);
            //Execute
            var result = await _sut.ImportDataAsync(deviceId, list).ConfigureAwait(false);
            //Assert
            Assert.AreEqual(true, result);
            //Clean
            await Cleanup(deviceId, courseId1).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeviceInstalledCourseRepository_CourseNotInTable_DicNotInTable_NullValue_ShouldSuccess()
        {
            //Setup
            var deviceId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var list = await SetupGeneralData(deviceId, courseId1).ConfigureAwait(false);
            //Execute
            var result = await _sut.ImportDataAsync(deviceId, list).ConfigureAwait(false);
            //Assert
            Assert.AreEqual(true, result);
            //Clean
            await Cleanup(deviceId, courseId1).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeviceInstalledCourseRepository_CourseNotInTable_DicNotInTable_ShouldSuccess()
        {
            //Setup
            var deviceId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var list = await SetupGeneralData(deviceId, courseId1).ConfigureAwait(false);
            list[0].Course.Grade = "2";
            list[0].Course.Subject = "ELA";
            list[0].PercentDownloaded = 22.8m;
            //Execute
            var result = await _sut.ImportDataAsync(deviceId, list).ConfigureAwait(false);
            //Assert
            Assert.AreEqual(true, result);
            //Clean
            await Cleanup(deviceId, courseId1).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeviceInstalledCourseRepository_MultipleCourseInTable_DicInTable_ShouldSuccess()
        {
            //Set up
            var deviceId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();
            var list = await SetupGeneralData(deviceId, courseId1).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format(
                    "INSERT INTO [dbo].[Course] ([CourseLearningResourceID]) VALUES ('{0}')",
                        courseId1)).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format(
                    "INSERT INTO [dbo].[DeviceInstalledCourse] ([DeviceID], [CourseLearningResourceID], [PercentDownloaded], [LastUpdated]) VALUES ('{0}', '{1}', {2}, '{3}')",
                        deviceId, courseId1, "3.14", DateTime.UtcNow)).ConfigureAwait(false);
            var dto2 = new DeviceInstalledCourseDto
            {
                Course = new CourseDto
                {
                    CourseLearningResourceID = courseId2
                },
                Device = new DeviceDto
                {
                    DeviceID = deviceId
                }
            };
            list.Add(dto2);
            //Execute
            var result = await _sut.ImportDataAsync(deviceId, list).ConfigureAwait(false);
            //Assert
            Assert.AreEqual(true, result);
            //Clean
            await Cleanup(deviceId, courseId1).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[Course] WHERE [CourseLearningResourceID] = '" + courseId2 + "'").ConfigureAwait(false);
        }

        [TestInitialize]
        public void Initialize()
        {
            _sut = new DeviceInstalledCourseRepository();
        }

        private async Task<List<DeviceInstalledCourseDto>> SetupGeneralData(Guid deviceId, Guid courseId1)
        {
            //Set up
            var query1 = string.Format("INSERT INTO [dbo].[Device] ([DeviceID]) VALUES ('{0}')", deviceId);
            await DataAccessHelper.ExecuteAsync(query1).ConfigureAwait(false);
            var list = new List<DeviceInstalledCourseDto>();
            var dto1 = new DeviceInstalledCourseDto()
            {
                Course = new CourseDto
                {
                    CourseLearningResourceID = courseId1,
                },
                Device = new DeviceDto
                {
                    DeviceID = deviceId
                }
            };
            list.Add(dto1);
            return list;
        }
    }
}