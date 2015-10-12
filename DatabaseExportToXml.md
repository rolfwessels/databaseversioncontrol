# So you have a database that you would like to bring under version control #

If you have a database that is already in production and you would like to bring it under version control then you could use the export tool on the command line to export the database for you.

NOTE: This tool should not be used for making backups !!! Never ever ever ever

# Where to start #

First things first. You will require sqlpubwiz


So first things first you need to setup the location of the SqlPubWiz. If you have installed microsoft sql manager you should already have it under "C:\Program Files\Microsoft SQL Server\90\Tools\Publishing\1.4\SqlPubWiz.exe". If not create a file called _dvc.exe.config_ in the save location as your _dvc.exe_

The file should contains the following

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  
    <configSections>
        <sectionGroup name="applicationSettings" type="System.Configuration.ApplicationSettingsGroup, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" >
            <section name="DatabaseVersionControl.Core.Export.SqlPubWiz.Settings" type="System.Configuration.ClientSettingsSection, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
        </sectionGroup>
      <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
    </configSections>
    <applicationSettings>
        <DatabaseVersionControl.Core.Export.SqlPubWiz.Settings>
            <setting name="Executable" serializeAs="String">
                <value>[PATH HERE]SqlPubWiz.exe</value>
            </setting>
        </DatabaseVersionControl.Core.Export.SqlPubWiz.Settings>
    </applicationSettings>
  <log4net>
    <appender name="RollingFileAppender" type="log4net.Appender.RollingFileAppender">
      <file value="dvclog.log" />
      <appendToFile value="true" />
      <rollingStyle value="Size" />
      <maxSizeRollBackups value="10" />
      <maximumFileSize value="10MB" />
      <staticLogFileName value="true" />
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level [%x] - %message%newline" />
      </layout>
    </appender>
    <root>
      <level value="INFO" />
      <appender-ref ref="RollingFileAppender" />
    </root>
  </log4net>
</configuration>
```

Modify the file as required

# How to export #

Use the command line with --export

Also look at the following options :

--ec                    Specify connection string
--es                    Specify export table settings file
-o                      Specify output file name
--skipSchemaExport      Skip the schema export if the file already exists

## Simple export example ##

So for example if you want to export a database called "MyProductionTable"

```
dvc --export --ec="Data Source=localhost;Initial Catalog=MyProductionTable;Integrated Security=True" --o="Export.xml"
```

The dvc tool will then use SqlPubWiz to generate the database schema. Once that is done it will prompt you with something looking like this

```
Load file data [Data Source=localhost;Initial Catalog=MyProductionTable;Integrated Security=True]
Please select what you would like to do to each tables data
s : Save data in csv export
t : Store data in test data section
t[0-9] : Save limited records as test data eg. T100 will store 100 records in the test data
m : Display more table information
i : Ignore information
Table [AVG_Transaction] [8]
```

What this means it that for every table you will be able to either ignore all the data that is in the table or store the data as a valid update or simply store it as test data. All data will be store in CSV file making it easy and quick to import again.

Once done you should have several files called Export...

## Repeated  Export ##

If you are exporting the data and you expect to be repeating the process. Add the --es option to save your table selections to file so that you don't have to repeat the process. You could also add the --skipSchemaExport if you don't want to override any of exports where the files already exist.

```
dvc --export --ec="Data Source=localhost;Initial Catalog=MyProductionTable;Integrated Security=True" --o="Export.xml" --es="ExportSettings.txt" --skipSchemaExport
```

This means that next time you run the same command you will not be promted for data entry on any of the tables you have already identified.

# Known Problems #

  * had some issues exporting blob files.
  * had some issues with exporting fields with sha excrypted values in them.


....to be continued