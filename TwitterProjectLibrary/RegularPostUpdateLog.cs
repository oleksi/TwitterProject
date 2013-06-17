using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class RegularPostUpdateLog : PostUpdateLog
	{
		public virtual RegularPostUpdate RegularPostUpdate { get; set; }
	}
}
