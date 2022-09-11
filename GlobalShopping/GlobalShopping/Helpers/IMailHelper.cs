using GlobalShopping.Common;

namespace GlobalShopping.Helpers
{
	public interface IMailHelper
	{
		Response SendMail(string toName, string toEmail, string subject, string body);

	}
}
