using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChapterTool
{
    public class CTLogger
    {
        // Fields
        private static StringBuilder _Log = new StringBuilder();

        public delegate void LogLineAddedEventHandler(string lineAdded, DateTime actionDate);


        // Events
        public static event LogLineAddedEventHandler LogLineAdded;

        // Methods
        public static void Log(string message)
        {
            DateTime now = DateTime.Now;
            string str = string.Format("{0} {1}", now.ToString("[yyyy-MM-dd][HH:mm:ss]"), message);
            _Log.AppendLine(str);
            OnLogLineAdded(str, now);
        }

        protected static void OnLogLineAdded(string lineAdded, DateTime actionDate)
        {
            if (LogLineAdded != null)
            {
                LogLineAdded(lineAdded, actionDate);
            }
        }

        // Properties
        public static string LogText
        {
            get
            {
                return _Log.ToString();
            }
        }
    }
}
