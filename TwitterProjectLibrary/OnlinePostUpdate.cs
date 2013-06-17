using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class OnlinePostUpdate : PostUpdate
	{
		public OnlinePostUpdate()
		{
		}

		public OnlinePostUpdate(PostUpdate postUpdate)
		{
			this.Id = postUpdate.Id;
			this.PostText = postUpdate.PostText;
		}
	}
}
