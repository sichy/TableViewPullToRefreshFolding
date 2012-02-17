using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreAnimation;

namespace TableViewPullToRefreshFolding
{
	public class FoldingTableViewController : UIViewController
	{
		const float kRefreshViewHeight = 65;
		
		UIView headerView;
		UIView topView;
		UILabel topLabel;
		UIView bottomView;
		UILabel bottomLabel;
		
		UITableView tableView;
		
		bool refreshing;
		
		public FoldingTableViewController (RectangleF frame, UITableViewStyle withStyle) : base ()
		{
			tableView = new UITableView (frame, withStyle);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.View.AddSubview (tableView);
			
			headerView = new UIView (new RectangleF (0, -kRefreshViewHeight, this.View.Frame.Width, kRefreshViewHeight));
			headerView.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
			tableView.AddSubview (headerView);
	
			var transform = CATransform3D.Identity;
    		transform.m34 = -1/500.0f;
			headerView.Layer.SublayerTransform = transform; // add the perspective
	
			topView = new UIView (new RectangleF (0, -kRefreshViewHeight / 4, headerView.Bounds.Size.Width, kRefreshViewHeight / 2.0f));
			topView.BackgroundColor = new UIColor (0.886f, 0.906f, 0.929f, 1);
			topView.Layer.AnchorPoint = new PointF (0.5f, 0.0f);
			headerView.AddSubview (topView);
	
			topLabel = new UILabel (topView.Bounds);
			topLabel.BackgroundColor = UIColor.Clear;
			topLabel.TextAlignment = UITextAlignment.Center;
			topLabel.Text = "Pull down to refresh";
			topLabel.TextColor = new UIColor (0.395f, 0.427f, 0.510f, 1);
			topLabel.ShadowColor = UIColor.FromWhiteAlpha (1, 0.7f);
			topLabel.ShadowOffset = new SizeF (0, 1);
			topView.AddSubview (topLabel);
			
			bottomView = new UIView (new RectangleF (0, kRefreshViewHeight * 3 / 4, headerView.Bounds.Size.Width, kRefreshViewHeight / 2));
			bottomView.BackgroundColor = new UIColor (0.836f, 0.856f, 0.879f, 1);
			bottomView.Layer.AnchorPoint = new PointF (0.5f, 1.0f);
			headerView.AddSubview (bottomView);
			
			bottomLabel = new UILabel (bottomView.Bounds);
			bottomLabel.BackgroundColor = UIColor.Clear;
			bottomLabel.Text = "Last updated: 1/11/13 8:41 PM";
			bottomLabel.TextAlignment = UITextAlignment.Center;
			bottomLabel.TextColor = UIColor.FromRGBA (0.395f, 0.427f, 0.510f, 1);
			bottomLabel.ShadowColor = UIColor.FromWhiteAlpha (1.0f, 0.7f);
			bottomLabel.ShadowOffset = new SizeF (0, 1);
			bottomView.AddSubview (bottomLabel);
			
			// Just so it's not white above the refresh view.
			var aboveView = new UIView (new RectangleF (0, -this.View.Bounds.Size.Height, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height - kRefreshViewHeight));
			aboveView.BackgroundColor = UIColor.FromRGB (0.886f, 0.906f, 0.929f);
			aboveView.Tag = 123;
			
			this.tableView.AddSubview (aboveView);
			
			refreshing = false;
			
			this.TableView.Scrolled += HandleTableViewhandleScrolled;
			this.TableView.DraggingEnded += HandleTableViewhandleDraggingEnded;
		}
		
		public UITableView TableView
		{
			get
			{
				return tableView;	
			}		
		}
		
		void RefreshData ()
		{
			refreshing = true;
			
			topLabel.Text = "Refreshing...";
			UIView.Animate (0.2, delegate {
				this.TableView.ContentInset = new UIEdgeInsets (kRefreshViewHeight, 0, 0, 0);
			});
			
			// two seconds delay here simulates the reload mechanism, obvisously this is just for a show, normally we would call the reset transformation on the worker thread that does the refresh
			UIView.Animate (0.2, 2, UIViewAnimationOptions.CurveLinear, delegate {
				ResetHeader ();
			}, null);
		}
		
		void ResetHeader ()
		{
			refreshing = false;
			this.TableView.ContentInset = UIEdgeInsets.Zero;
		}
		
		void UnfoldHeaderToFraction (float fraction)
		{
			bottomView.Layer.Transform = CATransform3D.MakeRotation ((float)Math.PI / 2f - (float)Math.Asin (fraction), 1f, 0, 0);
			topView.Layer.Transform = CATransform3D.MakeRotation ((float)Math.Asin (fraction) + ((((float)Math.PI) * 3f) / 2f), 1f, 0, 0);
			topView.Frame = new RectangleF (0, kRefreshViewHeight * (1 - fraction), this.View.Bounds.Size.Width, kRefreshViewHeight / 2);
		}

		void HandleTableViewhandleDraggingEnded (object sender, DraggingEventArgs e)
		{
			if (TableView.ContentOffset.Y < -kRefreshViewHeight) 
				this.RefreshData ();
		}

		void HandleTableViewhandleScrolled (object sender, EventArgs e)
		{
			if (!refreshing)
			{
				var fraction = this.TableView.ContentOffset.Y / -kRefreshViewHeight;
				
				if (fraction < 0) fraction = 0;
				if (fraction > 1) fraction = 1;
				
				UnfoldHeaderToFraction (fraction);
				
				if (fraction == 1)
					topLabel.Text = "Release to refresh";
				else 
					topLabel.Text = "Pull down to refresh";
			}
		}
	}
}

