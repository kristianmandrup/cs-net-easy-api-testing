using System.Collections.Generic;
using System.Linq;

namespace Switchr.API.Tests.Common.Db.Models
{
	public class User
	{
		public string Id { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public string LastName { get; set; }
		public string SSN { get; set; }
		public int BrandId { get; set; }
		public int SegmentId { get; set; }
		public int RoleId { get; set; }

		public string BrandName { get; set; }
		public string SegmentName { get; set; }

		public IEnumerable<Ban> Bans { get; set; }
		public IEnumerable<Subscription> Subscriptions => Bans.SelectMany(ban => ban.Subscriptions);
	}
}
