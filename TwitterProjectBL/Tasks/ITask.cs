using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectBL.Tasks
{
	public interface ITask
	{
		DateTime GetNextRunningDate();
		void SetNextRunningDate();
		void Run();
	}
}
