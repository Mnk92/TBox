using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    public interface ISkyScript
    {
        string DataFolderPath { get; set; }
        string[] PathMasksToInclude { get; set; }

        // this part will be executed on the server
        #region Server
        //this method should return  agentsData for the each agent
        IList<SkyAgentWork> ServerBuildAgentsData(string workingDirectory, IList<ServerAgent> agents);
        //this method should return report by agentData and agentReport for the each agent
        string ServerBuildResultByAgentResults(IList<SkyAgentWork> results);
        #endregion Server

        // this part will be executed on the each agent
        #region Agent
        //this method should return report
        string AgentExecute(string workingDirectory, string agentData, ISkyContext context);
        #endregion Agent
    }
}
