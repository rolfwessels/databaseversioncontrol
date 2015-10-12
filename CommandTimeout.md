# Command Timeout #

If you find you updates timing out then add the following to you profile sections

```
<repositoryProfile
          type="sqlServer"
          connectionString="data source=Localhost;Integrated Security=True;"
          commandTimeout="300"
          databaseName="DvcTest"
          repositoryID="MainDatabase">
....
```