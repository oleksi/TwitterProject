using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProjectModel
{
	public class PostUpdateLog
	{
		public virtual int? Id { get; set; }
		public virtual Model Model { get; set; }
		public virtual DateTime LastPublishedDate { get; set; }
	}
}
