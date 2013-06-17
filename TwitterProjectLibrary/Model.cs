using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class Model
	{
		public virtual int? Id { get; set; }
		public virtual string UserName { get; set; }
		public virtual string TwitterConsumerKey { get; set; }
		public virtual string TwitterConsumerSecret { get; set; }
		public virtual string TwitterAccessToken { get; set; }
		public virtual string TwitterAccessTokenSecret { get; set; }
		public virtual string LiveChatURL { get; set; }
		public virtual string OnlineStatusXMLFeed { get; set; }
	}
}
