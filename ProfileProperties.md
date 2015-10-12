# Why Profile properties #

Profile properties can be used to change sql at run time based on the profile selected.

# Example #

Here is an example where we would like the developers profile to clear database history but also to set it to single user mode so that all other users get disconnected. We also place property on the _repository_ node to say that the default value for the property is empty

```
<profiles>
  <profile id="Default">
    <repositoryProfile
        type="sqlServer"
        connectionString="data source=Localhost;Integrated Security=True;"
        databaseName="DvcTest"
        repositoryID="MainDatabase">
      <trackingByTable tableName="DVC_Tracking" autoCreate="True" />
    </repositoryProfile>
    </profile>
    <profile id="ProfileDeveloper">
        <repositoryProfile
            type="sqlServer"
            connectionString="data source=Localhost;Integrated Security=True;"
            databaseName="DvcTest"
            repositoryID="MainDatabase">
          <trackingByTable tableName="DVC_Tracking" autoCreate="True" />
          <properties>
            <property name="Pre.InitialRunner.Rollback">
              EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = '${dp.DatabaseName};'
              ALTER DATABASE [${dp.DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            </property>
          </properties>
        </repositoryProfile>
    </profile>
</profiles>
<repositories>
  <repository id="MainDatabase">
    <properties>
      <property name="Pre.InitialRunner.Rollback" value="" />
    </properties>
    <initialRunner>
      <sqlRunner index="1"  createDate="2010-04-16" description="Initialize script" createBy="Rolf Wessels">
        <command>
          <sql>create database ${dp.DatabaseName}</sql>
        </command>
        <rollback>
          <sql>
          ${Pre.InitialRunner.Rollback}
          drop database ${dp.DatabaseName}</sql>
        </rollback>
      </sqlRunner>
    </initialRunner>

```