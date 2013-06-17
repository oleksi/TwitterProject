using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class OnlinePostUpdateLog : PostUpdateLog
	{
		public virtual OnlinePostUpdate OnlinePostUpdate { get; set; }
	}
}
