using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetThetaLib
{
    public class ObjectInfo
    {
        public UInt32 StorageID { get; private set; }
        public UInt16 ObjectFormat { get; private set; }
        public UInt16 ProtectionStatus { get; private set; }
        public UInt32 ObjectCompressedSize { get; private set; }
        public UInt16 ThumbFormat { get; private set; }
        public UInt32 ThumbCompressedSize { get; private set; }
        public UInt32 ThumbPixWidth { get; private set; }
        public UInt32 ThumbPixHeight { get; private set; }
        public UInt32 ImagePixWidth { get; private set; }
        public UInt32 ImagePixHeight { get; private set; }
        public UInt32 ImageBitDepth { get; private set; }
        public UInt32 ParentObject { get; private set; }
        public UInt16 AssociationType { get; private set; }
        public UInt32 AssociationDescription { get; private set; }
        public UInt32 SequenceNumber { get; private set; }
        public string Filename { get; private set; }
        public string DateCreated { get; private set; }
        public string DateModified { get; private set; }
        public string Keywords { get; private set; }

        public ObjectInfo(byte[] data)
        {
            int index = 52;
            StorageID = BitConverter.ToUInt32(data, 0);
            ObjectFormat = BitConverter.ToUInt16(data, 4);
            ProtectionStatus = BitConverter.ToUInt16(data, 6);
            ObjectCompressedSize = BitConverter.ToUInt32(data, 8);
            ThumbFormat = BitConverter.ToUInt16(data, 12);
            ThumbCompressedSize = BitConverter.ToUInt32(data, 14);
            ThumbPixWidth = BitConverter.ToUInt32(data, 18);
            ThumbPixHeight = BitConverter.ToUInt32(data, 22);
            ImagePixWidth = BitConverter.ToUInt32(data, 26);
            ImagePixHeight = BitConverter.ToUInt32(data, 30);
            ImageBitDepth = BitConverter.ToUInt32(data, 34);
            ParentObject = BitConverter.ToUInt32(data, 38);
            AssociationType = BitConverter.ToUInt16(data, 42);
            AssociationDescription = BitConverter.ToUInt32(data, 44);
            SequenceNumber = BitConverter.ToUInt32(data, 48);
            Filename = toPtpString(data, 52);
            index += Filename.Length * 2 + 1;
            DateCreated = toPtpString(data, index);
            index += DateCreated.Length * 2 + 1;
            DateModified = toPtpString(data, index);
            index += DateModified.Length * 2 + 1;
            Keywords = toPtpString(data, index);
        }

        private string toPtpString(byte[] data, int startIndex)
        {
            int index = 0;
            string ret = "";
            while (true)
            {
                if (index % 2 == 0)
                {
                    index++;
                }
                else
                {
                    if (data[startIndex + index] == 0)
                    {
                        break;
                    }
                    else
                    {
                        ret = ret + BitConverter.ToChar(data, startIndex + index);
                    }
                    index++;
                }
            }
            
            return ret;
        }
    }
}
