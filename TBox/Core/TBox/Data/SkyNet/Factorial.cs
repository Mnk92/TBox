using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.Library.ScriptEngine;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.Library.WpfControls.Tools;
using Mnk.Library.Common.Tools;
using System.Numerics;
using ServiceStack.Text;

public class Factorial : ISkyScript
{
    [Ignore]
    public string DataFolderPath { get; set; }
    [Ignore]
    public string[] PathMasksToInclude { get; set; }
    [Integer(100)]
    public int N { get; set; }

    private class Operation
    {
        public int Left { get; set; }
        public int Right { get; set; }
    }

    public IList<SkyAgentWork> ServerBuildAgentsData(string workingDirectory, IList<ServerAgent> agents)
    {
        if (N <= 0) throw new ArgumentException("Please specify positive number");
        var delta = N / agents.Count - 1;
        var results = new List<SkyAgentWork>();
        var x = 1;
        for (var i = 0; i < agents.Count; ++i)
        {
            var a = agents[i];
            var o = new Operation { Left = x, Right = x + delta };
            x += delta + 1;
            if (i == agents.Count - 1) o.Right = N;
            var w = new SkyAgentWork
            {
                Agent = a,
                Config = JsonSerializer.SerializeToString(o)
            };
            results.Add(w);
        }
        return results;
    }

    public string ServerBuildResultByAgentResults(IList<SkyAgentWork> results)
    {
        BigInteger result = 1;
        foreach (var x in results)
        {
            var value = BigInteger.Parse(x.Report);
            if (value == 0) return x.Report;
            checked
            {
                result *= value;
            }
        }
        return result.ToString();
    }

    public string AgentExecute(string workingDirectory, string agentData, ISkyContext context)
    {
        var o = JsonSerializer.DeserializeFromString<Operation>(agentData);
        checked
        {
            BigInteger result = o.Left;
            if (o.Left <= o.Right)
            {
                var size = o.Right - o.Left;
                var i = 0;
                while (++i + o.Left <= o.Right)
                {
                    result *= i + o.Left;
                    context.Update(i / size);
                    if (context.UserPressClose) break;
                }
            }
            else
            {
                result = 1;
            }
            return result.ToString();
        }
    }

}
