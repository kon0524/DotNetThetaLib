using System;
using System.Net.Sockets;
using System.Text;

namespace DotNetThetaLib
{
	public class CmdDataConnection : Connection
	{
		// フィールド
        private Guid guid = new Guid();
		private const string FRIENDLY_NAME = "TEST";

		// プロパティ
		public UInt32 ConnectionNumber { get; private set; }

		public CmdDataConnection (string ip, int port) : base(ip, port)
		{
            // Init Command Requestを送信
            byte[] data = initCommandRequestData();
            stream.Write(data, 0, data.Length);

            // Init Command Ackを受信
            data = recvAllData();

            // ConnectionNumberを取得する
            ConnectionNumber = BitConverter.ToUInt32(data, 8);
		}

        //public OperationResponse operationRequest(
        //	DataPhaseInfo dpi, OperationCode code, UInt32 tid, 
        //	UInt32 param1 = 0, UInt32 param2 = 0, UInt32 param3 = 0, UInt32 param4 = 0, UInt32 param5 = 0)
        //{
        //	OperationRequest request = new OperationRequest(
        //		dpi, code, tid, param1, param2, param3, param4, param5);
        //	request.send(cmdStream);

        //	OperationResponse response = new OperationResponse();
        //	response.recv(cmdStream);

        //	return response;
        //}

        /// <summary>
        /// InitCommandRequestのデータを作成する
        /// </summary>
        /// <returns>InitCommandRequestのデータ</returns>
        private byte[] initCommandRequestData()
        {
            byte[] friendlyNameBytes = Encoding.Unicode.GetBytes(FRIENDLY_NAME);
            UInt32 length = (UInt32)(4 + 4 + 16 + friendlyNameBytes.Length);
            byte[] data = new byte[length];
            Array.Copy(BitConverter.GetBytes(length), data, 4);
            Array.Copy(BitConverter.GetBytes((UInt32)PacketType.InitCommandRequest), 0, data, 4, 4);
            Array.Copy(guid.ToByteArray(), 0, data, 8, 16);
            Array.Copy(friendlyNameBytes, 0, data, 24, friendlyNameBytes.Length);

            return data;
        }
    }
}

