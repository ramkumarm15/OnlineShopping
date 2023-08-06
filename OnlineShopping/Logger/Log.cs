using Microsoft.OpenApi.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace OnlineShopping.Logger
{
    /// <summary>
    /// Log data to file
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Log
    {
        private const string SourceFolderName = "logs";
        private static readonly string CurrentDirectory = Directory.GetCurrentDirectory();
        private static readonly string DestinationFolderPath = $"{CurrentDirectory}\\{SourceFolderName}";
        private const string FileName = "log_{date}.txt";


        public static void LogWrite(LogLevel logLevel, string logMessage)
        {
            if (!Directory.Exists(DestinationFolderPath))
            {
                Directory.CreateDirectory(DestinationFolderPath);
            }

            string fileNameWithCurrentDate = FileName.Replace("{date}", DateTime.Now.ToString("yyyyMMdd"));

            string fullFilePath = string.Format($"{DestinationFolderPath}/{fileNameWithCurrentDate}");

            string logDetails = string.Format("{0:yyyy-MM-dd HH:ss} [{1}] {2}", DateTime.Now,
                                              logLevel.GetDisplayName(),
                                              logMessage);


            using (StreamWriter writer = new StreamWriter(fullFilePath, true))
            {
                writer.WriteLine(logDetails);
            }


        }
    }
}
