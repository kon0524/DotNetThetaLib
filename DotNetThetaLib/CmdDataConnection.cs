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
        private byte[] recvData = null;

        public byte[] RecvData
        {
            get
            {
                return recvData;
            }
        }



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

        public void operationRequest(
            DataPhaseInfo dpi, OperationCode code, UInt32 tid,
            UInt32 param1 = 0, UInt32 param2 = 0, UInt32 param3 = 0, UInt32 param4 = 0, UInt32 param5 = 0)
        {
            // OperationRequestを送信
            byte[] data = operationRequestData(dpi, code, tid, param1, param2, param3, param4, param5);
            stream.Write(data, 0, data.Length);

            // データフェーズがあれば送信（未実装）

            // データを受信
            data = recvAllData();
            UInt32 length = BitConverter.ToUInt32(data, 0);
            PacketType pt = (PacketType)BitConverter.ToUInt32(data, 4);
            ResponseCode rc;

            // データフェーズがあれば受信する
            if (pt == PacketType.StartData)
            {
                // 全データサイズ
                UInt64 tlen = BitConverter.ToUInt64(data, 12);
                recvData = new byte[tlen];
                int recvDataCount = 0;
                do
                {
                    data = recvAllData();
                    length = BitConverter.ToUInt32(data, 0);
                    pt = (PacketType)BitConverter.ToUInt32(data, 4);
                    Array.Copy(data, 12, recvData, recvDataCount, length - 12);
                    recvDataCount += (int)length - 12;
                }
                while (pt != PacketType.EndData);

                data = recvAllData();
                length = BitConverter.ToUInt32(data, 0);
                pt = (PacketType)BitConverter.ToUInt32(data, 4);
            }
            rc = (ResponseCode)BitConverter.ToUInt16(data, 8);
        }

        private byte[] recvDataPhase()
        {
            byte[] data = recvAllData();
            UInt32 length = BitConverter.ToUInt32(data, 0);
            PacketType pt = (PacketType)BitConverter.ToUInt32(data, 4);

            data = recvAllData();
            length = BitConverter.ToUInt32(data, 0);
            pt = (PacketType)BitConverter.ToUInt32(data, 4);

            return null;
        }

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

        private byte[] operationRequestData(DataPhaseInfo dpi, OperationCode oc, UInt32 tid,
            UInt32 param1 = 0, UInt32 param2 = 0, UInt32 param3 = 0, UInt32 param4 = 0, UInt32 param5 = 0)
        {
            UInt32 length = (UInt32)(4 + 4 + 4 + 2 + 4 + 4 + 4 + 4 + 4 + 4);
            byte[] data = new byte[length];
            Array.Copy(BitConverter.GetBytes(length), data, 4);
            Array.Copy(BitConverter.GetBytes((UInt32)PacketType.OperationRequest), 0, data, 4, 4);
            Array.Copy(BitConverter.GetBytes((UInt32)dpi), 0, data, 8, 4);
            Array.Copy(BitConverter.GetBytes((UInt16)oc), 0, data, 12, 2);
            Array.Copy(BitConverter.GetBytes(tid), 0, data, 14, 4);
            Array.Copy(BitConverter.GetBytes(param1), 0, data, 18, 4);
            Array.Copy(BitConverter.GetBytes(param2), 0, data, 22, 4);
            Array.Copy(BitConverter.GetBytes(param3), 0, data, 26, 4);
            Array.Copy(BitConverter.GetBytes(param4), 0, data, 30, 4);
            Array.Copy(BitConverter.GetBytes(param5), 0, data, 34, 4);

            return data;
        }
    }
}

