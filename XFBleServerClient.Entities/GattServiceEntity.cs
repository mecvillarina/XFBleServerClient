using System;
using System.Collections.Generic;
using System.Text;

namespace XFBleServerClient.Entities
{
	public class GattServiceEntity
	{
		public Guid ServiceUuid { get; set; }
		public string Name { get; set; }
		public bool IsDeletable { get; set; }
	}
}
