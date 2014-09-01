using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;

/// <summary>
/// Summary description for BCCFilterList
/// </summary>
namespace BCC.Core
{
    class BCCFilterList : CollectionBase
    {
        public ArrayList mInnerArrayList;

        public BCCFilterList()
        {
            mInnerArrayList = ArrayList.Adapter(this.InnerList);
        }

        public BCCFilter this[int index]
        {
            get { return (BCCFilter)InnerList[index]; }
            set { InnerList[index] = value; }
        }

        public int Add(BCCFilter value)
        {
            return InnerList.Add(value);
        }

        public void Remove(BCCFilter value)
        {
            InnerList.Remove(value);
        }

        public void Sort()
        {
            mInnerArrayList.Sort();
        }

        protected override void OnValidate(object value)
        {
            base.OnValidate(value);
            if (!(value is BCCFilter)) throw new ArgumentException("Type of the  value is not acceptable");
        }
    }




}
