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

            string campaignId = (await _storage.CreateCampaign("Me", "Campaign", "Location Test Campaign"));
            
            string locationId = (await _storage.AddLocation(campaignId, locationName, description, null, population, tags)).ID;

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
            Assert.IsNotNull(location.ParentsPath);
            Assert.AreEqual(0, location.ParentsPath.Length);

            //  Cleanup
            await _storage.RemoveLocation(campaignId, locationId, HandleChildren.MoveToRoot);
            await _storage.DeleteCampaign(campaignId);
        }


        [Test]
        public async Task LocationDataStoredWithParentChildRelation()
        {
            if (_storage == null)
                throw new ArgumentNullException("_storage", "Storage must be initialized in Setup first.  Perhaps you need a runsettings file.");

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

            string campaignId = (await _storage.CreateCampaign("Me", "Campaign", "Location Test Campaign"));

            string parentId = (await _storage.AddLocation(campaignId, parentLocationName, parentDescription, null, population, parentTags)).ID;
            string locationId = (await _storage.AddLocation(campaignId, locationName, description, parentId, population, tags)).ID;

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
            Assert.IsNotNull(location.ParentsPath);
            Assert.AreEqual(1, location.ParentsPath.Length);
            Assert.AreEqual(parentId, location.ParentId);
            Assert.AreEqual(parentId, location.ParentsPath[0].ID);
            Assert.AreEqual(parentLocationName, location.ParentsPath[0].Name);
            CollectionAssert.AreEqual(parentTags, location.ParentsPath[0].Tags);


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
            Assert.AreEqual(locationId, parentLocation.ChildLocations[0].ID);
            Assert.AreEqual(locationName, parentLocation.ChildLocations[0].Name);
            CollectionAssert.AreEqual(tags, parentLocation.ChildLocations[0].Tags);

            //  Check parent locations
            Assert.IsNotNull(parentLocation.ParentsPath);
            Assert.AreEqual(0, parentLocation.ParentsPath.Length);

            await _storage.RemoveLocation(campaignId, locationId, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, parentId, HandleChildren.MoveToRoot);
            await _storage.DeleteCampaign(campaignId);
        }

        [TestCase(HandleChildren.MoveToParent)]
        [TestCase(HandleChildren.MoveToRoot)]
        public async Task DeleteLocationReparents(HandleChildren childrenHandling)
        {
            if (_storage == null)
                throw new ArgumentNullException(nameof(_storage));

            string campaignId = (await _storage.CreateCampaign("Me", "Campaign", "Location Test Campaign"));

            string parentId = (await _storage.AddLocation(campaignId, "Test", "Test", null, null, Array.Empty<string>())).ID;
            string midId = (await _storage.AddLocation(campaignId, "Test", "Test", parentId, null, Array.Empty<string>())).ID;
            string child1Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).ID;
            string child2Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).ID;

            Location[] updated = (await _storage.RemoveLocation(campaignId, midId, childrenHandling))
                .ToArray();

            Location location = await _storage.GetLocation(campaignId, parentId);

            Assert.IsNotNull(location);
            Assert.IsNotNull(location.ChildLocations);
            if (childrenHandling == HandleChildren.MoveToParent)
            {
                Assert.AreEqual(2, location.ChildLocations.Length);

                Assert.AreEqual(child1Id, location.ChildLocations[0].ID);
                Assert.AreEqual(child2Id, location.ChildLocations[1].ID);

                Assert.AreEqual(parentId, updated[0].ParentId);
                Assert.AreEqual(parentId, updated[1].ParentId);
            }
            else
            {
                Assert.AreEqual(0, location.ChildLocations.Length);

                Assert.IsNull(updated[0].ParentId);
                Assert.IsNull(updated[1].ParentId);
            }

            await _storage.RemoveLocation(campaignId, parentId, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, midId, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, child1Id, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, child2Id, HandleChildren.MoveToRoot);
            await _storage.DeleteCampaign(campaignId);
        }

        [Test]
        public async Task DeleteLocationDeletesChildren()
        {
            if (_storage == null)
                throw new ArgumentNullException(nameof(_storage));

            string campaignId = (await _storage.CreateCampaign("Me", "Campaign", "Location Test Campaign"));

            string parentId = (await _storage.AddLocation(campaignId, "Test", "Test", null, null, Array.Empty<string>())).ID;
            string midId = (await _storage.AddLocation(campaignId, "Test", "Test", parentId, null, Array.Empty<string>())).ID;
            string child1Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).ID;
            string child2Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).ID;

            Location[] updated = (await _storage.RemoveLocation(campaignId, midId, HandleChildren.Delete))
                .ToArray();

            Location location = await _storage.GetLocation(campaignId, parentId);
            LocationListItem[] locations = (await _storage.GetRootLocations(campaignId)).ToArray();

            Assert.IsNotNull(location);
            Assert.IsNotNull(location.ChildLocations);
            Assert.AreEqual(0, location.ChildLocations.Length);
            Assert.AreEqual(1, locations.Length);

            Assert.ThrowsAsync<ItemNotFoundException>(async () => await _storage.GetLocation(campaignId, child1Id));
            Assert.ThrowsAsync<ItemNotFoundException>(async () => await _storage.GetLocation(campaignId, child2Id));

            await _storage.RemoveLocation(campaignId, parentId, HandleChildren.MoveToRoot);
            await _storage.DeleteCampaign(campaignId);
        }

        [Test]
        public async Task MoveLocationsReparentsBetweenLocations()
        {
            if (_storage == null)
                throw new ArgumentNullException(nameof(_storage));

            string campaignId = (await _storage.CreateCampaign("Me", "Campaign", "Location Test Campaign"));

            string parentId = (await _storage.AddLocation(campaignId, "Test", "Test", null, null, Array.Empty<string>())).ID;
            string midId = (await _storage.AddLocation(campaignId, "Test", "Test", parentId, null, Array.Empty<string>())).ID;
            string child1Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).ID;
            string child2Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).ID;

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
            string[] childIds = parent.ChildLocations.Select(p => p.ID).ToArray();
            string[] expectedChildIds = new string[] { child1Id, child2Id, midId };

            CollectionAssert.AreEquivalent(expectedChildIds, childIds);

            await _storage.RemoveLocation(campaignId, parentId, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, midId, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, child1Id, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, child2Id, HandleChildren.MoveToRoot);
            await _storage.DeleteCampaign(campaignId);
        }

        [Test]
        public async Task MoveLocationsReparentsToRoot()
        {
            if (_storage == null)
                throw new ArgumentNullException(nameof(_storage));

            string campaignId = (await _storage.CreateCampaign("Me", "Campaign", "Location Test Campaign"));

            string parentId = (await _storage.AddLocation(campaignId, "Test", "Test", null, null, Array.Empty<string>())).ID;
            string midId = (await _storage.AddLocation(campaignId, "Test", "Test", parentId, null, Array.Empty<string>())).ID;
            string child1Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).ID;
            string child2Id = (await _storage.AddLocation(campaignId, "Test", "Test", midId, null, Array.Empty<string>())).ID;

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
            Assert.AreEqual(midId, parent.ChildLocations[0].ID);

            await _storage.RemoveLocation(campaignId, parentId, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, midId, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, child1Id, HandleChildren.MoveToRoot);
            await _storage.RemoveLocation(campaignId, child2Id, HandleChildren.MoveToRoot);
            await _storage.DeleteCampaign(campaignId);
        }

        [Test]
        public async Task GetRootLocationsForCampaign()
        {
            if (_storage == null)
                throw new ArgumentNullException(nameof(_storage));

            string campaignId = (await _storage.CreateCampaign("Me", "Campaign", "Location Test Campaign"));

            Task<Location>[] rootLocationTasks = Enumerable.Range(1, 5)
                .Select(p => _storage.AddLocation(campaignId, p.ToString(CultureInfo.InvariantCulture), "Description", null, null, Array.Empty<string>()))
                .ToArray();

            await Task.WhenAll(rootLocationTasks);

            Task<Location>[] childLocationTasks = rootLocationTasks
                .Select(p => _storage.AddLocation(campaignId, $"Child {p.Result}", "Description", p.Result.ID, null, Array.Empty<string>()))
                .ToArray();

            await Task.WhenAll(childLocationTasks);

            string[] rootLocationIds = rootLocationTasks.Select(p => p.Result.ID)
                .ToArray();
            string[] childLocationIds = childLocationTasks.Select(p => p.Result.ID)
                .ToArray();

            LocationListItem[] locations = (await _storage.GetRootLocations(campaignId))
                .ToArray();

            string[] locationIds = locations.Select(p => p.ID)
                .ToArray();

            CollectionAssert.AreEquivalent(rootLocationIds, locationIds);

            foreach (string id in rootLocationIds)
                await _storage.RemoveLocation(campaignId, id, HandleChildren.MoveToRoot);
            foreach (string id in childLocationIds)
                await _storage.RemoveLocation(campaignId, id, HandleChildren.MoveToRoot);
            await _storage.DeleteCampaign(campaignId);
        }
    }
}