using System.Security.Claims;

namespace Switchr.API.Tests.Common
{
	public class TestPrincipal : ClaimsPrincipal
	{
		public TestPrincipal(params Claim[] claims) : base(new TestIdentity(claims))
		{
		}
	}

	public class TestIdentity : ClaimsIdentity
	{
		public TestIdentity(params Claim[] claims) : base(claims)
		{
		}

		//userServicesModel.Segment = (Models.Enums.Segment) dbUser.SegmentId;
		//userServicesModel.Role = (Models.Enums.Role) dbUser.Role.Id;
		//userServicesModel.Brand = (Models.Enums.Brand) dbUser.Brand.Id;

	}
}
