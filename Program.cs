using System;
using System.Threading.Tasks;

namespace ChatRouletteClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                String name = AskForName();
                using (Client client = new Client(name))
                {
                    Boolean isConnected = await client.ConnectToServer();
                    if (!isConnected)
                        throw new Exception();

                    client.StartReceiveMessagesCycle();
                    client.StartSendMessagesCycle();
                    client.ExceptionHandleCycle();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }

        private static String AskForName()
        {
            Console.WriteLine("Enter your name to start:");
            String name = Console.ReadLine();
            while (name.Length == 0)
            {
                Console.WriteLine("Name length should be 1 or more characters, enter the name again");
                name = Console.ReadLine();
            }
            return name;
        }
    }
}
