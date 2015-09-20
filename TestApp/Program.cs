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

            // Open Session
            cmdDataConnection.operationRequest(
                DataPhaseInfo.NoDataOrDataInPhase, OperationCode.OpenSession, 1, 1);

            // Get Object Handles
            cmdDataConnection.operationRequest(
                DataPhaseInfo.NoDataOrDataInPhase, OperationCode.GetObjectHandles, 2, 0xFFFFFFFF, 0xFFFFFFFF);


            cmdDataConnection.close();
            eventConnection.close();
        }
    }
}
