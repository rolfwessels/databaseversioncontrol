using System;
using System.IO;
using System.Reflection;

namespace DatabaseVersionControl.Cmd.Templates
{
    public class TemplateHelper
    {
        public static string GetFileTemplate(string exportSchemaFileName, string connectionString, string outputFileName, string schemaFileName, string[] additionalUpdates)
        {
            var template = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("DatabaseVersionControl.Cmd.Templates.NewExportTemplate.txt")).ReadToEnd();
            template = template.Replace("%ConnectionString%", connectionString)
                .Replace("%DatabaseName%", Path.GetFileNameWithoutExtension(outputFileName))
                .Replace("%DateTime%", DateTime.Now.ToExactFormatString())
                .Replace("%ExportFileName%", Path.GetFileName(schemaFileName))
                .Replace("%AdditionalUpdates%", string.Join("\n", additionalUpdates));
            return template;
        }

        public static string GetTestDataTemplate(int index, string table, string filename, string tableName)
        {
            var template = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("DatabaseVersionControl.Cmd.Templates.TestSqlTemplate.txt")).ReadToEnd();
            template = template.Replace("%Index%", index.ToString())
                .Replace("%Description%", "Insert test data for table "+table)
                .Replace("%DateTime%", DateTime.Now.ToExactFormatString())
                .Replace("%Filename%", Path.GetFileName(filename))
                .Replace("%TableName%", tableName);
            return template;
        }

        public static string GetCommandDataTemplate(int index, string table, string filename, string tableName)
        {
            var template = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("DatabaseVersionControl.Cmd.Templates.CommandSqlTemplate.txt")).ReadToEnd();
            template = template.Replace("%Index%", index.ToString())
                .Replace("%Description%", "Insert data table "+table)
                .Replace("%DateTime%", DateTime.Now.ToExactFormatString())
                .Replace("%Filename%", Path.GetFileName(filename))
                .Replace("%TableName%", tableName);
            
            return template;
        }
        
    }

    public static class DateTimeEx {
        public static string ToExactFormatString(this DateTime ex)
        {
            return ex.ToString("yyyy-MM-dd");
        }
    }
}