namespace Mnk.TBox.Plugins.RequestsWatcher.Code.Parser
{
	enum RequestState
	{
		Idle,
		Created,
		SendBegin,
		SendEnd,
		ReceiveBegin,
		ReceiveEnd,
		Dispose
	}
}
