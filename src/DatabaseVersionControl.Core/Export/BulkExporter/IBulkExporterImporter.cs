namespace DatabaseVersionControl.Core.Export.BulkExporter
{
    /// <summary>
    /// Used to bulkexport sql tables
    /// </summary>
    public interface IBulkExporterImporter
    {
        /// <summary>
        /// Exports a table to selected filename
        /// </summary>
        /// <param name="table"></param>
        /// <param name="filenameSql"></param>
        void ExportTable(string table, string filenameSql);
    }
}