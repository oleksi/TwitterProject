using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public enum PostUpdateType
	{
		Regular = 1,
		Online = 2
	}

	public class PostUpdate
	{
		public virtual int? Id { get; set; }
		public virtual string PostText { get; set; }
	}
}
