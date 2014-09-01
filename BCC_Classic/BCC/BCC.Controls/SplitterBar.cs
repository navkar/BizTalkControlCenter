//---------------------------------------------------
//     Copyright (c) 2007 Jeffrey Bazinet, VWD-CMS 
//     http://www.vwd-cms.com/  All rights reserved.
//---------------------------------------------------

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BCC.Controls
{
	// this enum tells ths SplitterBar what technique to 
	// use when hiding IFrames. IFrames capture mouse 
	// events which prevent the SplitterBar from functioning
	// properly
	public enum SplitterBarIFrameHiding
	{
		DoNotHide,     // don't hide IFrames, this is really for testing, debugging
		UseVisibility, // use iframe.style.visibility = "hidden"
		UseDisplay     // use iframe.style.display = "none"
	}

	public enum SplitterBarOrientations
	{
		Vertical,
		Horizontal
	}

    [DefaultProperty("Tooltip")]
    [ToolboxData("<{0}:SplitterBar runat=server></{0}:SplitterBar>")]
	public class SplitterBar : Panel, INamingContainer, IPostBackDataHandler
	{
		protected HtmlInputHidden hdnWidth;
		protected HtmlInputHidden hdnMinWidth;
		protected HtmlInputHidden hdnMaxWidth;
		protected HtmlInputHidden hdnHeight;
		protected HtmlInputHidden hdnMinHeight;
		protected HtmlInputHidden hdnMaxHeight;

		private SplitterBarOrientations _orientation = SplitterBarOrientations.Vertical;

		private string _leftResizeTargets = string.Empty; // semi-colon delimited
		private string _rightResizeTargets = string.Empty; // semi-colon delimited
		private string _topResizeTargets = string.Empty; // semi-colon delimited
		private string _bottomResizeTargets = string.Empty; // semi-colon delimited
		private bool _dynamicResizing = false;
		private string _backgroundColor = null;
		private string _backgroundColorHilite = null;
		private string _backgroundColorResizing = null;
		private string _backgroundColorLimit = null;
		//private int _maxWidth = 0; // pixels, max size of LeftResizeTarget
		//private int _minWidth = 0; // pixels, min size of LeftResizeTarget
		private int _totalWidth = 0; // pixels, Total size of Left + Right target widths
		private string _saveWidthToElement = null; // element id to save the width to (input text or hidden)
		//private int _maxHeight = 0; // pixels, max size of TopResizeTarget
		//private int _minHeight = 0; // pixels, min size of TopResizeTarget
		private int _totalHeight = 0; // pixels, Total size of Top + Bottom target widths
		private string _saveHeightToElement = null; // element id to save the Height to (input text or hidden)
		private string _onResizeStart = null; // onmousedown fires this
		private string _onResize = null; // dynamic resizing and onmouseup fires this
		private string _onResizeComplete = null; // onmouseup fires this
		private string _onDoubleClick = null;
		private int _splitterWidth = 6;
		private int _splitterHeight = 6;
		private string _debugElement = null;
		private SplitterBarIFrameHiding _iframeHiding = SplitterBarIFrameHiding.UseVisibility;

		public SplitterBarOrientations Orientation
		{
			get { return _orientation; }
			set { _orientation = value; }
		}

		protected override void OnLoad(EventArgs e)
		{
			this.Page.RegisterRequiresPostBack(this);
			AddCompositeControls();
			this.RegisterPageStartupScript();
			this.ResizeTargetControls();

			base.OnLoad(e);
		}
        
		public void ResizeTargetControls()
		{
			if (this.Orientation == SplitterBarOrientations.Vertical)
			{
				SetTargetControlWidths();
			}
			else if (this.Orientation == SplitterBarOrientations.Horizontal)
			{
				SetTargetControlHeights();
			}
		}

		private void SetTargetControlWidths()
		{
			if (this.Page.IsPostBack)
			{
				Control[] targets = null;
				string width = null;

				// set the width of the controls in the 
				// LeftResizeTargets
				width = this.LeftColumnWidth;
				if (!string.IsNullOrEmpty(width))
				{
					targets = GetTargetControls(this.LeftResizeTargets);
					if (targets != null && targets.Length > 0)
					{
						foreach (Control c in targets)
						{
							SetControlWidth(c, width);
						}
					}
				}

				// set the width of the controls in the 
				// RightResizeTargets
				width = this.RightColumnWidth;
				if (!string.IsNullOrEmpty(width))
				{
					targets = GetTargetControls(this.RightResizeTargets);
					if (targets != null && targets.Length > 0)
					{
						foreach (Control c in targets)
						{
							SetControlWidth(c, width);
						}
					}
				}
			}
		}

		private void SetControlWidth(Control control, string width)
		{
			if (control != null)
			{
				if (control is WebControl)
				{
					WebControl wc = (WebControl)control;
					wc.Style.Add("width", width);
				}
				else if (control is HtmlControl)
				{
					HtmlControl hc = (HtmlControl)control;
					hc.Style.Add("width", width);
				}
			}
		}

		private void SetTargetControlHeights()
		{
			if (this.Page.IsPostBack)
			{
				Control[] targets = null;
				string height = null;

				// set the height of the controls in the 
				// LeftResizeTargets
				height = this.TopRowHeight;
				if (!string.IsNullOrEmpty(height))
				{
					targets = GetTargetControls(this.TopResizeTargets);
					if (targets != null && targets.Length > 0)
					{
						foreach (Control c in targets)
						{
							SetControlHeight(c, height);
						}
					}
				}

				// set the height of the controls in the 
				// BottomResizeTargets
				height = this.BottomRowHeight;
				if (!string.IsNullOrEmpty(height))
				{
					targets = GetTargetControls(this.BottomResizeTargets);
					if (targets != null && targets.Length > 0)
					{
						foreach (Control c in targets)
						{
							SetControlHeight(c, height);
						}
					}
				}
			}
		}

		private void SetControlHeight(Control control, string height)
		{
			if (control != null)
			{
				if (control is WebControl)
				{
					WebControl wc = (WebControl)control;
					wc.Style.Add("height", height);
				}
				else if (control is HtmlControl)
				{
					HtmlControl hc = (HtmlControl)control;
					hc.Style.Add("height", height);
				}
			}
		}
		
		private Control[] GetTargetControls(string controlIds)
		{
			// warning: the controls array that this method returns
			// may contain null values if a control is not found
			Control[] controls = null;
			string[] ids = GetTargetControlIds(controlIds);

			Control container = this.Page;
			if (this.NamingContainer != null && this.NamingContainer != this.Page)
			{
				container = this.NamingContainer;
			}

			if ( ids != null && ids.Length > 0 )
			{
				int i = 0;
				Control c = null;
				string id = null;
				controls = new Control[ids.Length];
				for ( i = 0; i < ids.Length; i++ )
				{
					id = ids[i];
					if (!string.IsNullOrEmpty(id))
					{
						c = container.FindControl(id);
						if (c != null)
						{
							controls[i] = c;
						}
					}
				}
			}
			return controls;
		}

		private string[] GetTargetControlIds(string controlIds)
		{
			string[] ids = null;
			if (!string.IsNullOrEmpty(controlIds))
			{
				ids = controlIds.Split(';');
			}
			return ids;
		}

		private void AddCompositeControls()
		{
			// save the width to a hidden field so that
			// on PostBacks the width will be available
			if (this.Orientation == SplitterBarOrientations.Vertical)
			{
				if (this.hdnWidth == null)
				{
					this.hdnWidth = new HtmlInputHidden();
					this.hdnWidth.ID = "hdnWidth";
				}
				this.Controls.Add(this.hdnWidth);

				if (this.hdnMinWidth == null)
				{
					this.hdnMinWidth = new HtmlInputHidden();
					this.hdnMinWidth.ID = "hdnMinWidth";
				}
				this.Controls.Add(this.hdnMinWidth);

				if (this.hdnMaxWidth == null)
				{
					this.hdnMaxWidth = new HtmlInputHidden();
					this.hdnMaxWidth.ID = "hdnMaxWidth";
				}
				this.Controls.Add(this.hdnMaxWidth);
			}
			else if (this.Orientation == SplitterBarOrientations.Horizontal)
			{
				if (this.hdnHeight == null)
				{
					this.hdnHeight = new HtmlInputHidden();
					this.hdnHeight.ID = "hdnHeight";
				}
				this.Controls.Add(this.hdnHeight);

				if (this.hdnMinHeight == null)
				{
					this.hdnMinHeight = new HtmlInputHidden();
					this.hdnMinHeight.ID = "hdnMinHeight";
				}
				this.Controls.Add(this.hdnMinHeight);

				if (this.hdnMaxHeight == null)
				{
					this.hdnMaxHeight = new HtmlInputHidden();
					this.hdnMaxHeight.ID = "hdnMaxHeight";
				}
				this.Controls.Add(this.hdnMaxHeight);
			}
		}

		protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			AddCompositeControls();

			if (this.Orientation == SplitterBarOrientations.Vertical)
			{
				this.hdnWidth.Value = postCollection[this.hdnWidth.UniqueID];
				this.hdnMinWidth.Value = postCollection[this.hdnMinWidth.UniqueID];
				this.hdnMaxWidth.Value = postCollection[this.hdnMaxWidth.UniqueID];
			}
			else if (this.Orientation == SplitterBarOrientations.Horizontal)
			{
				this.hdnHeight.Value = postCollection[this.hdnHeight.UniqueID]; ;
				this.hdnMinHeight.Value = postCollection[this.hdnMinHeight.UniqueID]; ;
				this.hdnMaxHeight.Value = postCollection[this.hdnMaxHeight.UniqueID]; ;
			}
			
			return true;
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string LeftColumnWidth
		{
			get 
			{
				AddCompositeControls();
				return this.hdnWidth.Value; 
			}
			set 
			{
				AddCompositeControls();
				this.hdnWidth.Value = value;
				this.ResizeTargetControls();
			}
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string RightColumnWidth
		{
			get 
			{ 
				string rcWidth = string.Empty;
				int total = this.TotalWidth;
				if ( total != 0 )
				{
					int width = 0;
					if (!string.IsNullOrEmpty(this.LeftColumnWidth))
					{
						width = Convert.ToInt32(this.LeftColumnWidth.Replace("px", string.Empty));
						width = total - width;
						width = (width < 1) ? 1 : width;
						rcWidth = width.ToString() + "px";
					}
				}
				return rcWidth;
			}
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string SaveWidthToElement
		{
			get { return _saveWidthToElement; }
			set { _saveWidthToElement = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string TopRowHeight
		{
			get
			{
				AddCompositeControls();
				return this.hdnHeight.Value;
				//string height = "100px"; // default value
				//if(!string.IsNullOrEmpty(this.hdnHeight.Value))
				//{
				//    height = this.hdnHeight.Value;
				//}
				//else if (this.MinHeight > 0)
				//{
				//    height = this.MinHeight.ToString() + "px";
				//}
				//return height;
			}
			set
			{
				AddCompositeControls();
				this.hdnHeight.Value = value;
				this.ResizeTargetControls();
			}
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string BottomRowHeight
		{
			get
			{
				string rcHeight = string.Empty;
				int total = this.TotalHeight;
				if (total != 0)
				{
					int height = 0;
					if(!string.IsNullOrEmpty(this.TopRowHeight))
					{
						height = Convert.ToInt32(this.TopRowHeight.Replace("px", string.Empty));
						height = total - height;
						height = (height < 1) ? 1 : height;
						rcHeight = height.ToString() + "px";
					}
				}
				return rcHeight;
			}
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string SaveHeightToElement
		{
			get { return _saveHeightToElement; }
			set { _saveHeightToElement = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string LeftResizeTargets
		{
			get { return _leftResizeTargets; }
			set { _leftResizeTargets = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string RightResizeTargets
		{
			get { return _rightResizeTargets; }
			set { _rightResizeTargets = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string TopResizeTargets
		{
			get { return _topResizeTargets; }
			set { _topResizeTargets = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string BottomResizeTargets
		{
			get { return _bottomResizeTargets; }
			set { _bottomResizeTargets = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public bool DynamicResizing
		{
			get { return _dynamicResizing; }
			set { _dynamicResizing = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string BackgroundColor
		{
			get { return _backgroundColor; }
			set 
			{ 
				_backgroundColor = value;
				this.Style.Add("background-color", _backgroundColor);
			}
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string BackgroundColorHilite
		{
			get { return _backgroundColorHilite; }
			set { _backgroundColorHilite = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string BackgroundColorResizing
		{
			get { return _backgroundColorResizing; }
			set { _backgroundColorResizing = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string BackgroundColorLimit
		{
			get { return _backgroundColorLimit; }
			set { _backgroundColorLimit = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string OnResizeStart
		{
			get { return _onResizeStart; }
			set { _onResizeStart = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string OnResize
		{
			get { return _onResize; }
			set { _onResize = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string OnResizeComplete
		{
			get { return _onResizeComplete; }
			set { _onResizeComplete = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string OnDoubleClick
		{
			get { return _onDoubleClick; }
			set { _onDoubleClick = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public string DebugElement
		{
			get { return _debugElement; }
			set { _debugElement = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public int MinWidth
		{
			//get { return _minWidth; }
			//set { _minWidth = value; }
			get
			{
				AddCompositeControls();
				return GetInt32(this.hdnMinWidth.Value);
			}
			set
			{
				AddCompositeControls();
				this.hdnMinWidth.Value = Convert.ToString(value) + "px";
			}
		}

        [Bindable(true)]
        [Category("Appearance")]
		public int MaxWidth
		{
			//get { return _maxWidth; }
			//set { _maxWidth = value; }
			get
			{
				AddCompositeControls();
				return GetInt32(this.hdnMaxWidth.Value);
			}
			set
			{
				AddCompositeControls();
				this.hdnMaxWidth.Value = Convert.ToString(value) + "px";
			}
		}

        [Bindable(true)]
        [Category("Appearance")]
		public int TotalWidth
		{
			get { return _totalWidth; }
			set { _totalWidth = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public int SplitterWidth
		{
			get { return _splitterWidth; }
			set { _splitterWidth = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public int MinHeight
		{
			//get { return _minHeight; }
			//set { _minHeight = value; }
			get
			{
				AddCompositeControls();
				return GetInt32(this.hdnMinHeight.Value);
			}
			set
			{
				AddCompositeControls();
				this.hdnMinHeight.Value = Convert.ToString(value) + "px";
			}
		}

        [Bindable(true)]
        [Category("Appearance")]
		public int MaxHeight
		{
			//get { return _maxHeight; }
			//set { _maxHeight = value; }
			get
			{
				AddCompositeControls();
				return GetInt32(this.hdnMaxHeight.Value);
			}
			set
			{
				AddCompositeControls();
				this.hdnMaxHeight.Value = Convert.ToString(value) + "px";
			}
		}

        [Bindable(true)]
        [Category("Appearance")]
		public int TotalHeight
		{
			get { return _totalHeight; }
			set { _totalHeight = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public int SplitterHeight
		{
			get { return _splitterHeight; }
			set { _splitterHeight = value; }
		}

        [Bindable(true)]
        [Category("Appearance")]
		public int GetInt32(string size)
		{
			int intsize = 0;
			if (!string.IsNullOrEmpty(size))
			{
				try
				{
					size = size.Replace("px", string.Empty);
					if (!string.IsNullOrEmpty(size))
					{
						intsize = Convert.ToInt32(size);
					}
				}
				catch
				{
					intsize = 0;
				}
			}
			return intsize;
		}

        [Bindable(true)]
        [Category("Appearance")]
		public SplitterBarIFrameHiding IFrameHiding
		{
			get { return _iframeHiding; }
			set { _iframeHiding = value; }
		}

		private void RegisterPageStartupScript()
		{
			StringBuilder sb = new StringBuilder();
			const string newline = "\r\n";

			sb.Append("<script type=\"text/javascript\"> <!-- ");
			sb.Append(newline);

			string id = "sbar_" + this.ClientID;

			// createNew / constructor
			sb.Append("var ");
			sb.Append(id);
			sb.Append("= VwdCmsSplitterBar.createNew(\"");
			sb.Append(this.ClientID);
			sb.Append("\",");
			if ( this.DebugElement == null )
			{
				sb.Append(" null);");
			}
			else
			{
				sb.Append("\"");
				sb.Append(this.DebugElement);
				sb.Append("\");");
			}
			sb.Append(newline);

			// set the namingContainerId
			if (this.NamingContainer != null && this.NamingContainer != this.Page )
			{
				string prefix = this.NamingContainer.ClientID
					+ this.ClientIDSeparator.ToString();

				sb.Append(id);
				sb.Append(".namingContainerId = \"");
				sb.Append(prefix);
				sb.Append("\";");
				sb.Append(newline);
			}

			// set the orientation
			sb.Append(id);
			sb.Append(".orientation = \"");
			sb.Append(this.Orientation.ToString().ToLower());
			sb.Append("\";");
			sb.Append(newline);

			// set the debugElementId
			if (!string.IsNullOrEmpty(this.DebugElement))
			{
				sb.Append(id);
				sb.Append(".debugElementId = \"");
				sb.Append(this.DebugElement);
				sb.Append("\";");
				sb.Append(newline);
			}

			if (this.Orientation == SplitterBarOrientations.Vertical)
			{
				// set the left resize target Ids
				sb.Append(id);
				sb.Append(".leftResizeTargetIds = \"");
				sb.Append(this.LeftResizeTargets);
				sb.Append("\";");
				sb.Append(newline);

				// set the right resize target Ids
				sb.Append(id);
				sb.Append(".rightResizeTargetIds = \"");
				sb.Append(this.RightResizeTargets);
				sb.Append("\";");
				sb.Append(newline);

				if (this.SplitterWidth != 6)
				{
					sb.Append(id);
					sb.Append(".splitterWidth = new Number(");
					sb.Append(this.SplitterWidth.ToString());
					sb.Append(");");
					sb.Append(newline);
				}

				if (!string.IsNullOrEmpty(this.SaveWidthToElement))
				{
					sb.Append(id);
					sb.Append(".saveWidthToElement = \"");
					sb.Append(this.SaveWidthToElement);
					sb.Append("\";");
					sb.Append(newline);
				}

				if (this.MinWidth > 0)
				{
					sb.Append(id);
					sb.Append(".minWidth = ");
					sb.Append(this.MinWidth.ToString());
					sb.Append(";");
					sb.Append(newline);
				}

				if (this.MaxWidth > 0)
				{
					sb.Append(id);
					sb.Append(".maxWidth = ");
					sb.Append(this.MaxWidth.ToString());
					sb.Append(";");
					sb.Append(newline);
				}

				if (this.TotalWidth > 0)
				{
					sb.Append(id);
					sb.Append(".totalWidth = ");
					sb.Append(this.TotalWidth.ToString());
					sb.Append(";");
					sb.Append(newline);
				}
			}
			else if (this.Orientation == SplitterBarOrientations.Horizontal)
			{
				// set the top resize target Ids
				sb.Append(id);
				sb.Append(".topResizeTargetIds = \"");
				sb.Append(this.TopResizeTargets);
				sb.Append("\";");
				sb.Append(newline);

				// set the bottom resize target Ids
				sb.Append(id);
				sb.Append(".bottomResizeTargetIds = \"");
				sb.Append(this.BottomResizeTargets);
				sb.Append("\";");
				sb.Append(newline);

				if (this.SplitterHeight != 6)
				{
					sb.Append(id);
					sb.Append(".splitterHeight = new Number(");
					sb.Append(this.SplitterHeight.ToString());
					sb.Append(");");
					sb.Append(newline);
				}

				if (!string.IsNullOrEmpty(this.SaveHeightToElement))
				{
					sb.Append(id);
					sb.Append(".saveHeightToElement = \"");
					sb.Append(this.SaveHeightToElement);
					sb.Append("\";");
					sb.Append(newline);
				}

				if (this.MinHeight > 0)
				{
					sb.Append(id);
					sb.Append(".minHeight = ");
					sb.Append(this.MinHeight.ToString());
					sb.Append(";");
					sb.Append(newline);
				}

				if (this.MaxHeight > 0)
				{
					sb.Append(id);
					sb.Append(".maxHeight = ");
					sb.Append(this.MaxHeight.ToString());
					sb.Append(";");
					sb.Append(newline);
				}

				if (this.TotalHeight > 0)
				{
					sb.Append(id);
					sb.Append(".totalHeight = ");
					sb.Append(this.TotalHeight.ToString());
					sb.Append(";");
					sb.Append(newline);
				}
			}

			// IFrameHiding
			sb.Append(id);
			sb.Append(".iframeHiding = \"");
			sb.Append(this.IFrameHiding.ToString());
			sb.Append("\";");
			sb.Append(newline);


			if (!string.IsNullOrEmpty(this.OnResizeStart))
			{
				sb.Append(id);
				sb.Append(".onResizeStart = ");
				sb.Append(this.OnResizeStart);
				sb.Append(";");
				sb.Append(newline);
			}

			if (!string.IsNullOrEmpty(this.OnResize))
			{
				sb.Append(id);
				sb.Append(".onResize = ");
				sb.Append(this.OnResize);
				sb.Append(";");
				sb.Append(newline);
			}

			if (!string.IsNullOrEmpty(this.OnResizeComplete))
			{
				sb.Append(id);
				sb.Append(".onResizeComplete = ");
				sb.Append(this.OnResizeComplete);
				sb.Append(";");
				sb.Append(newline);
			}

			if (!string.IsNullOrEmpty(this.OnDoubleClick))
			{
				sb.Append(id);
				sb.Append(".OnDoubleClick = ");
				sb.Append(this.OnDoubleClick);
				sb.Append(";");
				sb.Append(newline);
			}

			if (this.DynamicResizing)
			{
				sb.Append(id);
				sb.Append(".liveResize = true;");
				sb.Append(newline);
			}
			if (!string.IsNullOrEmpty(this.BackgroundColor))
			{
				sb.Append(id);
				sb.Append(".SetBackgroundColor(\"");
				sb.Append(this.BackgroundColor);
				sb.Append("\");");
				sb.Append(newline);
			}

			if (!string.IsNullOrEmpty(this.BackgroundColorHilite))
			{
				sb.Append(id);
				sb.Append(".backgroundColorHilite = \"");
				sb.Append(this.BackgroundColorHilite);
				sb.Append("\";");
				sb.Append(newline);
			}
			if (!string.IsNullOrEmpty(this.BackgroundColorResizing))
			{

				sb.Append(id);
				sb.Append(".backgroundColorResizing = \"");
				sb.Append(this.BackgroundColorResizing);
				sb.Append("\";");
				sb.Append(newline);
			}
			if (!string.IsNullOrEmpty(this.BackgroundColorLimit))
			{

				sb.Append(id);
				sb.Append(".backgroundColorLimit = \"");
				sb.Append(this.BackgroundColorLimit);
				sb.Append("\";");
				sb.Append(newline);
			}


			// do this last...
			// be sure to call configure after all of the 
			// configuration properties have been set
			sb.Append(id);
			sb.Append(".configure();");
			sb.Append(newline);

			sb.Append("// -->");
			sb.Append(newline);
			sb.Append("</script>");
			sb.Append(newline);

			this.Page.ClientScript.RegisterStartupScript(this.GetType(),
				this.ClientID + "_VwdCmsSplitterBarStartupScript", sb.ToString());
		}

		// IPostBackDataHandler Members

		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			return this.LoadPostData(postDataKey, postCollection);
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent()
		{
			//not implemented
		}
	}
}