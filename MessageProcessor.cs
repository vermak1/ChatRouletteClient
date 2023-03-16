using System;

namespace ChatRouletteClient
{
    internal class MessageProcessor
    {
        public static void VerifyAndPrintMessage(String message)
        {
            if (VerifyIsPing(message) || String.IsNullOrEmpty(message))
                return;
            Console.WriteLine("[{0}]\t{1}", DateTime.Now, message);
        }

        private static Boolean VerifyIsPing(String message)
        {
            if (message == "PingToStartChat")
                return true;
            return false;
        }
    }
}
