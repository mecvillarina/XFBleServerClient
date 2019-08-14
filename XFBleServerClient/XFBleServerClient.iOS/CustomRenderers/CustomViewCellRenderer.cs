using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XFBleServerClient.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(ViewCell), typeof(CustomViewCellRenderer))]
namespace XFBleServerClient.iOS.CustomRenderers
{
	public class CustomViewCellRenderer : ViewCellRenderer
	{
		public override UITableViewCell GetCell(Xamarin.Forms.Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
			if (cell != null)
			{
				switch (item.StyleId)
				{
					case "RemoveSelectionStyle": cell.SelectionStyle = UIKit.UITableViewCellSelectionStyle.None; break;
					case "TransparentStyle": cell.BackgroundColor = UIColor.Clear; break;
					default: break;
				}
			}
			return cell;
		}
	}
}