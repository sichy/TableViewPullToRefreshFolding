using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TableViewPullToRefreshFolding
{
	public partial class TableViewPullToRefreshFoldingViewController : UIViewController
	{
		FoldingTableViewController foldingTableViewController;
		
		public TableViewPullToRefreshFoldingViewController () : base ("TableViewPullToRefreshFoldingViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			foldingTableViewController = new FoldingTableViewController (new RectangleF (0, -20, 320, 480), UITableViewStyle.Plain);
			
			foldingTableViewController.TableView.Source = new TableViewSource ();
			foldingTableViewController.TableView.ReloadData ();
			
			this.View.AddSubview (foldingTableViewController.View);
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
		
		class TableViewSource : UITableViewSource
		{
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				return 100;
			}
			
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = new UITableViewCell (UITableViewCellStyle.Default, "Cell");
	
				cell.TextLabel.Text = "Row " + indexPath.Row;
	
				return cell;
			}
		}
	}
}

