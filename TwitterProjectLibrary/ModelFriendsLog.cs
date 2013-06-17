using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class ModelFriendsLog
	{
		public virtual int? Id { get; set; }
		public virtual Model Model { get; set; }
		public virtual FriendProspect Friend { get; set; }
		public virtual DateTime DateFriended { get; set; }
	}
}
