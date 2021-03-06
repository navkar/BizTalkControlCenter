using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;

using BCC.Core;

/// <summary>
/// Performance Counter Report
/// </summary>
public partial class PerformanceCounterReport : System.Web.UI.Page
{
    private string categoryName = string.Empty;
    private string counterName = string.Empty;
    private string instanceName = string.Empty;
    private string pollingInterval = string.Empty;
    private int numOfDataPoints = 0;
    private string textToDraw = string.Empty;
    /// <summary>
    /// Page - Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        int interval = 1000;

        categoryName = this.Request.QueryString["CAT"];
        counterName = this.Request.QueryString["CNTR"];
        instanceName = this.Request.QueryString["INST"];
        pollingInterval = this.Request.QueryString["POLL"];

        this.Page.Title = "Real time performance counter chart";
        this.chartHeader.Text = categoryName + " - " + counterName;

        int index = pollingInterval.IndexOf("secs");
        if (index != -1)
        {
            pollingInterval = pollingInterval.Substring(0, index);
        }

        Int32.TryParse(pollingInterval, out interval);
        chartTimer.Interval = interval * 1000;

        // Number of Data Points
        Int32.TryParse(this.dataPoints.SelectedValue, out numOfDataPoints);

        if (!Page.IsPostBack)
        {
            if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
            {
                DisplayChart(categoryName, counterName, instanceName, numOfDataPoints);
            }
            else
            {
                textToDraw = "You must be either in 'Admin' or 'Artifact' role to view the chart.";
            }
        }
    }

    /// <summary>
    /// Post Paint - Empty Chart
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void PostPaint_EmptyChart(object sender, ChartPaintEventArgs e)
    {
        // If the series count is 0, this means that there is no data.
        if (chartPerfCounter.Series.Count == 0)
        {
            // create text to draw
            if (textToDraw.Equals(string.Empty))
            {
                textToDraw = "No performance counter data reported as of " + String.Format("{0:f}", DateTime.UtcNow);
            }
            // get graphics tools
            System.Drawing.Graphics g = e.ChartGraphics.Graphics;
            System.Drawing.Font drawFont = System.Drawing.SystemFonts.CaptionFont;
            System.Drawing.Brush drawBrush = System.Drawing.Brushes.Gray;
            // see how big the text will be
            int txtWidth = (int)g.MeasureString(textToDraw, drawFont).Width;
            int txtHeight = (int)g.MeasureString(textToDraw, drawFont).Height;
            // where to draw
            int x = 75;  // a few pixels from the left border
            int y = (int)e.Chart.Height.Value;
            y = y - txtHeight - 200; // a few pixels off the bottom
            // draw the string        
            g.DrawString(textToDraw, drawFont, drawBrush, x, y);
        }
    }

    /// <summary>
    /// Display - Chart
    /// </summary>
    /// <param name="categoryName">Category Name</param>
    /// <param name="counterName">Counter Name</param>
    /// <param name="instanceName">Instance Name</param>
    /// <param name="numOfDataPoints">No of data points</param>
    private void DisplayChart(string categoryName, string counterName, string instanceName, int numOfDataPoints)
    {
        BCCPerfCounterDataAccess da = new BCCPerfCounterDataAccess();
        List<BCCPerfCounterReportEntry> counterList = da.PerformanceCounterDataReport(categoryName, counterName, instanceName, 0);

        // Build the series.
        Series series = new Series();
        series.Name = counterName + instanceName;
        series.ChartType = SeriesChartType.Point;
        series.LegendText = counterName;
        series.LegendToolTip = categoryName + " - " + counterName + "(" + instanceName + ")";
        series.XValueType = ChartValueType.Time;
        series.YValueType = ChartValueType.Int32;
        series.IsValueShownAsLabel = true;
        series["DrawingStyle"] = "Emboss";

        chartPerfCounter.Series.Add(series);

        foreach (BCCPerfCounterReportEntry entry in counterList)
        {
            chartPerfCounter.Series[0].Points.AddXY(String.Format("{0:T}", entry.ReportedDate), entry.PerformanceCounterValue);
        }

        while (this.chartPerfCounter.Series[0].Points.Count > numOfDataPoints)
        {
            chartPerfCounter.Series[0].Points.RemoveAt(0);
        }
    }

    protected void ChartTimer_Tick(object sender, EventArgs e)
    {
        DisplayChart(categoryName, counterName, instanceName, numOfDataPoints);
    }

    protected void dataPoints_OnSelectedIndexChanged(object source, EventArgs e)
    {
        Int32.TryParse(this.dataPoints.SelectedValue, out numOfDataPoints);
        DisplayChart(categoryName, counterName, instanceName, numOfDataPoints);
    }
}