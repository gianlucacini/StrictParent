using Serilog;
using SimpleInjector;
using StrictParent.Service.BusinessLogic;
using StrictParent.Service.Jobs;
using StrictParent.Service.Services;
using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace StrictParent.Service
{
    static class Program
    {
        static readonly Container _container = new Container();

        /// <summary>
        /// Main Entry Point
        /// </summary>
        static void Main()
        {
            RegisterServices();

            var strictParentService = _container.GetInstance<StrictParentService>();

            if (Environment.UserInteractive)
            {
                strictParentService.OnDebug();

                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                ServiceBase[] ServicesToRun = new ServiceBase[]
                {
                   strictParentService
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        private static void RegisterServices()
        {
            _container.RegisterInstance(ConfigureLogger());
            _container.Register<BlockConnectionsFirewallRule>(Lifestyle.Singleton);
            _container.Register<CriticalProcess>(Lifestyle.Singleton);

            _container.Register<BlockConnectionsFirewallRuleJob>(Lifestyle.Singleton);
            _container.Register<RegistryWrapper>(Lifestyle.Singleton);
            _container.Register<StrictParentJob>(Lifestyle.Singleton);

            _container.Register<AppStatusService>(Lifestyle.Singleton);
            _container.Register<DateTimeService>(Lifestyle.Singleton);
            _container.Register<OrchestratorService>(Lifestyle.Singleton);
            _container.Register<WCFService>(Lifestyle.Singleton);

            _container.Register<StrictParentService>(Lifestyle.Singleton);

            _container.Verify();
        }

        private static ILogger ConfigureLogger()
        {

            var serilogConfiguration = new LoggerConfiguration()
                .WriteTo
                .File(LogPath())
                .CreateLogger();

            return serilogConfiguration;
        }

        private static string LogPath()
        {
            String localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(localPath, $"StrictParent_{DateTime.Now.ToString("yyyy")}_{DateTime.Now.ToString("MM")}.log");
        }
    }
}