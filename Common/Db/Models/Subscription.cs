using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switchr.API.Tests.Common.Db.Models
{
	public class Subscription
	{
		public string Id { get; set; }
		public int Type { get; set; }
		public string TypeName { get; set; }
		public string SubscriptionNumber { get; set; }
		public string MsIsdn { get; set; }
	}
}
