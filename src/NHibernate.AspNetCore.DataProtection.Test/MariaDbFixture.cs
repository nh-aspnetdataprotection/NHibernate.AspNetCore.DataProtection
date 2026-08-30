using System;
using System.Data.Common;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.Data.SqlClient;

using MySqlConnector;

using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Driver.MySqlConnector;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

using Npgsql;

using Testcontainers.MariaDb;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;
using Testcontainers.Xunit;

using Xunit.Sdk;

namespace NHibernate.AspNetCore.DataProtection.Test
{
    public abstract class DatabaseFixture<TBuilderEntity, TContainerEntity, TDriver, TDialect>(IMessageSink messageSink) : DbContainerFixture<TBuilderEntity, TContainerEntity>(messageSink)
        where TDriver : DriverBase
        where TDialect : Dialect.Dialect
        where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity, IContainerConfiguration>, new()
        where TContainerEntity : IDatabaseContainer
    {
        public ISessionFactory BuildSessionFactoryAndCreateSchema()
        {
            var configuration = new Configuration().DataBaseIntegration(c =>
                                                                        {
                                                                            c.Driver<TDriver>();
                                                                            c.Dialect<TDialect>();
                                                                            c.ConnectionString = ConnectionString;
                                                                        });
            configuration.AddDataProtectionKeyMapping();

            new SchemaExport(configuration).Execute(true, true, false);

            return configuration.BuildSessionFactory();
        }
    }

    public class MariaDbFixture(IMessageSink messageSink) : DatabaseFixture<MariaDbBuilder, MariaDbContainer, MySqlConnectorDriver, MySQL8InnoDBDialect>(messageSink)
    {
        protected override MariaDbBuilder Configure() => new MariaDbBuilder("mariadb:11").WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));

        public override DbProviderFactory DbProviderFactory => MySqlConnectorFactory.Instance;
    }

    public class PostgreSqlFixture(IMessageSink messageSink) : DatabaseFixture<PostgreSqlBuilder, PostgreSqlContainer, NpgsqlDriver, PostgreSQL83Dialect>(messageSink)
    {
        protected override PostgreSqlBuilder Configure() => new PostgreSqlBuilder("postgres:15").WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));

        public override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;
    }

    public class MsSqlFixture(IMessageSink messageSink) : DatabaseFixture<MsSqlBuilder, MsSqlContainer, MicrosoftDataSqlClientDriver, MsSql2012Dialect>(messageSink)
    {
        protected override MsSqlBuilder Configure() => new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest").WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));

        public override DbProviderFactory DbProviderFactory => SqlClientFactory.Instance;
    }
}
