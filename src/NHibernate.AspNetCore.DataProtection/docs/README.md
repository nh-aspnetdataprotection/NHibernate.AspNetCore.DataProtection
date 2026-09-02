## About

Provides a mechanism for storing data protection keys to a database using NHibernate. Based on [Microsoft.AspNetCore.DataProtection.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.AspNetCore.DataProtection.EntityFrameworkCore/).

## How to Use

### Installation

`dotnet add package NHibernate.AspNetCore.DataProtection`

## Configuration

To store keys in a database, use the PersistKeysToNHibernateSession extension method. For example:

```
builder.Services.AddDataProtection().
                 PersistKeysToNHibernateSession();
```

The included `DataProtectionKey` class needs to be added to your NHibernate `Configuration` either by mapping it yourself or by using one of the provided extension methods. For example:

```
var configuration = new Configuration().DataBaseIntegration(c =>
                                                            {
                                                                c.Driver<MySqlConnectorDriver>();
                                                                c.Dialect<MySQL8InnoDBDialect>();
                                                                c.ConnectionString = ConnectionString;
                                                            });
configuration.AddDataProtectionKeyMapping();
```
You can also use your existing `ModelMapper` instance if you are already using NHibernate's mapping by code:

```
var configuration = new Configuration()

var mapper = new ConventionModelMapper();
mapper.AddDataProtectionKeyMapping();

configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
```
