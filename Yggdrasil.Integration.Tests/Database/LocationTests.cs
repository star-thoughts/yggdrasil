using NUnit.Framework;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Models.Locations;
using Yggdrasil.Server.Configuration;
using Yggdrasil.Server.Storage;
using Yggdrasil.Server.Storage.Mongo;

namespace Yggdrasil.Integration.Tests.Database
{
    public class LocationTests
    {
        private ICampaignStorage _storage;

        [SetUp]
        public async Task Setup()
        {
            StorageMongoDbConfiguration configuration = new StorageMongoDbConfiguration()
            {
                DatabaseName = "test_yggdrasil",
                ConnectionString = TestContext.Parameters["MongoDbUrl"],
            };
            _storage = new MongoDbCampaignStorage(configuration);

            await _storage.Connect();
        }

        [Test]
        public async Task LocationDataStoredWithNoChildrenOrParent()
        {
            if (_storage == null)
                throw new ArgumentNullException("_storage", "Storage must be initialized in Setup first.  Perhaps you need a runsettings file.");

            string campaignId = "campaign";
            string locationName = "LocationName";
            string description = "Description";
            string[] tags = new string[] { "tag1", "tag2" };
            Population population = new Population()
            {
                Populations = new PopulationEntry[]
                {
                    new PopulationEntry()
                    {
                        Count = "100%",
                        Name = "Elves",
                    }
                }
            };

            string locationId = (await _storage.AddLocation(campaignId, locationName, description, null, population, tags)).Id;

            Location location = await _storage.GetLocation(campaignId, locationId);

            Assert.IsNotNull(location);
            Assert.AreEqual(locationName, location.Name);
            Assert.AreEqual(description, location.Description);
            CollectionAssert.AreEqual(tags, location.Tags);

            // Check populations
            Assert.IsNotNull(location.Population);
            Assert.IsNotNull(location.Population.Populations);
            Assert.AreEqual(population.Populations[0].Count, location.Population.Populations[0].Count);
            Assert.AreEqual(population.Populations[0].Name, location.Population.Populations[0].Name);

            //  Check child locations
            Assert.IsNotNull(location.ChildLocations);
            Assert.AreEqual(0, location.ChildLocations.Length);

            //  Check parent locations
            Assert.IsNull(location.Parent);

            //  Cleanup
            await _storage.RemoveLocation(campaignId, locationId, false);
        }


        [Test]
        public async Task LocationDataStoredWithParentChildRelation()
        {
            if (_storage == null)
                throw new ArgumentNullException("_storage", "Storage must be initialized in Setup first.  Perhaps you need a runsettings file.");

            string campaignId = "campaign";
            string locationName = "LocationName";
            string parentLocationName = "Parent";
            string description = "Description";
            string parentDescription = "Parent Description";
            string[] tags = new string[] { "tag1", "tag2" };
            string[] parentTags = new string[] { "tag1", "tag2", "parent" };

            Population population = new Population()
            {
                Populations = new PopulationEntry[]
                {
                    new PopulationEntry()
                    {
                        Count = "100%",
                        Name = "Elves",
                    }
                }
            };

            string parentId = (await _storage.AddLocation(campaignId, parentLocationName, parentDescription, null, population, parentTags)).Id;
            string locationId = (await _storage.AddLocation(campaignId, locationName, description, parentId, population, tags)).Id;

            Location location = await _storage.GetLocation(campaignId, locationId);

            //  First check that the child has a parent
            Assert.IsNotNull(location);
            Assert.AreEqual(locationName, location.Name);
            Assert.AreEqual(description, location.Description);
            CollectionAssert.AreEqual(tags, location.Tags);

            // Check populations
            Assert.IsNotNull(location.Population);
            Assert.IsNotNull(location.Population.Populations);
            Assert.AreEqual(population.Populations[0].Count, location.Population.Populations[0].Count);
            Assert.AreEqual(population.Populations[0].Name, location.Population.Populations[0].Name);

            //  Check child locations
            Assert.IsNotNull(location.ChildLocations);
            Assert.AreEqual(0, location.ChildLocations.Length);

            //  Check parent locations
            Assert.IsNotNull(location.Parent);
            Assert.AreEqual(parentId, location.ParentId);
            Assert.AreEqual(parentId, location.Parent.Id);
            Assert.AreEqual(parentLocationName, location.Parent.Name);
            CollectionAssert.AreEqual(parentTags, location.Parent.Tags);


            //  Now check that the parent has children
            Location parentLocation = await _storage.GetLocation(campaignId, parentId);

            Assert.IsNotNull(parentLocation);
            Assert.AreEqual(parentLocationName, parentLocation.Name);
            Assert.AreEqual(parentDescription, parentLocation.Description);
            CollectionAssert.AreEqual(parentTags, parentLocation.Tags);

            // Check populations
            Assert.IsNotNull(parentLocation.Population);
            Assert.IsNotNull(parentLocation.Population.Populations);
            Assert.AreEqual(population.Populations[0].Count, parentLocation.Population.Populations[0].Count);
            Assert.AreEqual(population.Populations[0].Name, parentLocation.Population.Populations[0].Name);

            //  Check child locations
            Assert.IsNotNull(location.ChildLocations);
            Assert.AreEqual(1, parentLocation.ChildLocations.Length);
            Assert.AreEqual(locationId, parentLocation.ChildLocations[0].Id);
            Assert.AreEqual(locationName, parentLocation.ChildLocations[0].Name);
            CollectionAssert.AreEqual(tags, parentLocation.ChildLocations[0].Tags);

            //  Check parent locations
            Assert.IsNull(parentLocation.Parent);

            await _storage.RemoveLocation(campaignId, locationId, false);
            await _storage.RemoveLocation(campaignId, parentId, false);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task DeleteLocationsReparents(bool moveToParent)
        {
            if (_storage == null)
                throw new ArgumentNullException(nameof(_storage));

            string campaignId = "campaign";

            string parentId = (await _storage.AddLocation(campaignId, "Test", "Test", null, null, Array.Empty<string>())).Id;
            string midId = (await _storage.AddLocation(campaignId, "Test", "Test", parentId, null, Array.Empty<string>())).Id;
            string child1Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).Id;
            string child2Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).Id;

            Location[] updated = (await _storage.RemoveLocation(campaignId, midId, moveToParent))
                .ToArray();

            Location location = await _storage.GetLocation(campaignId, parentId);

            Assert.IsNotNull(location);
            Assert.IsNotNull(location.ChildLocations);
            if (moveToParent)
            {
                Assert.AreEqual(2, location.ChildLocations.Length);

                Assert.AreEqual(child1Id, location.ChildLocations[0].Id);
                Assert.AreEqual(child2Id, location.ChildLocations[1].Id);

                Assert.AreEqual(parentId, updated[0].ParentId);
                Assert.AreEqual(parentId, updated[1].ParentId);
            }
            else
            {
                Assert.AreEqual(0, location.ChildLocations.Length);

                Assert.IsNull(updated[0].ParentId);
                Assert.IsNull(updated[1].ParentId);
            }

            await _storage.RemoveLocation(campaignId, parentId, false);
            await _storage.RemoveLocation(campaignId, midId, false);
            await _storage.RemoveLocation(campaignId, child1Id, false);
            await _storage.RemoveLocation(campaignId, child2Id, false);
        }

        [Test]
        public async Task MoveLocationsReparentsBetweenLocations()
        {
            if (_storage == null)
                throw new ArgumentNullException(nameof(_storage));

            string campaignId = "campaign";

            string parentId = (await _storage.AddLocation(campaignId, "Test", "Test", null, null, Array.Empty<string>())).Id;
            string midId = (await _storage.AddLocation(campaignId, "Test", "Test", parentId, null, Array.Empty<string>())).Id;
            string child1Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).Id;
            string child2Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).Id;

            Location[] updated = (await _storage.MoveLocations(campaignId, parentId, new string[] { child1Id, child2Id }))
                .ToArray();

            //  Get the children that were effected, and the parent that now has all others as children
            Location child1 = await _storage.GetLocation(campaignId, child1Id);
            Location child2 = await _storage.GetLocation(campaignId, child2Id);
            Location parent = await _storage.GetLocation(campaignId, parentId);

            //  Check to make sure first child has proper parent
            Assert.IsNotNull(child1);
            Assert.AreEqual(parentId, child1.ParentId);

            //  Check to make sure second child has proper parent
            Assert.IsNotNull(child2);
            Assert.AreEqual(parentId, child2.ParentId);

            //  Check to make sure the parent gets the children correctly
            Assert.IsNotNull(parent);
            Assert.IsNotNull(parent.ChildLocations);
            Assert.AreEqual(3, parent.ChildLocations.Length);
            string[] childIds = parent.ChildLocations.Select(p => p.Id).ToArray();
            string[] expectedChildIds = new string[] { child1Id, child2Id, midId };

            CollectionAssert.AreEquivalent(expectedChildIds, childIds);

            await _storage.RemoveLocation(campaignId, parentId, false);
            await _storage.RemoveLocation(campaignId, midId, false);
            await _storage.RemoveLocation(campaignId, child1Id, false);
            await _storage.RemoveLocation(campaignId, child2Id, false);
        }

        [Test]
        public async Task MoveLocationsReparentsToRoot()
        {
            if (_storage == null)
                throw new ArgumentNullException(nameof(_storage));

            string campaignId = "campaign";

            string parentId = (await _storage.AddLocation(campaignId, "Test", "Test", null, null, Array.Empty<string>())).Id;
            string midId = (await _storage.AddLocation(campaignId, "Test", "Test", parentId, null, Array.Empty<string>())).Id;
            string child1Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).Id;
            string child2Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).Id;

            Location[] updated = (await _storage.MoveLocations(campaignId, null, new string[] { child1Id, child2Id }))
                .ToArray();

            //  Get the children that were effected, and the parent that now has all others as children
            Location child1 = await _storage.GetLocation(campaignId, child1Id);
            Location child2 = await _storage.GetLocation(campaignId, child2Id);
            Location parent = await _storage.GetLocation(campaignId, parentId);

            //  Check to make sure first child has proper parent
            Assert.IsNotNull(child1);
            Assert.IsNull(child1.ParentId);

            //  Check to make sure second child has proper parent
            Assert.IsNotNull(child2);
            Assert.IsNull(child2.ParentId);

            //  Check to make sure the parent still only has one child
            Assert.IsNotNull(parent);
            Assert.IsNotNull(parent.ChildLocations);
            Assert.AreEqual(1, parent.ChildLocations.Length);
            Assert.AreEqual(midId, parent.ChildLocations[0].Id);

            await _storage.RemoveLocation(campaignId, parentId, false);
            await _storage.RemoveLocation(campaignId, midId, false);
            await _storage.RemoveLocation(campaignId, child1Id, false);
            await _storage.RemoveLocation(campaignId, child2Id, false);
        }

        [Test]
        public async Task GetRootLocationsForCampaign()
        {
            if (_storage == null)
                throw new ArgumentNullException(nameof(_storage));

            Task<Location>[] rootLocationTasks = Enumerable.Range(1, 5)
                .Select(p => _storage.AddLocation("campaign", p.ToString(CultureInfo.InvariantCulture), "Description", null, null, Array.Empty<string>()))
                .ToArray();

            await Task.WhenAll(rootLocationTasks);

            Task<Location>[] childLocationTasks = rootLocationTasks
                .Select(p => _storage.AddLocation("campaign", $"Child {p.Result}", "Description", p.Result.Id, null, Array.Empty<string>()))
                .ToArray();

            await Task.WhenAll(childLocationTasks);

            string[] rootLocationIds = rootLocationTasks.Select(p => p.Result.Id)
                .ToArray();
            string[] childLocationIds = childLocationTasks.Select(p => p.Result.Id)
                .ToArray();

            LocationListItem[] locations = (await _storage.GetRootLocations("campaign"))
                .ToArray();

            string[] locationIds = locations.Select(p => p.Id)
                .ToArray();

            CollectionAssert.AreEquivalent(rootLocationIds, locationIds);

            foreach (string id in rootLocationIds)
                await _storage.RemoveLocation("campaign", id, false);
            foreach (string id in childLocationIds)
                await _storage.RemoveLocation("campaign", id, false);
        }
    }
}