using System;
using System.Diagnostics;
using NLog;
using NLog.Targets;
using NodaTime;
using Raven.Client.Documents;

namespace DragonCon.RavenDB.NLog
{
    [Target("RavenDb")]
    public class RavenNLogTarget : TargetWithLayout 
    {
        private static IDocumentStore Store { get; set; }

        public RavenNLogTarget(IDocumentStore store)
        {
            Store = store;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (Store == null)
            {
                Debug.WriteLine("Error: Cannot find Raven Document Store");
                return;
            }

            var entry = new LogEntry
            {
                Exception = logEvent.Exception,
                LogLevel = logEvent.Level.Name,
                Message = logEvent.FormattedMessage,
                StackTrace = logEvent.StackTrace,
                TimeStamp = Instant.FromDateTimeUtc(logEvent.TimeStamp.ToUniversalTime()),
                LoggerName = logEvent.LoggerName,
                LogLevelOrdinal = logEvent.Level.Ordinal,
                Caller = new LogEntry.CallerInformation
                {
                    ClassName = logEvent.CallerClassName,
                    FilePath = logEvent.CallerFilePath,
                    LineNumber = logEvent.CallerLineNumber,
                    MemberName = logEvent.CallerMemberName
                }
            };

            using (var session = Store.OpenSession())
            {
                session.Store(entry);
                session.SaveChanges();
            }
        }
    }

    public class LogEntry
    {
        public LogEntry()
        {
            Caller = new CallerInformation();
        }
        public string Id { get; set; }
        public Exception Exception { get; set; }
        public string LogLevel { get; set; }
        public int LogLevelOrdinal { get; set; }
        public StackTrace StackTrace { get; set; }
        public Instant TimeStamp { get; set; }
        public string Message { get; set; }
        public string LoggerName { get; set; }
        public CallerInformation Caller { get; set; }

        public class CallerInformation
        {
            public string ClassName { get; set; }
            public string FilePath { get; set; }
            public int LineNumber { get; set; }
            public string MemberName { get; set; }
        }
    }
}
