// <auto-generated/>
// Source: https://weblogs.asp.net/marianor/a-wpf-wrapper-around-windows-form-notifyicon
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace ParentalControl.View
{
	/// <summary>
	/// Balloon tip icon enum.
	/// </summary>
	public enum BalloonTipIcon
	{
		/// <summary>
		/// None.
		/// </summary>
		None = 0,

		/// <summary>
		/// Info.
		/// </summary>
		Info = 1,

		/// <summary>
		/// Warning.
		/// </summary>
		Warning = 2,

		/// <summary>
		/// Error.
		/// </summary>
		Error = 3,
	}

	[ContentProperty("Text")]
	[DefaultEvent("MouseDoubleClick")]
	public partial class NotifyIcon : FrameworkElement, IAddChild
	{
		/// <summary>
		/// Mouse click event.
		/// </summary>
		public static readonly RoutedEvent MouseClickEvent = EventManager.RegisterRoutedEvent(
			"MouseClick",
			RoutingStrategy.Bubble,
			typeof(MouseButtonEventHandler),
			typeof(NotifyIcon));

		/// <summary>
		/// Mouse double click event.
		/// </summary>
		public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent(
			"MouseDoubleClick",
			RoutingStrategy.Bubble,
			typeof(MouseButtonEventHandler),
			typeof(NotifyIcon));

		/// <summary>
		/// Balloon tip icon property.
		/// </summary>
		public static readonly DependencyProperty BalloonTipIconProperty =
			DependencyProperty.Register("BalloonTipIcon", typeof(BalloonTipIcon), typeof(NotifyIcon));

		/// <summary>
		/// Balloon tip text property.
		/// </summary>
		public static readonly DependencyProperty BalloonTipTextProperty =
			DependencyProperty.Register("BalloonTipText", typeof(string), typeof(NotifyIcon));

		/// <summary>
		/// Balloon tip title property.
		/// </summary>
		public static readonly DependencyProperty BalloonTipTitleProperty =
			DependencyProperty.Register("BalloonTipTitle", typeof(string), typeof(NotifyIcon));

		/// <summary>
		/// Icon property.
		/// </summary>
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
			"Icon",
			typeof(Drawing.Icon),
			typeof(NotifyIcon),
			new FrameworkPropertyMetadata(OnIconChanged));

		/// <summary>
		/// Text property.
		/// </summary>
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text",
			typeof(string),
			typeof(NotifyIcon),
			new PropertyMetadata(OnTextChanged));

		private Forms.NotifyIcon notifyIcon;

		static NotifyIcon()
		{
			VisibilityProperty.OverrideMetadata(typeof(NotifyIcon), new PropertyMetadata(OnVisibilityChanged));
		}

		/// <summary>
		/// Mouse click.
		/// </summary>
		public event MouseButtonEventHandler MouseClick
		{
			add { this.AddHandler(MouseClickEvent, value); }
			remove { this.RemoveHandler(MouseClickEvent, value); }
		}

		/// <summary>
		/// Mouse double click.
		/// </summary>
		public event MouseButtonEventHandler MouseDoubleClick
		{
			add { this.AddHandler(MouseDoubleClickEvent, value); }
			remove { this.RemoveHandler(MouseDoubleClickEvent, value); }
		}

		/// <summary>
		/// Balloon tip icon.
		/// </summary>
		public BalloonTipIcon BalloonTipIcon
		{
			get { return (BalloonTipIcon)this.GetValue(BalloonTipIconProperty); }
			set { this.SetValue(BalloonTipIconProperty, value); }
		}

		/// <summary>
		/// Balloon tip text.
		/// </summary>
		public string BalloonTipText
		{
			get { return (string)this.GetValue(BalloonTipTextProperty); }
			set { this.SetValue(BalloonTipTextProperty, value); }
		}

		/// <summary>
		/// Balloon tip title.
		/// </summary>
		public string BalloonTipTitle
		{
			get { return (string)this.GetValue(BalloonTipTitleProperty); }
			set { this.SetValue(BalloonTipTitleProperty, value); }
		}

		/// <summary>
		/// Icon.
		/// </summary>
		public Drawing.Icon Icon
		{
			get { return (Drawing.Icon)this.GetValue(IconProperty); }
			set { this.SetValue(IconProperty, value); }
		}

		/// <summary>
		/// Text.
		/// </summary>
		public string Text
		{
			get { return (string)this.GetValue(TextProperty); }
			set { this.SetValue(TextProperty, value); }
		}

		/// <summary>
		/// Begin init.
		/// </summary>
		public override void BeginInit()
		{
			base.BeginInit();
			this.InitializeNotifyIcon();
		}

		/// <summary>
		/// Show balloon tip.
		/// </summary>
		/// <param name="timeout"></param>
		public void ShowBalloonTip(int timeout)
		{
			this.notifyIcon.BalloonTipTitle = this.BalloonTipTitle;
			this.notifyIcon.BalloonTipText = this.BalloonTipText;
			this.notifyIcon.BalloonTipIcon = (Forms.ToolTipIcon)this.BalloonTipIcon;
			this.notifyIcon.ShowBalloonTip(timeout);
		}

		/// <summary>
		/// Show balloon tip.
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="tipTitle"></param>
		/// <param name="tipText"></param>
		/// <param name="tipIcon"></param>
		public void ShowBalloonTip(int timeout, string tipTitle, string tipText, BalloonTipIcon tipIcon)
		{
			this.notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, (Forms.ToolTipIcon)tipIcon);
		}

		#region IAddChild Members

		void IAddChild.AddChild(object value)
		{
			throw new InvalidOperationException();
		}

		void IAddChild.AddText(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			this.Text = text;
		}

		#endregion

		/// <summary>
		/// On visual parent changed.
		/// </summary>
		/// <param name="oldParent"></param>
		protected override void OnVisualParentChanged(DependencyObject oldParent)
		{
			base.OnVisualParentChanged(oldParent);
			this.AttachToWindowClose();
		}

		private static MouseButtonEventArgs CreateMouseButtonEventArgs(
			RoutedEvent handler,
			Forms.MouseButtons button)
		{
			return new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(button))
			{
				RoutedEvent = handler
			};
		}

		private static void OnIconChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(target))
			{
				NotifyIcon control = (NotifyIcon)target;
				control.notifyIcon.Icon = control.Icon;
			}
		}

		private static void OnTextChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			NotifyIcon control = (NotifyIcon)target;
			control.notifyIcon.Text = control.Text;
		}

		private static void OnVisibilityChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			NotifyIcon control = (NotifyIcon)target;
			control.notifyIcon.Visible = control.Visibility == Visibility.Visible;
		}

		private static MouseButton ToMouseButton(Forms.MouseButtons button)
		{
			switch (button)
			{
				case Forms.MouseButtons.Left:
					return MouseButton.Left;
				case Forms.MouseButtons.Right:
					return MouseButton.Right;
				case Forms.MouseButtons.Middle:
					return MouseButton.Middle;
				case Forms.MouseButtons.XButton1:
					return MouseButton.XButton1;
				case Forms.MouseButtons.XButton2:
					return MouseButton.XButton2;
			}

			throw new InvalidOperationException();
		}

		private void AttachToWindowClose()
		{
			var window = Window.GetWindow(this);
			if (window != null)
			{
				window.Closed += (s, a) => this.notifyIcon.Dispose();
			}
		}

		private void InitializeNotifyIcon()
		{
			this.notifyIcon = new Forms.NotifyIcon();
			this.notifyIcon.Text = this.Text;
			this.notifyIcon.Icon = this.Icon;
			this.notifyIcon.Visible = this.Visibility == Visibility.Visible;

			this.notifyIcon.MouseDown += this.OnMouseDown;
			this.notifyIcon.MouseUp += this.OnMouseUp;
			this.notifyIcon.MouseClick += this.OnMouseClick;
			this.notifyIcon.MouseDoubleClick += this.OnMouseDoubleClick;

			this.InitializeNativeHooks();
		}

		private void OnMouseDown(object sender, Forms.MouseEventArgs e)
		{
			this.RaiseEvent(CreateMouseButtonEventArgs(MouseDownEvent, e.Button));
		}

		private void OnMouseDoubleClick(object sender, Forms.MouseEventArgs e)
		{
			this.RaiseEvent(CreateMouseButtonEventArgs(MouseDoubleClickEvent, e.Button));
		}

		private void OnMouseClick(object sender, Forms.MouseEventArgs e)
		{
			this.RaiseEvent(CreateMouseButtonEventArgs(MouseClickEvent, e.Button));
		}

		private void OnMouseUp(object sender, Forms.MouseEventArgs e)
		{
			if (e.Button == Forms.MouseButtons.Right)
			{
				this.ShowContextMenu();
			}

			this.RaiseEvent(CreateMouseButtonEventArgs(MouseUpEvent, e.Button));
		}

		private void ShowContextMenu()
		{
			if (this.ContextMenu != null)
			{
				this.AttachContextMenu();
				this.ContextMenu.IsOpen = true;
			}
		}

		partial void AttachContextMenu();

		partial void InitializeNativeHooks();
	}

	internal static class IconUtilities
	{
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		public static ImageSource ToImageSource(this Drawing.Icon icon)
		{
			Drawing.Bitmap bitmap = icon.ToBitmap();
			IntPtr hBitmap = bitmap.GetHbitmap();

			ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
				hBitmap,
				IntPtr.Zero,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());

			if (!DeleteObject(hBitmap))
			{
				throw new Win32Exception();
			}

			return wpfBitmap;
		}
	}
}