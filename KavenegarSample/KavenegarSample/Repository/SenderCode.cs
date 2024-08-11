using KavenegarSample.Models;
using Kavenegar.Core;
namespace KavenegarSample.Repository
{
	public class SenderCode : ISenderCode
	{
		public async Task<bool> SendCode(string phoneNumber, string templateName, string token1, string? token2 = "", string? token3 = "")
		{
			try
			{
				var api = new Kavenegar.KavenegarApi("Your API Code");
				return (await api.VerifyLookup(phoneNumber, token1, token2, token3, template: templateName)).Status == 200 ? true : false;
			}
			catch
			{
				return false;
			}

		}
	}
}
