namespace DotNetThetaLib
{
    public enum PacketType
    {
        InvalidValue = 0x00000000,
        InitCommandRequest,
        InitCommandAck,
        InitEventRequest,
        InitEventAck,
        InitFail,
        OperationRequest,
        OperationResponse,
        Event,
        StartData,
        Data,
        Cancel,
        EndData,
        ProbeRequest,
        ProbeResponse
    }
}
