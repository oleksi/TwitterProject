using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace TwitterProjectBL
{
	public class TwitterProjectException : ApplicationException
	{
		public int ModelId { get; private set; }
		public int TwitterErrorCode { get; private set; }
		public string TwitterErrorMessage { get; private set; }

		public TwitterProjectException(int modelId, TwitterError error)
		{
			ModelId = modelId;
			TwitterErrorCode = error.Code;
			TwitterErrorMessage = error.Message;
		}

		public override string ToString()
		{
			return String.Format("ModelId: {0}\nTwitterErrorCode: {1}\nTwitterErrorMessage: {2}", ModelId, TwitterErrorCode, TwitterErrorMessage);
		}
	}
}
