using System;
using System.Net.Sockets;

namespace DotNetThetaLib
{
    public class Connection
    {
        protected TcpClient client;
        protected NetworkStream stream;

        public Connection(string ip, int port)
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
        }

        public void close()
        {
            client.Close();
            stream = null;

            client.Close();
            client = null;
        }

        /// <summary>
        /// すべてのデータを受信する
        /// </summary>
        /// <returns>受信データ</returns>
        protected byte[] recvAllData()
        {
            // 全体のサイズを取得する
            byte[] lengthByte = new byte[4];
            stream.Read(lengthByte, 0, lengthByte.Length);
            UInt32 length = BitConverter.ToUInt32(lengthByte, 0);

            // 全データ用の配列にサイズを入れる
            byte[] data = new byte[length];
            Array.Copy(lengthByte, data, lengthByte.Length);

            // 全データ受信する
            stream.Read(data, 4, (int)(length - 4));

            return data;
        }
    }
}
