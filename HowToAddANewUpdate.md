# How to add a new update #

Updates are stored in the XML under the _databaseVersion > repositories > repository > updates_ node

The updates look something like this.

```
<sqlRunner index="2"  description="Create Product table" createDate="2010-05-25" createBy="Rolf">
  <command>
    <sql>
      CREATE TABLE [Product] (
      [SellableProductID] bigint NOT NULL,
      [Title] varchar(200) NOT NULL,
      [ArtistName] varchar(200) NOT NULL,
      [ImageName] varchar(200) NOT NULL,
      [Price] Decimal(8,2) NOT NULL,
      [CreateDate] datetime NOT NULL,
      [UpdateDate] datetime NOT NULL
      CONSTRAINT [PK__Product] PRIMARY KEY ([SellableProductID])
      )
      ON [PRIMARY]
    </sql>
  </command>
  <rollback>
    <sql>drop table Product</sql>
  </rollback>
  <testData>
    <sql>insert into Product
      (SellableProductID, Title, ArtistName, ImageName, Price, CreateDate, UpdateDate) VALUES
      ('4', 'Alpha Beta Gaga - Mark Ronson Remix (Instrumental)', 'Air', '1-0724386120458_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),
      ('14', 'Alpha Beta Gaga (Edit 91)', 'Air', '1-0724386120458_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),
      ('30', 'Le Soleil Est Près De Moi', 'Air', '1-0724386188458_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),
      ('38', 'Give My Love To Rose', 'Johnny Cash', '114-0829410977685_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),
      ('62', 'You''re The Nearest Thing To Heaven', 'Johnny Cash', '114-0829410977685_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),
      ('78', 'Raag Bageshree', 'Hari Prasad Chaurasia', '114-0803680556009_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),
      ('123', 'Raga Hameer', 'Ravi Shankar', '114-0803680583807_L_F_W.jpg', '123.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31'),
      ('142', 'Raga Hameer', 'Ravi Shankar', '114-0803680583807_L_F_W.jpg', '1.00', '2010-05-01 14:04:24', '2010-05-01 16:38:31')
    </sql>
  </testData>
</sqlRunner>
```

To add a new update create a new node and add the sections as required.

  * **command** This is mandatory and is the update command that you would like to run.
  * **rollback** This command is optional and allows the command to be rolled back. Useful in cases where a role out has failed and you would like to roll-back the database as well as the code.
  * **testData** For UAT and development machines it is often useful to have test data that can be used for integration testing

Please note that you can also add the update as sql script if it is a long script. Simply replace the sql statement as follows.
 <sql file="01_Update.sql">
}}}}

or if you have multiple files it can be done as follows 

{{{
 <sql files="01_*.sql">
}}}

BUT please ensure that these files remain immutable and that no additional files are added after an update has been done to production. ```