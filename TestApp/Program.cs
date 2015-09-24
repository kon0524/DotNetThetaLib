using DotNetThetaLib;
using System;
using System.IO;
using System.Collections.Generic;

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
            List<UInt32> objHndls = new List<UInt32>();
            cmdDataConnection.operationRequest(
                DataPhaseInfo.NoDataOrDataInPhase, OperationCode.GetObjectHandles, 2, 0xFFFFFFFF, 0xFFFFFFFF);
            byte[] data = cmdDataConnection.RecvData;
            if (data != null)
            {
                for (int offset = 0; offset < data.Length; offset += 4)
                {
                    objHndls.Add(BitConverter.ToUInt32(data, offset));
                }
            }

            // GetObjectInfo
            cmdDataConnection.operationRequest(
                DataPhaseInfo.NoDataOrDataInPhase, OperationCode.GetObjectInfo, 3, objHndls[objHndls.Count - 1]);
            ObjectInfo oi = new ObjectInfo(cmdDataConnection.RecvData);

            // GetObject
            cmdDataConnection.operationRequest(
                DataPhaseInfo.NoDataOrDataInPhase, OperationCode.GetObject, 3, objHndls[objHndls.Count - 1]);
            FileStream fs = new FileStream(@"C:\Users\kouta\Desktop\test.jpg", FileMode.Create, FileAccess.Write);
            fs.Write(cmdDataConnection.RecvData, 0, cmdDataConnection.RecvData.Length);
            fs.Close();

            cmdDataConnection.close();
            eventConnection.close();
        }
    }
}
