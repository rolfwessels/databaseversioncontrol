# Enable logging #

To enable a logging place a file called _loggingSettings.xml_ into the same folder where you _dvc.exe_ is located.

# Logging example #

The logging is done by log4.net so the following example creates a log file called _dvc\_console.log_ with a maximum size of 10MB

```
<?xml version="1.0" encoding="utf-8"?>
<log4net>
  <appender name="RollingFileAppender" type="log4net.Appender.RollingFileAppender">
    <file value="dvc_console.log" />
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
```

See external links for more examples


# External links #

  * Log4net - http://logging.apache.org/log4net/index.html
  * Config Examples - http://logging.apache.org/log4net/release/config-examples.html