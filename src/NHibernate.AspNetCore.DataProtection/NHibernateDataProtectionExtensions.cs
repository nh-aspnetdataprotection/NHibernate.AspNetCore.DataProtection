using System;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace NHibernate.AspNetCore.DataProtection
{
    /// <summary>
    /// Extension method class for configuring instances of <see cref="NHibernateXmlRepository"/>
    /// </summary>
    public static class NHibernateDataProtectionExtensions
    {
        /// <summary>
        /// Configures the data protection system to persist keys to an NHibernate session.
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder"/> instance to modify.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IDataProtectionBuilder PersistKeysToNHibernateSession(this IDataProtectionBuilder builder)
        {
            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
                                                                                   {
                                                                                       var loggerFactory = services.GetService<Microsoft.Extensions.Logging.ILoggerFactory>() ?? NullLoggerFactory.Instance;
                                                                                       var sessionFactory = services.GetRequiredService<ISessionFactory>();
                                                                                       return new ConfigureOptions<KeyManagementOptions>(options => { options.XmlRepository = new NHibernateXmlRepository(sessionFactory, loggerFactory); });
                                                                                   });

            return builder;
        }

        /// <summary>
        /// Adds the mapping for <see cref="DataProtectionKey"/> to the given <paramref name="configuration"/> instance.
        /// </summary>
        /// <param name="configuration">The instance to add the mapping to.</param>
        /// <param name="customizer">An optional delegate to customize the <see cref="ModelMapper"/>.</param>
        /// <returns></returns>
        public static Configuration AddDataProtectionKeyMapping(this Configuration configuration, Action<ModelMapper>? customizer = null)
        {
            var mapper = new ModelMapper();
            mapper.AddDataProtectionKeyMapping();
            customizer?.Invoke(mapper);
            configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
            return configuration;
        }

        /// <summary>
        /// Adds the mapping for <see cref="DataProtectionKey"/> to the given <paramref name="mapper"/>  instance.
        /// </summary>
        /// <param name="mapper">The instance to add the mapping to.</param>
        public static void AddDataProtectionKeyMapping(this ModelMapper mapper)
        {
            //Mapping based on https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-10.0&tabs=visual-studio#entity-framework-core
            mapper.Class<DataProtectionKey>(classMapper =>
                                            {
                                                classMapper.Lazy(false);
                                                classMapper.Id(x => x.Id, idMapper => idMapper.Generator(Generators.Identity));
                                                classMapper.Property(x => x.FriendlyName, propertyMapper => propertyMapper.Length(Int16.MaxValue - 1));
                                                classMapper.Property(x => x.Xml, propertyMapper => propertyMapper.Length(Int16.MaxValue - 1));
                                            });
        }
    }
}
