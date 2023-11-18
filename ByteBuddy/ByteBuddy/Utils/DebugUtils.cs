namespace ByteBuddy.Utils
{
    public class DebugLogger
    {
#if DEBUG
        public static void Debug(string format, params object[] args)
        {
            SetConsoleColor(ConsoleColor.Blue);
            (string className, string methodName) = GetCallingMethodDetails();
            Console.Write($"[Debug] [{className}.{methodName}] ");
            ResetConsoleColor();
            Console.WriteLine(format, args);
        }

        public static void DebugShort(string format, params object[] args)
        {
            SetConsoleColor(ConsoleColor.Yellow);
            Console.Write($"[Debug] ");
            ResetConsoleColor();
            Console.WriteLine(format, args);
        }

        private static void SetConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        private static void ResetConsoleColor()
        {
            Console.ResetColor();
        }

        private static (string className, string methodName) GetCallingMethodDetails()
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var callingMethod = stackTrace.GetFrame(2).GetMethod();
            var callingType = callingMethod.DeclaringType;

            string className = callingType != null ? callingType.Name : "UnknownClass";
            string methodName = callingMethod != null ? callingMethod.Name : "UnknownMethod";

            return (className, methodName);
        }
#endif
    }
}
