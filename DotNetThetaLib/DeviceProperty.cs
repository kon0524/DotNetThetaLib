namespace DotNetThetaLib
{
    public enum DeviceProperty
    {
        BatteryLevel = 0x5001,
        WhiteBalance = 0x5005,
        ExposureIndex = 0x500F,
        ExposureBiasCompensation = 0x5010,
        DateTime = 0x5011,
        StillCaptureMode = 0x5013,
        TimelapseNumber = 0x501A,
        TimelapseInterval = 0x501B,
        AudioVolume = 0x502C,
        ErrorInfo = 0xD006,
        ShutterSpeed = 0xD00F,
        GpsInfo = 0xD801,
        AutoPowerOffDelay = 0xD802,
        SleepDelay = 0xD803,
        ChannelNumber = 0xD807,
        CaptureStatus = 0xD808,
        RecordingTime = 0xD809,
        RemainingRecordingTime = 0xD80A
    }
}
