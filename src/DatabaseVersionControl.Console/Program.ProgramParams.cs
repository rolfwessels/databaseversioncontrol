using Plossum.CommandLine;

namespace DatabaseVersionControl.Cmd
{
    [CommandLineManager(ApplicationName = "Database version control",
        Copyright = "Common Development and Distribution License (CDDL)", EnabledOptionStyles = OptionStyles.Group | OptionStyles.LongUnix
        )]
    [CommandLineOptionGroup("commands", Name = "Commands", Require = OptionGroupRequirement.AtLeastOne)]
    [CommandLineOptionGroup("options", Name = "Options")]
    public class ProgramParams
    {
        public ProgramParams()
        {
           
        }

        #region Commands

        [CommandLineOption(Name = "h", Aliases = "help",
            Description = "Shows this help text", GroupId = "commands")]
        public bool Help { get; set; }

        [CommandLineOption(Name = "ir", 
            Description = "Initialization rollback", GroupId = "commands")]
        public bool InitializeRollback { get; set; } 

        [CommandLineOption(Name = "u", 
            Description = "update to latest version", GroupId = "commands")]
        public bool Update { get; set; }

        [CommandLineOption(Name = "rb", 
            Description = "rollback to index and save version", GroupId = "commands")]
        public int Rollback { get; set; }

        [CommandLineOption(Name = "run", 
            Description = "Run a specific repository command", GroupId = "commands")]
        public int RunIndex { get; set; }

        [CommandLineOption(Name = "rollback",
            Description = "Rollback a specific repository command", GroupId = "commands")]
        public int RollbackIndex { get; set; }

        [CommandLineOption(Name = "i", 
            Description = "Initialize database", GroupId = "commands")]
        public bool Initialize { get; set; }

        [CommandLineOption(Name = "lt",
            Description = "Load the tracker for profile", GroupId = "commands")]
        public bool LoadTracker{ get; set; }

        [CommandLineOption(Name = "version",
            Description = "Reports the current version", GroupId = "commands")]
        public bool Version { get; set; }

        [CommandLineOption(Name = "setversion",
            Description = "Set the current version in the database", GroupId = "commands")]
        public int SetVersion { get; set; }

        [CommandLineOption(Name = "export",
            Description = "Used to export data from connection string into new dbupdate file", GroupId = "commands")]
        public bool Export { get; set; }


        
        #endregion


        #region Options

        [CommandLineOption(Name = "v", Aliases = "verbose",
            Description = "Produce verbose output", GroupId = "options")]
        public bool Verbose { get; set; }

        [CommandLineOption(Name = "f", 
            Description = "Configuration file name", GroupId = "options")]
        public string UpdateFileName { get; set; }

        [CommandLineOption(Name = "q",
            Description = "Only display execution heading", GroupId = "options")]
        public bool Silent { get; set; }

        [CommandLineOption(Name = "qq",
            Description = "Don't display any output", GroupId = "options")]
        public bool SuperSilent { get; set; }

        [CommandLineOption(Name = "p",
            Description = "Select your profile", GroupId = "options")]
        public string ProfileId;
        
        [CommandLineOption(Name = "td",
            Description = "Include test data", GroupId = "options")]
        public bool IncludeTest;

        [CommandLineOption(Name = "ec",
            Description = "Specify connection string", GroupId = "options")]
        public string ConnectionString;

        [CommandLineOption(Name = "o",
            Description = "Specify output file name", GroupId = "options")]
        public string OutputFileName;

        [CommandLineOption(Name = "es",
            Description = "Specify export table settings file", GroupId = "options")]
        public string ExportSettingsFile;

        [CommandLineOption(Name = "skipSchemaExport",
            Description = "Skip the schema export if the file already exists", GroupId = "options")]
        public bool SkipSchemaExport;

        [CommandLineOption(Name = "c",
            Description = "Commit after every update", GroupId = "options")]
        public bool CommitAfterEveryUpdate;

        #endregion

    }
}