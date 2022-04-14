using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Mnk.TBox.Plugins.RequestsWatcher.Code.Parser
{
    class RequestEntry
    {
        private int pid;
        private int socket;
        private RequestState state = RequestState.Idle;
        private readonly Request request = new Request();
        public bool Process(string line)
        {
            switch (state)
            {
                case RequestState.Idle:
                    return OnIdle(line);
                case RequestState.Created:
                    return OnCreated(line);
                case RequestState.SendBegin:
                    return OnSendBegin(line);
                case RequestState.SendEnd:
                    return OnSendEnd(line);
                case RequestState.ReceiveBegin:
                    return OnReceiveBegin(line);
                case RequestState.ReceiveEnd:
                    return OnReceiveEnd(line);
                default:
                    throw new ArgumentException("Invalid state: " + state);
            }
        }

        private bool OnIdle(string line)
        {
            var m = Constants.RegCreated.Match(line);
            if (!m.Success) return false;
            var info = GetPidAndSocket(m);
            pid = info.Item1;
            socket = info.Item2;
            request.To = m.Groups[Constants.Destination].Value;
            state = RequestState.Created;
            return true;
        }

        private bool OnCreated(string line)
        {
            var m = Constants.RegSendBegin.Match(line);
            if (!m.Success) return false;
            var info = GetPidAndSocket(m);
            if (info.Item1 != pid || info.Item2 != socket) return false;
            state = RequestState.SendBegin;
            return true;
        }

        private bool OnSendBegin(string line)
        {
            var m = Constants.RegData.Match(line);
            if (!m.Success)
            {
                var m2 = Constants.RegSendEnd.Match(line);
                if (m2.Success)
                {
                    var i = GetPidAndSocket(m2);
                    if (i.Item1 != pid || i.Item2 != socket) return false;
                    state = RequestState.SendEnd;
                    return true;
                }
                return false;
            }
            if (int.Parse(m.Groups[Constants.Pid].Value) != pid) return false;
            request.Send += ParseText(m.Groups[Constants.Text].Value);
            return true;
        }

        private bool OnSendEnd(string line)
        {
            if (OnCreated(line)) return true;
            if (!TryBeginReceive(line)) return false;
            if (string.IsNullOrEmpty(request.Title))
            {
                var id = request.Send.IndexOf(" HTTP/", StringComparison.Ordinal);
                request.Title = id >= 0 ? WebUtility.UrlDecode(request.Send.Substring(0, id)) : request.Send;
            }
            return true;
        }

        private bool TryBeginReceive(string line)
        {
            var m = Constants.RegReceiveBegin.Match(line);
            if (!m.Success) return false;
            var info = GetPidAndSocket(m);
            if (info.Item1 != pid || info.Item2 != socket) return false;
            state = RequestState.ReceiveBegin;
            return true;
        }

        private bool OnReceiveBegin(string line)
        {
            var m = Constants.RegData.Match(line);
            if (!m.Success)
            {
                var m2 = Constants.RegReceiveEnd.Match(line);
                if (m2.Success)
                {
                    var i = GetPidAndSocket(m2);
                    if (i.Item1 != pid || i.Item2 != socket) return false;
                    state = RequestState.ReceiveEnd;
                    return true;
                }
                return false;
            }
            if (int.Parse(m.Groups[Constants.Pid].Value) != pid) return false;
            request.Receive += ParseText(m.Groups[Constants.Text].Value);
            return true;
        }

        private bool OnReceiveEnd(string line)
        {
            if (TryBeginReceive(line)) return true;
            var m = Constants.RegDispose.Match(line);
            if (!m.Success) return false;
            var info = GetPidAndSocket(m);
            if (info.Item1 != pid || info.Item2 != socket) return false;
            state = RequestState.Dispose;
            request.Title += Environment.NewLine + ParseResponseStatus();
            return true;
        }

        private string ParseResponseStatus()
        {
            var id = request.Receive.IndexOf(Environment.NewLine, StringComparison.Ordinal);
            if (id < 0) return string.Empty;
            var first = request.Receive.IndexOf(' ');
            if (first < 0) first = 0;
            return request.Receive.Substring(first, id - first);
        }

        private static Tuple<int, int> GetPidAndSocket(Match m)
        {
            return new Tuple<int, int>(
                int.Parse(m.Groups[Constants.Pid].Value),
                int.Parse(m.Groups[Constants.Socket].Value)
                );
        }

        private string ParseText(string value)
        {
            return Encoding.UTF8.GetString(
                value
                .Replace("-", " ")
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (byte)int.Parse(x, NumberStyles.HexNumber))
                .ToArray());
        }

        public bool TryGetResult(out Request result)
        {
            if (state == RequestState.Dispose)
            {
                result = (Request)request.Clone();
                return true;
            }
            result = null;
            return false;
        }

        public bool Idle => state == RequestState.Idle;
    }
}
