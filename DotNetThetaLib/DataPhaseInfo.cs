namespace DotNetThetaLib
{
    public enum DataPhaseInfo
	{
		UnknownDataPhase		= 0x00000000,
		NoDataOrDataInPhase,    // R -> I
		DataOutPhase            // I -> R
	}
}

