using log4net;
using System;
using System.ServiceProcess;
using System.Timers;

namespace ebook2memrise.service
{
    public partial class Scheduler : ServiceBase
    {
        private Timer timer = null;
        private static readonly ILog log = LogManager.GetLogger(typeof(Scheduler));

        public Scheduler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer();
            timer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            log.Info("Started");
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            log.Info("Tick elpased");
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            log.Info("Stopped");
        }
    }
}
