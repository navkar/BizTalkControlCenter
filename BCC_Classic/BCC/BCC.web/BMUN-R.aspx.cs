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
/// User Notification Report 
/// </summary>
public partial class UserNotificationReport : System.Web.UI.Page
{
    private string artifactType = string.Empty;
    private string artifactName = string.Empty;
    private string textToDraw = string.Empty;

    private enum Frequency
    {
        DAILY = 0, WEEKLY = 1, MONTHLY = 2, YEARLY = 3
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Page Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        artifactType = this.Request.QueryString["AT"];
        artifactName = this.Request.QueryString["AN"];

        this.Page.Title = "Report for " + artifactType + " - " + artifactName;
        this.chartHeader.Text = "Report for " + artifactType + " - " + artifactName; 

        if (!Page.IsPostBack)
        {
            if (User.IsInRole(BCCUIHelper.Constants.ROLE_ADMIN) || User.IsInRole(BCCUIHelper.Constants.ROLE_ARTIFACT))
            {
                DisplayChart(artifactType, artifactName, Frequency.DAILY);
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
        if (artifactReport.Series.Count == 0)
        {
            // create text to draw
            if (textToDraw.Equals(string.Empty))
            {
                textToDraw = "No events reported as of " + String.Format("{0:f}", DateTime.UtcNow);
            } 
            
            // get graphics tools
            System.Drawing.Graphics g = e.ChartGraphics.Graphics;
            System.Drawing.Font drawFont = System.Drawing.SystemFonts.CaptionFont;
            System.Drawing.Brush drawBrush = System.Drawing.Brushes.Gray;
            // see how big the text will be
            int txtWidth = (int)g.MeasureString(textToDraw, drawFont).Width;
            int txtHeight = (int)g.MeasureString(textToDraw, drawFont).Height;
            // where to draw
            int x = 150;  // a few pixels from the left border
            int y = (int)e.Chart.Height.Value;
            y = y - txtHeight - 200; // a few pixels off the bottom
            // draw the string        
            g.DrawString(textToDraw, drawFont, drawBrush, x, y);
        }
    }

    protected void frequency_OnSelectedIndexChanged(object source, EventArgs e)
    {
        this.chartFooter.Text = this.frequency.SelectedItem.ToString() + " view";

        Frequency enumFrequency = (Frequency)Enum.Parse(typeof(Frequency), this.frequency.SelectedValue.ToString());

        DisplayChart(artifactType, artifactName, enumFrequency);
    }

    private void DisplayChart(string artifactType, string artifactName, Frequency frequency)
    {
        string[] daysOfWeek = new string[7];
        string[] monthsInAnYear = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "July", "Aug", "Sept", "Oct", "Nov", "Dec" };
        List<BCCMonitoringReportEntry> list = null;
        Series series = null;
        BCC.Core.WMI.BizTalk.ArtifactType enumArtifactType = (BCC.Core.WMI.BizTalk.ArtifactType)Enum.Parse(typeof(BCC.Core.WMI.BizTalk.ArtifactType), artifactType);
        BCCMonitoringDataAccess da = new BCCMonitoringDataAccess();
        ArrayList seriesList = new ArrayList();

        switch (frequency)
        {
            case Frequency.DAILY:
#region Daily
                list = da.MonitoringDataReport(enumArtifactType, artifactName, DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1));

                // Build series based on the data received!
                foreach (BCCMonitoringReportEntry entry in list)
                {
                    // Create a new series in case you notice a new Artifact status
                    if (!(seriesList.Contains(entry.ArtifactStatus)))
                    {
                        // Add it to the series list before creating new series
                        seriesList.Add(entry.ArtifactStatus);

                        series = new Series();
                        series.Name = entry.ArtifactStatus;
                        series.ChartType = SeriesChartType.StackedColumn;
                        series.LegendText = entry.ArtifactStatus;
                        series.LegendToolTip = entry.ArtifactStatus;
                        series.XValueType = ChartValueType.Time;
                        series.YValueType = ChartValueType.Int32;
                        series.IsValueShownAsLabel = true;
                        series["DrawingStyle"] = "Emboss";

                        artifactReport.Series.Add(series);
                    }
                }


                // Build series based on the data received!
                foreach (BCCMonitoringReportEntry entry in list)
                {
                    object reportedDate = String.Format("{0:T}", entry.ReportedDate);

                    artifactReport.Series[entry.ArtifactStatus].Points.AddXY(reportedDate, 1);
                }

#endregion
                break;

            case Frequency.WEEKLY:
#region Weekly

                int[] seriesData = new int[daysOfWeek.Length];
                Hashtable htSeries = null;
                // Dynamically constructed days of week list based on today. (Today must be on the right most side)
                for (int count = 0; count < daysOfWeek.Length; count++)
                {
                    daysOfWeek[count] = DateTime.UtcNow.Subtract(new TimeSpan(6 - count, 0, 0, 0)).DayOfWeek.ToString();
                    // Initializing series data.
                    seriesData[count] = 0;
                }

                // Get the data for a week, starting from (6 days from today) until today.
                list = da.MonitoringDataReport(enumArtifactType, artifactName, DateTime.UtcNow.Subtract(new TimeSpan(6, 0, 0, 0)), DateTime.UtcNow.AddDays(1));

                htSeries = new Hashtable();
                // Build series based on the data received!
                foreach (BCCMonitoringReportEntry entry in list)
                {
                    // Create a new series in case you notice a new Artifact status
                    if (!(seriesList.Contains(entry.ArtifactStatus)))
                    {
                        // Add it to the series list before creating new series
                        seriesList.Add(entry.ArtifactStatus);

                        series = new Series();
                        series.Name = entry.ArtifactStatus;
                        series.ChartType = SeriesChartType.StackedColumn;
                        series.LegendText = entry.ArtifactStatus;
                        series.LegendToolTip = entry.ArtifactStatus;
                        series.XValueType = ChartValueType.String;
                        series.YValueType = ChartValueType.Int32;
                        series["DrawingStyle"] = "Emboss";
                        //series.IsValueShownAsLabel = true;
                        // Refer - http://blogs.msdn.com/b/alexgor/archive/2008/11/11/microsoft-chart-control-how-to-using-keywords.aspx
                        series.ToolTip = "On #VALX\nArtifact was #SERIESNAME #VALY time(s).";

                        // Add the series to the chart! 
                        artifactReport.Series.Add(series);
                        
                        //artifactReport.AlignDataPointsByAxisLabel(series.Name, PointSortOrder.Ascending);
                    }

                    if (seriesList.Contains(entry.ArtifactStatus))
                    {
                            object htData = htSeries[entry.ArtifactStatus]; 

                            if (htData != null)
                            {
                                // Get the data from hashtable and update it. 
                                int[] htSeriesData = (int[]) htData;

                                for (int index = 0; index < daysOfWeek.Length; index++)
                                {
                                    if (entry.ReportedDate.DayOfWeek == DateTime.UtcNow.Subtract(new TimeSpan(index, 0, 0, 0)).DayOfWeek)
                                    {
                                        // Increment by 1.
                                        htSeriesData[(daysOfWeek.Length - 1) - index] += 1;
                                        break;
                                    }
                                }

                                htSeries.Remove(entry.ArtifactStatus);
                                htSeries.Add(entry.ArtifactStatus, htSeriesData.Clone() );
                            }
                            else
                            {
                                for (int index = 0; index < daysOfWeek.Length; index++)
                                {
                                    if (entry.ReportedDate.DayOfWeek == DateTime.UtcNow.Subtract(new TimeSpan(index, 0, 0, 0)).DayOfWeek)
                                    {
                                        // Initialize to 1.
                                        seriesData[(daysOfWeek.Length - 1) - index] = 1;
                                        // Hashtable holds the series data
                                        htSeries.Add(entry.ArtifactStatus, seriesData.Clone());
                                        // Reset the value
                                        seriesData[(daysOfWeek.Length - 1) - index] = 0;
                                        break;
                                    }
                                }
                                
                            }
                    }
                }
                    
                for (int count = 0; count < htSeries.Count; count++)
                {
                    seriesData = (int[]) htSeries[seriesList[count]];
                    artifactReport.Series[count].Points.DataBindXY(daysOfWeek, seriesData);
                }
#endregion
                break;

            case Frequency.MONTHLY:
#region Monthly

                int[] monthlySeriesData = new int[monthsInAnYear.Length];
                Hashtable htYrSeries = null;

                for (int count = 0; count < monthsInAnYear.Length; count++)
                {
                    // Initializing series data.
                    monthlySeriesData[count] = 0;
                }

                // Get the data for a year, starting from (365 days from today) until today.
                list = da.MonitoringDataReport(enumArtifactType, artifactName, DateTime.UtcNow.Subtract(new TimeSpan(365, 0, 0, 0)), DateTime.UtcNow.AddDays(1));

                htYrSeries = new Hashtable();
                
                // Build series based on the data received!
                foreach (BCCMonitoringReportEntry entry in list)
                {
                    // Create a new series in case you notice a new Artifact status
                    if (!(seriesList.Contains(entry.ArtifactStatus)))
                    {
                        // Add it to the series list before creating new series
                        seriesList.Add(entry.ArtifactStatus);

                        series = new Series();
                        series.Name = entry.ArtifactStatus;
                        series.ChartType = SeriesChartType.StackedColumn;
                        series.LegendText = entry.ArtifactStatus;
                        series.LegendToolTip = entry.ArtifactStatus;
                        series.XValueType = ChartValueType.String;
                        series.YValueType = ChartValueType.Int32;
                        series["DrawingStyle"] = "Emboss";
                        //series.IsValueShownAsLabel = true;
                        // Refer - http://blogs.msdn.com/b/alexgor/archive/2008/11/11/microsoft-chart-control-how-to-using-keywords.aspx
                        series.ToolTip = "In #VALX\nArtifact was #SERIESNAME #VALY time(s).";

                        // Add the series to the chart! 
                        artifactReport.Series.Add(series);
                    }

                    if (seriesList.Contains(entry.ArtifactStatus))
                    {
                        object htData = htYrSeries[entry.ArtifactStatus]; 

                            if (htData != null)
                            {
                                // Get the data from hashtable and update it. 
                                int[] htSeriesData = (int[]) htData;

                                for (int index = 0; index < monthsInAnYear.Length; index++)
                                {
                                    if (entry.ReportedDate.Month == (index + 1))
                                    {
                                        // Increment by 1.
                                        htSeriesData[index] += 1;
                                        break;
                                    }
                                }

                                htYrSeries.Remove(entry.ArtifactStatus);
                                htYrSeries.Add(entry.ArtifactStatus, htSeriesData.Clone());
                            }
                            else
                            {
                                for (int index = 0; index < monthsInAnYear.Length; index++)
                                {
                                    if (entry.ReportedDate.Month == (index + 1))
                                    {
                                        // Initialize to 1.
                                        monthlySeriesData[index] = 1;
                                        // Hashtable holds the series data
                                        htYrSeries.Add(entry.ArtifactStatus, monthlySeriesData.Clone());
                                        // Reset the value
                                        monthlySeriesData[index] = 0;
                                        break;
                                    }
                                }
                                
                            }
                    }
                }

                for (int count = 0; count < htYrSeries.Count; count++)
                {
                    monthlySeriesData = (int[])htYrSeries[seriesList[count]];
                    artifactReport.Series[count].Points.DataBindXY(monthsInAnYear, monthlySeriesData);
                }
#endregion
                break;

        }

    }

}
