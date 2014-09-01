using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Data;
using System.Threading;

namespace BCC.Core
{
    public class BCCPerformanceCounters
    {
        private string TRACE_CATEGORY = "BCCPerformanceCounters";

        public BCCPerformanceCounters()
        {

        }

        public DataTable EnumeratePerformanceCounters()
        {
            DataTable dt = new DataTable();
            DataRow dr = null;

            // define the table's schema
            dt.Columns.Add(new DataColumn("PerfCategoryName", typeof(string)));
            dt.Columns.Add(new DataColumn("PerfCounterName", typeof(string)));
            dt.Columns.Add(new DataColumn("PerfInstanceName", typeof(string)));
            dt.Columns.Add(new DataColumn("PerfCategoryDesc", typeof(string)));

            PerformanceCounterCategory[] perfCounterCats = PerformanceCounterCategory.GetCategories();
            PerformanceCounterCategory perfCounterCat = null;
            PerformanceCounter[] perfCounters = null;

            // PerformanceCounterCategoryType - MultiInstance, SingleInstance, Unknown
            string catName = string.Empty;
            string catDesc = string.Empty;
            string counterName = string.Empty;
            string[] instanceNames = null;

            for (int count = 0; count < perfCounterCats.Length; count++)
            {
                catName = perfCounterCats[count].CategoryName;
                catDesc = perfCounterCats[count].CategoryHelp;

                perfCounterCat = new PerformanceCounterCategory(catName);

                instanceNames = perfCounterCat.GetInstanceNames();

                if (instanceNames.Length > 0)
                {
                    foreach (string instanceName in instanceNames)
                    {
                        try
                        {
                            if (perfCounterCat.InstanceExists(instanceName))
                            {
                                perfCounters = perfCounterCat.GetCounters(instanceName);

                                foreach (PerformanceCounter perfCounter in perfCounters)
                                {
                                    counterName = perfCounter.CounterName;

                                    dr = dt.NewRow();
                                    dr[0] = catName;
                                    dr[1] = counterName;
                                    dr[2] = instanceName;
                                    dr[3] = catDesc;
                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.Write(ex.Message + ex.StackTrace, TRACE_CATEGORY);
                        }
                    }
                }
                else
                {
                    foreach (PerformanceCounter perfCounter in perfCounterCat.GetCounters())
                    {
                        counterName = perfCounter.CounterName;

                        dr = dt.NewRow();
                        dr[0] = catName;
                        dr[1] = counterName;
                        dr[2] = BCCUIHelper.Constants.SC303_INSTANCE_NOT_FOUND;
                        dr[3] = catDesc;
                        dt.Rows.Add(dr);
                    }
                }
            }

            return dt;
        }

        public static bool IsPerformanceCounterInstanceValid(string category, string instance)
        {
            try
            {
                return PerformanceCounterCategory.InstanceExists(instance, category);
            }
            catch
            {
                return false;
            }
        }
        
        public static float Monitor(string category, string counter, string instance)
        {
            if (!PerformanceCounterCategory.Exists(category))
            {
                throw new InvalidOperationException("Category '" + category + "' does not exist.");
            }
            else
                if (!PerformanceCounterCategory.CounterExists(counter, category))
                {
                    throw new InvalidOperationException("For category '" + category + ", the performance counter  '" + counter + "' does not exist.");
                }
                else
                    if (instance != null && instance != string.Empty && !PerformanceCounterCategory.InstanceExists(instance, category))
                    {
                        throw new InvalidOperationException("For performance counter '" + counter + "', the instance '" + instance + "'does not exist.");
                    }

            float nextValue = 0f;

            using (PerformanceCounter pc = new PerformanceCounter(category, counter, instance))
            {
                nextValue = pc.NextValue();
            }

            return nextValue;
        }

    }
}
