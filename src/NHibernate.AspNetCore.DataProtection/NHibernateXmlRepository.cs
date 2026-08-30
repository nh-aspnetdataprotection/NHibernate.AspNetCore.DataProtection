using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;

namespace NHibernate.AspNetCore.DataProtection
{
    /// <summary>
    /// An <see cref="IXmlRepository"/> backed by NHibernate.
    /// </summary>
    public class NHibernateXmlRepository : IXmlRepository
    {
        private readonly ILogger logger;
        private readonly ISessionFactory sessionFactory; 

        /// <summary>
        /// Creates a new instance of the <see cref="NHibernateXmlRepository"/>.
        /// </summary>
        /// <param name="sessionFactory">The <see cref="ISessionFactory"/> to use.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public NHibernateXmlRepository(ISessionFactory sessionFactory, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(loggerFactory);

            logger = loggerFactory.CreateLogger<NHibernateXmlRepository>();
            this.sessionFactory = sessionFactory;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            // forces complete enumeration
            return GetAllElementsCore().ToList().AsReadOnly();

            IEnumerable<XElement> GetAllElementsCore()
            {
                using var session = sessionFactory.OpenStatelessSession();
                using var tx = session.BeginTransaction();
            
                var keys = session.Query<DataProtectionKey>().ToList();

                foreach (var key in keys)
                {
                    logger.ReadingXmlFromKey(key.FriendlyName!, key.Xml);

                    if (!String.IsNullOrEmpty(key.Xml))
                    {
                        yield return XElement.Parse(key.Xml);
                    }
                }
            
                tx.Commit();
            }
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            var newKey = new DataProtectionKey
                         {
                             FriendlyName = friendlyName,
                             Xml = element.ToString(SaveOptions.DisableFormatting)
                         };

            using var session = sessionFactory.OpenStatelessSession();
            using var tx = session.BeginTransaction();
            session.Insert(newKey);
            logger.LogSavingKeyToSession(friendlyName);
            tx.Commit();
        }
    }
}
