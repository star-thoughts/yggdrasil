﻿using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Models.Locations;
using Yggdrasil.Server.Hubs;
using Yggdrasil.Server.Services;
using Yggdrasil.Server.Storage;

namespace Yggdrasil.Tests
{
    [TestFixture]
    public sealed class LocationsServiceTests
    {
        [Test]
        public void ConstructorNegativeTests()
        {
            ArgumentException exception = Assert.Throws<ArgumentNullException>(() => new LocationsService(null, Mock.Of<IHubContext<ServiceHub>>(), Mock.Of<IAuditStorage>()));
            Assert.AreEqual("storage", exception.ParamName);

            exception = Assert.Throws<ArgumentNullException>(() => new LocationsService(Mock.Of<ICampaignStorage>(), null, Mock.Of<IAuditStorage>()));
            Assert.AreEqual("hub", exception.ParamName);

            exception = Assert.Throws<ArgumentNullException>(() => new LocationsService(Mock.Of<ICampaignStorage>(), Mock.Of<IHubContext<ServiceHub>>(), null));
            Assert.AreEqual("auditor", exception.ParamName);
        }

        [Test]
        public void GetRootLocationsNegativeTests()
        {
            LocationsService service = new LocationsService(Mock.Of<ICampaignStorage>(), Mock.Of<IHubContext<ServiceHub>>(), Mock.Of<IAuditStorage>());

            ArgumentNullException exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.GetRootLocations(null));
            Assert.AreEqual("campaignId", exception.ParamName);
        }

        [Test]
        public async Task GetRootLocations()
        {
            string campaignId = "campaign";
            Mock<ICampaignStorage> storage = new Mock<ICampaignStorage>();
            LocationsService service = new LocationsService(storage.Object, Mock.Of<IHubContext<ServiceHub>>(), Mock.Of<IAuditStorage>());

            storage.Setup(p => p.GetRootLocations(campaignId, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((IEnumerable<LocationListItem>)Array.Empty<LocationListItem>()));

            await service.GetRootLocations(campaignId);

            storage.Verify(p => p.GetRootLocations(campaignId, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void AddLocationNegativeTests()
        {
            LocationsService service = new LocationsService(Mock.Of<ICampaignStorage>(), Mock.Of<IHubContext<ServiceHub>>(), Mock.Of<IAuditStorage>());

            ArgumentNullException exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.AddLocation(null, "user", "name", "description", "parentId", null, Array.Empty<string>()));
            Assert.AreEqual("campaignId", exception.ParamName);

            exception = Assert.ThrowsAsync<ArgumentNullException>(() => service.AddLocation("campaignId", null, "name", "description", "parentId", null, Array.Empty<string>()));
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public async Task AddLocation()
        {
            string campaignId = "campaign";
            Mock<ICampaignStorage> storage = new Mock<ICampaignStorage>();
            LocationsService service = new LocationsService(storage.Object, Mock.Of<IHubContext<ServiceHub>>(), Mock.Of<IAuditStorage>());

            storage.Setup(p => p.AddLocation(campaignId, "name", "description", "parentId", null, Array.Empty<string>(), CancellationToken.None))
                .Returns(Task.FromResult(new Location() { Id = "1" }));

            string id = await service.AddLocation(campaignId, "user", "name", "description", "parentId", null, Array.Empty<string>());
            Assert.AreEqual("1", id);

            storage.Verify(p => p.AddLocation(campaignId, "name", "description", "parentId", null, Array.Empty<string>(), CancellationToken.None), Times.Once());
        }
    }
}
