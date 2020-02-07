using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace fyleo.EventLog
{
    public class EventLogFile : IEventLog
    {
        public static string EVENTLOG_DIR = Directory.GetCurrentDirectory() + "/log";
        public const string EVENTLOG_FILE = "events.log";

        public EventLogFile() 
        {
            if(!Directory.Exists(EVENTLOG_DIR))
                Directory.CreateDirectory(EVENTLOG_DIR);
            
            var filePath = EVENTLOG_DIR + "/" + EVENTLOG_FILE;
            if(!File.Exists(filePath))
                File.CreateText(filePath);
        }
        
        public async Task Write(string user, string action, string message)
        {
            var filePath = EVENTLOG_DIR + "/" + EVENTLOG_FILE;
            var file = File.ReadAllText(filePath);

            file += $"{DateTime.UtcNow.ToString()};{user};{action};{message}\r\n";
            
            // ToDo: If filesize > x bytes split to archive files with incremental numbers n
            // var filePath = EVENTLOG_DIR + "/" + "n".PadLeft(2, '0') + "_" + EVENTLOG_FILE; 

            await File.WriteAllTextAsync(filePath, file);
        }

        public async Task<Activity[]> GetLatest()
        {
            var filePath = EVENTLOG_DIR + "/" + EVENTLOG_FILE;
            var logContent = await File.ReadAllTextAsync(filePath);

            var lastRows = logContent.Split("\r\n")
                .Where(x=>!string.IsNullOrEmpty(x))
                .Where( x => 
                    x.Split(';')[2] == Actions.UPLOAD_FILE || 
                    x.Split(';')[2] == Actions.DELETE_FOLDER || 
                    x.Split(';')[2] == Actions.EDIT_FOLDER || 
                    x.Split(';')[2] == Actions.CREATE_FOLDER || 
                    x.Split(';')[2] == Actions.DELETE_FILE)
                .TakeLast(10);
            var result = new List<Activity>();

            foreach(var r in lastRows)
            {
                var cols = r.Split(";");
                result.Add(Activity.Parse(DateTime.Parse(cols[0]), cols[1], cols[2], cols[3]));
            }

            return result.OrderByDescending(x=>x.Timestamp).ToArray();
        }
    }
}