using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class FriendProspect
	{
		public virtual int? Id { get; set; }
		public virtual string UserName { get; set; }
		public virtual DateTime InsertDate { get; set; }
		public virtual DateTime LastActivityDate { get; set; }
		public virtual MonitorUser ReferredBy { get; set; }
		public virtual bool IsActive { get; set; }
	}
}
