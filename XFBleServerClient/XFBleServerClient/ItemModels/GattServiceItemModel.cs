using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFBleServerClient.Core.ItemModels
{
    public class GattServiceItemModel : BindableBase
    {
		public Guid ServiceUuid { get; set; }
		public string Name { get; set; }
		public bool IsDeletable { get; set; }
	}
}
