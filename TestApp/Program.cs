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
        private static string APP_HOME = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\THETA_LIB";

        static void Main(string[] args)
        {
            ResponseCode res;

            if (!Directory.Exists(APP_HOME))
            {
                Directory.CreateDirectory(APP_HOME);
            }

            CmdDataConnection cmdDataConnection = 
                new CmdDataConnection(IP, PORT);
            EventConnection eventConnection = 
                new EventConnection(IP, PORT, cmdDataConnection.ConnectionNumber);

            // Open Session
            cmdDataConnection.operationRequest(
                DataPhaseInfo.NoDataOrDataInPhase, OperationCode.OpenSession, 1, 1);

            // SetDevicePropValue
            cmdDataConnection.SendData = BitConverter.GetBytes((Int16)(-2000));
            res = cmdDataConnection.operationRequest(
                DataPhaseInfo.DataOutPhase, OperationCode.SetDevicePropValue, 1, (UInt32)DeviceProperty.ExposureBiasCompensation);

            // InitiateCapture
            res = cmdDataConnection.operationRequest(
                DataPhaseInfo.NoDataOrDataInPhase, OperationCode.InitiateCapture, 1);

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

            // オブジェクトハンドル毎に取得する
            string jpegPath;
            foreach(UInt32 objHndl in objHndls)
            {
                // GetObjectInfo
                res = cmdDataConnection.operationRequest(
                    DataPhaseInfo.NoDataOrDataInPhase, OperationCode.GetObjectInfo, 3, objHndl);
                if (res != ResponseCode.OK) continue;
                ObjectInfo oi = new ObjectInfo(cmdDataConnection.RecvData);
                if (oi.ObjectFormat != ObjectFormats.EXIF_JPEG) continue;

                // すでに画像が取得済みであれば取得しない
                jpegPath = APP_HOME + "\\" + oi.Filename;
                if (File.Exists(jpegPath)) continue;

                // GetObject
                cmdDataConnection.operationRequest(
                    DataPhaseInfo.NoDataOrDataInPhase, OperationCode.GetObject, 3, objHndl);
                FileStream fs = new FileStream(jpegPath, FileMode.Create, FileAccess.Write);
                fs.Write(cmdDataConnection.RecvData, 0, cmdDataConnection.RecvData.Length);
                fs.Close();
            }

            cmdDataConnection.close();
            eventConnection.close();
        }
    }
}
