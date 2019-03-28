using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Switchr.API.Tests.Common.Db.Models;
using Switchr.Data.DataModel;
// Fixture Model
using Usr = Switchr.API.Tests.Common.Db.Models.User;
using Subscr = Switchr.API.Tests.Common.Db.Models.Subscription;
using Ban = Switchr.API.Tests.Common.Db.Models.Ban;
// DataModel
using Subscription = Switchr.Data.DataModel.Subscription;
using User = Switchr.Data.DataModel.User;

namespace Switchr.API.Tests.Common.Db
{
	public class DbDataBuilder
	{
		private int _id = 0;

		protected int NextId()
		{			
			return ++_id;
		}

		private static Mock<DbSet<T>> CreateMockedDbSet<T>(IEnumerable<T> dataSet) where T : class
		{
			var dbSet = CreateMocked<DbSet<T>>();
			DbMocker<T> dbMocker = new DbMocker<T>(dbSet, dataSet);
			var values = new[] { "Provider", "Expression", "ElementType", ":GetEnumerator" };
			dbMocker.CreateMockedDb<T>(values);
			return dbSet.MockObj;
		}

		public static MockMoq<T> CreateMocked<T>() where T : class
		{
			return MockMoq<T>.Create();
		}

		public IEnumerable<User> BuildUsers(IEnumerable<Usr> users)
		{
			return users.Select(BuildUser);
		}

		public IEnumerable<BAN> BuildBans(IEnumerable<Ban> bans)
		{
			return bans.Select(BuildBan);
		}

		public BAN BuildBan(Ban ban)
		{
			return new BAN
			{
				Id = NextId(),
				Subscriptions = BuildSubscriptions(ban.Subscriptions).ToList()
			};
		}

		private IEnumerable<Subscription> BuildSubscriptions(IEnumerable<Subscr> subscriptions)
		{
			return subscriptions.Select(BuildSubscription);
		}

		public User BuildUser(Usr usr)
		{
			return new User
			{
				Id = NextId(),
				Email = usr.Email,
				Name = usr.Name,
				LastName = usr.LastName,
				SSN = usr.SSN,
				SegmentId = usr.SegmentId,
				BrandId = usr.BrandId,
				RoleId = usr.RoleId,
				UserBans = BuildUserBans(usr.Bans, usr).ToList(),
				UserSubscriptions = BuildUserSubscriptions(usr.Subscriptions, usr).ToList()
			};
		}

		public IEnumerable<UserSubscription> BuildUserSubscriptions(IEnumerable<Subscr> subscriptions, Usr usr)
		{

			return subscriptions.Select(sub => BuildUserSubscription(sub, usr));
		}

		public IEnumerable<UserBan> BuildUserBans(IEnumerable<Ban> bans, Usr usr)
		{

			return bans.Select(ban => BuildUserBan(ban, usr));
		}

		public UserBan BuildUserBan(Ban ban, Usr usr)
		{
			return new UserBan
			{
				BanId = int.Parse(ban.Id),
				UserId = int.Parse(usr.Id),
				BanAccessRightsId = ban.BanAccessRightsId
			};
		}

		public UserSubscription BuildUserSubscription(Subscr subscription, Usr usr)
		{
			return new UserSubscription
			{
				Id = NextId(),
				SubscriptionId = int.Parse(subscription.Id),
				UserId = int.Parse(usr.Id),				
				Subscription = BuildSubscription(subscription)				
			};
		}

		public Subscription BuildSubscription(Subscr subscription)
		{
			return new Subscription()
			{
				SubscriptionNumber = subscription.SubscriptionNumber,
				TypeId = subscription.Type
			};
		}
	}
}
