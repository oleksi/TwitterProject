using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using TweetSharp;
using TwitterProjectBL;
using TwitterProjectBL.Tasks;
using TwitterProjectData;
using TwitterProjectModel;

namespace TwitterProjectWorkerRole
{
	public class WorkerRole : RoleEntryPoint
	{
		private List<ModelWorker> m_ModelWorkers = null;
		private int m_NextIterationMinSeconds = 0;
		private int m_NextIterationMaxSeconds = 0;

		public override void Run()
		{
			// This is a sample worker implementation. Replace with your logic.
			Trace.WriteLine("TwitterProjectWorkerRole entry point called", "Information");

			while (true)
			{
				foreach (ModelWorker currModelWorker in m_ModelWorkers)
				{
					foreach (ITask task in currModelWorker.Tasks)
					{
						if (task.GetNextRunningDate() <= DateTime.Now)
							task.RunAsync();
					}
				}

				//making it random so it doesn't look like a robot
				Random rnd = new Random(DateTime.Now.Millisecond);
				int secondsInterval = rnd.Next(m_NextIterationMinSeconds, m_NextIterationMaxSeconds);
				Thread.Sleep(secondsInterval * 1000);

				Trace.WriteLine("Working", "Information");
			}
		}

		public override bool OnStart()
		{
			m_NextIterationMinSeconds = Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("NextIterationMinSeconds"));
			m_NextIterationMaxSeconds = Convert.ToInt32(RoleEnvironment.GetConfigurationSettingValue("NextIterationMaxSeconds"));

			m_ModelWorkers = new List<ModelWorker>();
			ModelRepository modelRepository = new ModelRepository();

			IList<Model> models = modelRepository.GetActiveModels();
			foreach (Model currModel in models)
			{
				Dictionary<string, string> modelWorkerSettings = new Dictionary<string, string>();
				if (currModel.From == "Streamate")
					modelWorkerSettings["StreamateXMLRequest"] = RoleEnvironment.GetConfigurationSettingValue("StreamateXMLRequest");

				ModelWorker modelWorker = new ModelWorker(currModel, modelWorkerSettings);

				m_ModelWorkers.Add(modelWorker);
			}

			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			return base.OnStart();
		}
	}
}
