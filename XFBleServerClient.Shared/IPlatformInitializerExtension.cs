using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFBleServerClient.Shared
{
	public interface IPlatformInitializerExtension
	{
		IContainerRegistry Container { get;  }
	}
}
