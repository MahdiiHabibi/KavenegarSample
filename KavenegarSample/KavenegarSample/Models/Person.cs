using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KavenegarSample.Models
{
	public class Person
	{
		[Key]
		public int UserId { get; set; }

		public string FullName { get; set; }

		public string PhoneNumber { get; set; }

		public int Age { get; set; }

		public bool IsActive { get; set; }
	}
}
