
```
> dvc.bat -f=Sample1.xml -iu -q
ExecuteCommand - Initialize script [1 - Rolf Wessels 2010-04-16]
Initialize tracker - Initialize script [1 - Rolf Wessels 2010-04-16]
ExecuteCommand - Create Product table [2 - Rolf 2010-05-25]
ExecuteCommand - Additional Customer table required [3 - Rolf 2010-05-27]
ExecuteCommand - Additional Voucher table required [4 - Rolf 2010-05-27]
ExecuteCommand - Additional Order table required [5 - Rolf 2010-05-27]
ExecuteCommand - Additional OrderItem required [6 - Rolf 2010-05-27]
ExecuteCommand - Create index [7 - Rolf 2010-05-27]
Database up create and up to date
```

# What does it do? #

With dvc you can maintain a your database structure in a simple XML along side your standard source repository. Dvc allows you to easily create and maintain database update information. HowDoesItWork

# Why ? #

I find that I am always making changes to my development database and sometimes not maintaining a clear list of required update and rollbacks for products.

# How to maintain ? #

I store a simple xml file with all the connection profiles.
```
  <profile id="Default">
      <repositoryProfile
          type="sqlServer"
          connectionString="data source=Localhost;Integrated Security=True;"
          databaseName="DvcTest"
          repositoryID="MainDatabase">
        <trackingByTable tableName="DVC_Tracking" autoCreate="True" />
      </repositoryProfile>
  </profile>
```

I then maintain a repository of all statements in the same or external xml files

```
<sqlRunner 
  index="1" 
  createDate="2010-04-16" 
  description="Initialize script" 
  createBy="Rolf Wessels">
    <command>
      <sql>create database ${dp.DatabaseName}</sql>
    </command>
    <rollback>
      <sql>drop database ${dp.DatabaseName}</sql>
    </rollback>
</sqlRunner>

```

  * HowToAddANewUpdate



# How to execute? #

using the dvc.exe I can initialize a new database with tracking table
```
> dvc.bat -f=Sample1.xml --i
ExecuteCommand - Initialize script [1 - Rolf Wessels 2010-04-16]
Sql: create database DvcTest
Sql: use DvcTest
Initialize tracker - Initialize script [1 - Rolf Wessels 2010-04-16]
Sql: CREATE TABLE DVC_Tracking (...
```

I can run a update which will only runs newly added commands

```
>dvc.bat -f=Sample1.xml
ExecuteCommand - Create Product table [2 - Rolf 2010-05-25]
ExecuteCommand - Additional Customer table required [3 - Rolf 2010-05-27]
ExecuteCommand - Additional Voucher table required [4 - Rolf 2010-05-27]
ExecuteCommand - Additional Order table required [5 - Rolf 2010-05-27]
ExecuteCommand - Additional OrderItem required [6 - Rolf 2010-05-27]
ExecuteCommand - Create index [7 - Rolf 2010-05-27]
```

I also have the ability to rollback to a selected repository index
```
>dvc.bat -f=Sample1.xml --rb=3 -q
ExecuteRollback - Additional OrderItem required [6 - Rolf 2010-05-27]
ExecuteRollback - Additional Voucher table required [4 - Rolf 2010-05-27]
ExecuteRollback - Additional Customer table required [3 - Rolf 2010-05-27]
```

# Features #

  * Allows schema changes
  * Allows value tree population
  * Allows separate test data inserts for eg. UAT
  * Allows option for rollback (not automated)
  * Maintains versioning on both SQL server and Mysql
  * Has the ability to import your existing database into version control with one simple command



# Other #
[ProjectFuture](ProjectFuture.md)