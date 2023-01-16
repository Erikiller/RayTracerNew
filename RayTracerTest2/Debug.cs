using System.Runtime.InteropServices;

namespace RayTracerTest2;
#if DEBUG
class Debug
{
    public static void Log(string message, [Optional, DefaultParameterValue(LogPrefix.NoInfo)] LogPrefix logPrefix,
        [Optional, DefaultParameterValue("No Comment")] string comment)
    {
        DateTime dt = new();
        string msg = $"{dt.TimeOfDay} : {logPrefix}:\n{message}\nComment: {comment}";
        Console.WriteLine(msg);
    }
}
#endif

public enum LogPrefix

{
    Important,
    Warning,
    Info,
    NoInfo
}