using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Đồ_Án_Web.Others
{
    public class XuLy
    {
        public const string VERSION = "2.1.0";
        private SortedList<String, String> _requestData = new SortedList<String, String>(new SoSanh());
        private SortedList<String, String> _responseData = new SortedList<String, String>(new SoSanh());


        //---------------------------------------------------YÊU CẦU 1 DATA-----------------------------------------
        public void AddRequestData(string key, string value)
        {
            //Nếu chuỗi không rỗng
            if (!String.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }
        //Tạo 1 url yêu cầu 
        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
               if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            string queryString = data.ToString();
            baseUrl += "?" + queryString;
            String signData = queryString;
            if (signData.Length > 0)
            {

                signData = signData.Remove(data.Length - 1, 1);
            }
            string vnp_SecureHash = ChuyenDoi.HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            return baseUrl;
        }


        //--------------------------------------TRẢ DỮ LIỆU VỀ------------------------------------------------------
        public void AddResponseData(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            string retValue;
            if (_responseData.TryGetValue(key, out retValue))
            {
                return retValue;
            }
            else
            {
                return string.Empty;
            }
        }
        public bool ValidateSignature(string inputHash, string secretKey)
        {
            string rspRaw = GetResponseData();
            string myChecksum = ChuyenDoi.HmacSHA512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
        private string GetResponseData()
        {

            StringBuilder data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }
            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }
            foreach (KeyValuePair<string, string> kv in _responseData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }
            return data.ToString();
        }
    }
}