using KavenegarSample.Models;

namespace KavenegarSample.Repository
{
	public interface ISenderCode
	{
		public Task<bool> SendCode(string phoneNumber, string templateName, string token1, string? token2 = "", string? token3 = "");
	}
}
