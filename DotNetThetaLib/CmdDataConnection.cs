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
        private byte[] sendData = null;

        public byte[] RecvData
        {
            get
            {
                return recvData;
            }
        }

        public byte[] SendData
        {
            get
            {
                return sendData;
            }
            set
            {
                sendData = value;
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

        public ResponseCode operationRequest(
            DataPhaseInfo dpi, OperationCode code, UInt32 tid,
            UInt32 param1 = 0, UInt32 param2 = 0, UInt32 param3 = 0, UInt32 param4 = 0, UInt32 param5 = 0)
        {
            // OperationRequestを送信
            byte[] data = operationRequestData(dpi, code, tid, param1, param2, param3, param4, param5);
            stream.Write(data, 0, data.Length);

            // データフェーズがあれば送信（未実装）
            if (dpi == DataPhaseInfo.DataOutPhase)
            {
                sendDataPhase(tid);
            }

            // データを受信
            data = recvAllData();
            UInt32 length = BitConverter.ToUInt32(data, 0);
            PacketType pt = (PacketType)BitConverter.ToUInt32(data, 4);

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

            return (ResponseCode)BitConverter.ToUInt16(data, 8);
        }

        /// <summary>
        /// DataOutPhaseを実行する
        /// </summary>
        /// <param name="tid"></param>
        private void sendDataPhase(UInt32 tid)
        {
            byte[] data = startData(tid);
            stream.Write(data, 0, data.Length);

            data = endData(tid, 0);
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// DataInPhaseを実行する
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// OperationRequestのデータを作成する
        /// </summary>
        /// <param name="dpi"></param>
        /// <param name="oc"></param>
        /// <param name="tid"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="param4"></param>
        /// <param name="param5"></param>
        /// <returns></returns>
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

        /// <summary>
        /// StartDataのデータを作成する
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        private byte[] startData(UInt32 tid)
        {
            UInt32 length = (UInt32)(4 + 4 + 4 + 8);
            byte[] data = new byte[length];
            Array.Copy(BitConverter.GetBytes(length), data, 4);
            Array.Copy(BitConverter.GetBytes((UInt32)PacketType.StartData), 0, data, 4, 4);
            Array.Copy(BitConverter.GetBytes(tid), 0, data, 8, 4);
            Array.Copy(BitConverter.GetBytes((UInt64)sendData.Length), 0, data, 12, 8);

            return data;
        }

        /// <summary>
        /// EndDataのデータを作成する
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private byte[] endData(UInt32 tid, int startIndex)
        {
            UInt32 length = (UInt32)(4 + 4 + 4 + sendData.Length - startIndex);
            byte[] data = new byte[length];
            Array.Copy(BitConverter.GetBytes(length), data, 4);
            Array.Copy(BitConverter.GetBytes((UInt32)PacketType.EndData), 0, data, 4, 4);
            Array.Copy(BitConverter.GetBytes(tid), 0, data, 8, 4);
            Array.Copy(sendData, startIndex, data, 12, (sendData.Length - startIndex));

            return data;
        }
    }
}

