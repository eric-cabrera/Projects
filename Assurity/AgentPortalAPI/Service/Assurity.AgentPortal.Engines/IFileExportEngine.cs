namespace Assurity.AgentPortal.Engines;

public interface IFileExportEngine
{
    /// <summary>
    /// Returns a list of strings that can be used as headers when using the CreateExxcelDocument method.
    /// The type must have the DisplayName attribute for each member that should be used as a header.
    /// </summary>
    /// <typeparam name="T">The type to use when generating the headers.</typeparam>
    /// <returns></returns>
    List<string> CreateHeaders<T>();

    /// <summary>
    /// Creates an formatted Excel document.
    /// </summary>
    /// <typeparam name="T">The type that will be used to generate the Excel document.</typeparam>
    /// <param name="headers">A list of strings that will be added to the top of the Excel document as headers.</param>
    /// <param name="data">A list of objects that will be used to generate the data cells.</param>
    /// <param name="sheetName">An optional name of the sheet in the Excel document.</param>
    /// <returns></returns>
    byte[] CreateExcelDocument<T>(List<string> headers, List<T> data, string sheetName = "data");
}