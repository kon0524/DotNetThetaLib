using System;
using System.Net.Sockets;

namespace DotNetThetaLib
{
	public class EventConnection : Connection
	{
        private UInt32 connectionNumber;

		public EventConnection (string ip, int port, UInt32 connectionNumber) : base(ip, port)
		{
            this.connectionNumber = connectionNumber;

            // Init Event Commandを送信
            byte[] data = initEventRequestData();
            stream.Write(data, 0, data.Length);

            // Init Event Ackを受信
            data = recvAllData();
		}

        /// <summary>
        /// initEventRequestのデータを作成する
        /// </summary>
        /// <returns>initEventRequestのデータ</returns>
        private byte[] initEventRequestData()
        {
            UInt32 length = (UInt32)(4 + 4 + 4);
            byte[] data = new byte[length];
            Array.Copy(BitConverter.GetBytes(length), data, 4);
            Array.Copy(BitConverter.GetBytes((UInt32)PacketType.InitEventRequest), 0, data, 4, 4);
            Array.Copy(BitConverter.GetBytes(connectionNumber), 0, data, 8, 4);

            return data;
        }
    }
}

