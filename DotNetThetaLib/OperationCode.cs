namespace DotNetThetaLib
{
    public enum OperationCode
	{
		GetDeviceInfo			= 0x1001,
		OpenSession				= 0x1002,
		CloseSession			= 0x1003,
		GetStorageIDs			= 0x1004,
		GetStorageInfo			= 0x1005,
		GetNumObjects			= 0x1006,
		GetObjectHandles		= 0x1007,
		GetObjectInfo			= 0x1008,
		GetObject				= 0x1009,
		GetThumb				= 0x100A,
		DeleteObject			= 0x100B,
		InitiateCapture			= 0x100E,
		GetDevicePropDesc		= 0x1014,
		GetDevicePropValue		= 0x1015,
		SetDevicePropValue		= 0x1016,
		TerminateOpenCapture	= 0x1018,
		InitiateOpenCapture		= 0x101C,
		GetResizedImageObject	= 0x1022,
		WlanPowerControl		= 0x99A1
	}
}

