# Introduction #

There are a few features that I would like to add to this project in the future. They are as follows:

  * Integration with mysql, Oracle
  * Alternative platforms (Linux)
  * Simple nAnt library for easy deployment and integration with integrations servers.


SkipTransaction

> # Skip transaction #

There are some commands that cannot run in a transaction

```

<sqlRunner index="167"  description="Add full text searching to the system" createDate="2010-09-21" createBy="Rolf Wessels" skipTransaction="True">
 <command>
   <sql>
    /*
     Cannot run in transaction
     */
     CREATE FULLTEXT CATALOG MyCatalog AS DEFAULT;
   </sql>
 </command>
 <rollback>
   <sql>
     /*
     Cannot run in transaction
     */
     DROP FULLTEXT CATALOG MyCatalog ;
   </sql>
 </rollback>
</sqlRunner>

```

Note: This will cause a transaction commit before the update is run and start another transaction once it has been completed.