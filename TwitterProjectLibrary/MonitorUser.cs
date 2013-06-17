using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class MonitorUser
	{
		public virtual int? Id { get; set; }
		public virtual string UserName { get; set; }
		public virtual DateTime LastMonitorDate { get; set; }
		public virtual bool IsActive { get; set; }
	}
}
