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

				Thread.Sleep(20000);
				Trace.WriteLine("Working", "Information");
			}
		}

		public override bool OnStart()
		{
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
