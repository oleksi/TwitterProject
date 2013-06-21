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
						{
							try
							{
								task.Run();
							}
							catch (Exception ex)
							{
								Trace.TraceError(ex.ToString());
							}
							finally
							{
								task.SetNextRunningDate();
							}
						}
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
			string[] modelIDs = RoleEnvironment.GetConfigurationSettingValue("ModelIDs").Split(',');

			foreach (string modelIDStr in modelIDs)
			{
				Model currModel = modelRepository.GetModelById(Convert.ToInt32(modelIDStr));
				m_ModelWorkers.Add(new ModelWorker(currModel));
			}

			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			return base.OnStart();
		}
	}
}
