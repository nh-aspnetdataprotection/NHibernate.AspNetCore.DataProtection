using System;
using System.Linq;
using System.Xml.Linq;

using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Xunit;

namespace NHibernate.AspNetCore.DataProtection.Test
{
    public class DataProtectionNHibernateTests : IClassFixture<MariaDbFixture>
    {
        private readonly ISessionFactory sessionFactory;
        private readonly NHibernateXmlRepository xmlRepository;

        public DataProtectionNHibernateTests(MariaDbFixture fixture)
        {
            sessionFactory = fixture.BuildSessionFactoryAndCreateSchema();
            xmlRepository = new NHibernateXmlRepository(sessionFactory, NullLoggerFactory.Instance);
        }

        [Fact]
        public void CreateRepository_ThrowsIf_SessionFactory_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new NHibernateXmlRepository(null, null));
        }

        [Fact]
        public void StoreElement_PersistsData()
        {
            var element = XElement.Parse("<Element1/>");
            var friendlyName = "Element1";
            var key = new DataProtectionKey() { FriendlyName = friendlyName, Xml = element.ToString() };

            xmlRepository.StoreElement(element, friendlyName);

            using var session = sessionFactory.OpenSession();
            var dataProtectionKeys = session.Query<DataProtectionKey>().ToList();

            var item = Assert.Single(dataProtectionKeys);
            Assert.Equal(key.FriendlyName, item?.FriendlyName);
            Assert.Equal(key.Xml, item?.Xml);
        }

        [Fact]
        public void GetAllElements_ReturnsAllElements()
        {
            var element1 = XElement.Parse("<Element1/>");
            var element2 = XElement.Parse("<Element2/>");

            xmlRepository.StoreElement(element1, "element1");
            xmlRepository.StoreElement(element2, "element2");

            using var session = sessionFactory.OpenSession();
            var elements = session.Query<DataProtectionKey>().ToList();
            Assert.Equal(2, elements.Count);
        }
        
        [Fact]
        public void PersistKeysToNHibernateSession_Uses_NHibernateXmlRepository()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(sessionFactory);
            serviceCollection.AddDataProtection().PersistKeysToNHibernateSession();
            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);
            var keyManagementOptions = serviceProvider.GetRequiredService<IOptions<KeyManagementOptions>>();
            Assert.IsType<NHibernateXmlRepository>(keyManagementOptions.Value.XmlRepository);
        }
    }
}
