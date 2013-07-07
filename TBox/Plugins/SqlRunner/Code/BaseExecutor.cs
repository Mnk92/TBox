using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Mnk.TBox.Locales.Localization.Plugins.SqlRunner;
using Mnk.TBox.Plugins.SqlRunner.Code.Settings;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfControls.Dialogs;

namespace Mnk.TBox.Plugins.SqlRunner.Code
{
	public class BaseExecutor
	{
		public void Execute(Window owner, Op operation, string connectionString, Action<DatabaseInfo> onEnd, ImageSource icon)
		{
		    DialogsCache.ShowProgress(
		        u => onEnd(GetResult(connectionString, operation)),
		        SqlRunnerLang.MakeRequestTo + ": " + operation.Key, owner, icon: icon);
		}

		public static DatabaseInfo GetResult(string connectionString, Op operation)
		{
			var sw = new Stopwatch();
			sw.Start();
			try
			{
				var table = new List<IList<string>>(); 
				using (var connection = new SqlConnection(connectionString))
				{
				    connection.Open();
				    return operation.UseTransaction ? 
                        ExecuteWithTransaction(operation, connection, table, sw) :
                        ExecuteWithoutTransaction(operation, connection, table, sw);
				}
			}
			catch (Exception ex)
			{
				return new DatabaseInfo {Status = "Exception", Body = ex.Message, Time = sw.ElapsedMilliseconds};
			}
		}

        private static DatabaseInfo ExecuteWithoutTransaction(Op operation, SqlConnection connection, List<IList<string>> table, Stopwatch sw)
        {
            using (var cmd = new SqlCommand(operation.Command, connection))
            {
                cmd.CommandTimeout = operation.Timeout;
                var sb = new StringBuilder();
                var count = cmd.ExecuteNonQuery();
                sb.AppendLine(SqlRunnerLang.RecordsAffected + ": " + count);

                return new DatabaseInfo
                {
                    Status = "OK",
                    Body = sb.ToString(),
                    Time = sw.ElapsedMilliseconds
                };
            }
        }


	    private static DatabaseInfo ExecuteWithTransaction(Op operation, SqlConnection connection, List<IList<string>> table, Stopwatch sw)
	    {
	        using (var tr = connection.BeginTransaction())
	        {
	            using (var cmd = new SqlCommand(operation.Command, connection))
	            {
	                cmd.CommandTimeout = operation.Timeout;
	                cmd.Transaction = tr;
	                var sb = new StringBuilder();
	                using (var r = cmd.ExecuteReader())
	                {
	                    sb.AppendLine(SqlRunnerLang.RecordsAffected + ": " + r.RecordsAffected);
	                    if (r.VisibleFieldCount > 0)
	                    {
	                        table.Add(new List<string>());
	                        for (var i = 0; i < r.VisibleFieldCount; ++i)
	                        {
	                            table[0].Add(r.GetName(i));
	                        }
	                        while (r.Read())
	                        {
	                            var list = new List<string>();
	                            table.Add(list);
	                            for (var i = 0; i < r.VisibleFieldCount; ++i)
	                            {
	                                list.Add(r[i].ToString());
	                            }
	                        }
	                        sb.PrintTable(table);
	                    }
	                }
	                tr.Commit();

	                return new DatabaseInfo
	                    {
	                        Status = "OK",
	                        Body = sb.ToString(),
	                        Time = sw.ElapsedMilliseconds
	                    };
	            }
	        }
	    }
	}
}
