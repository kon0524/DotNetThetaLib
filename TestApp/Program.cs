using DotNetThetaLib;

namespace TestApp
{
    class Program
    {
        private static string IP = "192.168.1.1";
        private static int PORT = 15740;

        static void Main(string[] args)
        {
            CmdDataConnection cmdDataConnection = 
                new CmdDataConnection(IP, PORT);
            EventConnection eventConnection = 
                new EventConnection(IP, PORT, cmdDataConnection.ConnectionNumber);

            cmdDataConnection.close();
            eventConnection.close();
        }
    }
}
