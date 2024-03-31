using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ogu.Dal.Sql.Observers
{
    public class SqlWithNoLockObserver : IObserver<KeyValuePair<string, object>>
    {
        private static readonly Regex WithNoLockRegex = new Regex(@"(FROM|JOIN).+AS\s[^\s\)(]+", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private const string WithNoLockReplacement = "${0} WITH (NOLOCK)";
        private const string SqlServerConnection = nameof(SqlServerConnection);

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (value.Key != RelationalEventId.CommandExecuting.Name) 
                return;

            var commandEventData = (CommandEventData)value.Value;

            if (commandEventData.Connection.GetType().Name != SqlServerConnection) 
                return;

            var executeMethod = commandEventData.ExecuteMethod;

            if (executeMethod == DbCommandMethod.ExecuteNonQuery) 
                return;

            var command = commandEventData.Command;
            command.CommandText = WithNoLockRegex.Replace(command.CommandText, WithNoLockReplacement);
        }
    }
}