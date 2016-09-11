using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHOffice.Web.Models.Bill
{
    public class ConfirmImportInternalNoModel
    {
        public class Item
        {
            [JsonProperty(PropertyName = "bid")]
            public long Bid { get; set; }
            [JsonIgnore]
            public string No { get; set; }
            [JsonProperty(PropertyName = "i_no")]
            public string InternalNo { get; set; }
            [JsonProperty(PropertyName = "i_express")]
            public string InternalExpress { get; set; }
            [JsonProperty(PropertyName = "remarks")]
            public string Remarks { get; set; }
            [JsonProperty(PropertyName = "updated")]
            public DateTime? UpdateStateDate { get; set; }
            [JsonIgnore]
            public bool IsEnabled
            {
                get
                {
                    return !String.IsNullOrWhiteSpace(No)
                        && !String.IsNullOrWhiteSpace(InternalNo)
                        && !String.IsNullOrWhiteSpace(InternalExpress)
                        && Bid > 0;
                }
            }
            [JsonIgnore]
            public string ErrorMessage
            {
                get
                {
                    if (IsEnabled)
                        return String.Empty;
                    else
                    {
                        var msgs = new List<string>();
                        if (String.IsNullOrWhiteSpace(No))
                            msgs.Add("未填写运单号");
                        else if (Bid <= 0)
                            msgs.Add("此订单号不存在或者无权限更新");
                        if (String.IsNullOrWhiteSpace(InternalNo))
                            msgs.Add("未填写国内运单号");
                        if (String.IsNullOrWhiteSpace(InternalExpress))
                            msgs.Add("未填写国内快递");
                        return String.Join(",", msgs);
                    }
                }
            }
        }

        public int EnabledItemsLength
        {
            get
            {
                return Items == null ? 0 : Items.Where(i => i.IsEnabled).Count();
            }
        }

        public Item[] Items { get; set; }
    }
}