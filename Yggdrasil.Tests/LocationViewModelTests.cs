using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Client.Services;
using Yggdrasil.Client.ViewModels;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Tests
{
    [TestFixture]
    public sealed class LocationViewModelTests
    {
        [Test]
        public async Task TryUpdateLocationMovedToNewParent()
        {
            Location source = CreateTestLocation("LocationID", "ParentID");
            Mock<ICampaignService> service = new Mock<ICampaignService>();
            LocationViewModel viewModel = new LocationViewModel(source, service.Object);

            service.Setup(p => p.GetLocation("NewParentID", It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(CreateTestLocation("NewParentID")));

            await viewModel.TryUpdate(new LocationsMoved() { Locations = new Dictionary<string, string>() { { "LocationID", "NewParentID" } } });

            Assert.AreEqual(1, viewModel.Ancestors.Count());
            Assert.AreEqual("NewParentID", viewModel.Ancestors.ElementAt(0).Id);
        }

        [Test]
        public async Task TryUpdateLocationChildrenMovedInAndOut()
        {
            Location source = CreateTestLocation("LocationID", "ParentID");
            source.ChildLocations = new LocationListItem[] { new LocationListItem() { Id = "OldChildID", Name = "Test", Tags = Array.Empty<string>() } };
            Mock<ICampaignService> service = new Mock<ICampaignService>();
            LocationViewModel viewModel = new LocationViewModel(source, service.Object);

            service.Setup(p => p.GetLocation("NewChildID", It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(CreateTestLocation("NewChildID", "LocationID")));

            await viewModel.TryUpdate(new LocationsMoved() { Locations = new Dictionary<string, string>() { { "NewChildID", "LocationID" }, { "OldChildID", "SomeOtherID" } } });

            Assert.AreEqual(1, viewModel.ChildLocations.Count());
            Assert.AreEqual("NewChildID", viewModel.ChildLocations.ElementAt(0).Id);
        }

        Location CreateTestLocation(string ID, string parentID = default)
        {
            return new Location()
            {
                ChildLocations = Array.Empty<LocationListItem>(),
                Description = "Test Description",
                Name = "Test Name",
                ID = ID,
                ParentId = parentID,
                Population = new Population(),
                Tags = Array.Empty<string>(),
                ParentsPath = parentID != null
                    ? new LocationListItem[] { new LocationListItem() { Id = parentID, Name = "Parent", Tags = Array.Empty<string>() } }
                    : Array.Empty<LocationListItem>(),
            };
        }
    }
}
