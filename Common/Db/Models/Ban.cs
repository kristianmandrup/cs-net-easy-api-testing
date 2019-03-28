using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switchr.API.Tests.Common.Db.Models
{
	public class Ban
	{
		public string Id { get; set; }
		public int BanAccessRightsId { get; set; }
		public Subscription[] Subscriptions { get; set; }
	}
}
