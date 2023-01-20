using System.Runtime.InteropServices;

namespace RayTracerTest2;
#if DEBUG
public class Debug
{
    /// <summary>
    /// Writes a log inside the console and to the .\log\ folder
    /// </summary>
    /// <param name="message"></param>
    /// <param name="logPrefix"></param>
    /// <param name="comment"></param>
    public static void Log(string message, [Optional, DefaultParameterValue(LogPrefix.NoInfo)] LogPrefix logPrefix,
        [Optional, DefaultParameterValue("No Comment")]
        string comment)
    {
        string directory = @".\log\";
        string convertedDay = (DateTime.Today.ToString("d").Replace(".", "_"));
        string filePath = @".\log\log_" + convertedDay +".txt";

        string msg = $"{DateTime.Now} : {logPrefix}: {message} (Comment: {comment})\n";
        Console.WriteLine(msg);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        if (!File.Exists(filePath))
           File.Create(filePath);

        using (StreamWriter sw = File.AppendText(filePath))
            sw.WriteLine(msg);
    }
}
#endif
/// <summary>
/// The LogPrefix is used to determine the importance of the log message
/// </summary>
public enum LogPrefix

{
    Important,
    Warning,
    Info,
    NoInfo
}