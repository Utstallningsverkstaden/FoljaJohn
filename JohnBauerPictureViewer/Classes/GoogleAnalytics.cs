using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CC.Utilities
{
    public enum HitType
    {
        Event,
        Exception,
        Item,
        PageView,
        Social,
        Timing,
        Transaction,
        ScreenView
    }

    public enum TimingType
    {
        UserTimingTime,
        PageLoadTime,
        DNSTime,
        PageDownloadTime,
        RedirectResponseTime,
        TCPConnectTime,
        ServerResponseTime,
        DOMInteractiveTime,
        ContentLoadTime
    }

    public class GoogleAnalytics
    {
        private const string TrackerID = "UA-27454476-6";
        private static string _DataSource = "app";
        private static string _Language;
        private static string _ScreenResolution;
        private static string _ViewportSize;
        private static string _DocumentLocationURL;
        private static string _DocumentHostName = "spree.se";
        private static string _DocumentPath = "%2Fanalytics";
        private static string _DocumentTitle = "analyze";
        private static string _ExperimentID;
        private static int _ExperimentVariant;

        private static string StoredAppName { get; set; }
        private static string StoredVersion { get; set; }

        public static string GoogleAnalyticsCID { get; set; }
        public static string DataSource { get { return _DataSource; } set { _DataSource = value; } }
        public static string Language { get { return _Language; } set { _Language = value; } }
        public static string ScreenResolution { get { return _ScreenResolution; } set { _ScreenResolution = value; } }
        public static string ViewportSize { get { return _ViewportSize; } set { _ViewportSize = value; } }
        public static string DocumentLocationURL { get { return _DocumentLocationURL; } set { _DocumentLocationURL = value; } }
        public static string DocumentHostName { get { return _DocumentHostName; } set { _DocumentHostName = value; } }
        public static string DocumentPath { get { return _DocumentPath; } set { _DocumentPath = value; } }
        public static string DocumentTitle { get { return _DocumentTitle; } set { _DocumentTitle = value; } }
        public static string ExperimentID { get { return _ExperimentID; } set { _ExperimentID = value; } }
        public static int ExperimentVariant { get { return _ExperimentVariant; } set { _ExperimentVariant = value; } }


        ///  http://www.google-analytics.com/collect?v=1&tid=UA-XXXXXXXX-X&cid=d8e5ad8d-54d3-4f16-901c-a05ddbe35d17&av=1.0&t=appview&cd=StartOfApp&z=987059828

        public static void ApplicationStart(string AppName, string Version)
        {
            StoredAppName = AppName;
            StoredVersion = Version;

            string Payload = BasicPayLoad() +
                             AppNameParameter(StoredAppName) +
                             VersionParameter(StoredVersion) +
                             HitTypeToString(HitType.Event) +
                             "&ec=app&ea=start&ev=1&el=program";

            SendDataToGoogleAnalytics(Payload);
        }
        public static void PageView(string PageName)
        {
            string Payload = HitTypeToString(HitType.PageView) +
                             ScreenNameParameter(PageName);
            SendPayload(Payload);
        }
        public static void SendException(string Message)
        {
            string Payload = HitTypeToString(HitType.Exception) +
                             ExceptionParameter(Message);
            SendPayload(Payload);
        }
        public static void SendTiming(string Category, string Variable, string Label, TimingType timingType, int Milliseconds)
        {
            string Payload = HitTypeToString(HitType.Timing) +
                             "&utc=" + Category + // User timing category
                             "&utv=" + Variable + // User timing variable name
                             "&utl=" + Label; // User timing label

            switch (timingType)
            {
                case TimingType.UserTimingTime:
                    Payload += "&utt=" + Milliseconds.ToString(); // User timing time
                    break;
                case TimingType.PageLoadTime:
                    Payload += "&plt=" + Milliseconds.ToString(); // Page Load Time
                    break;
                case TimingType.DNSTime:
                    Payload += "&dns=" + Milliseconds.ToString(); // DNS Time
                    break;
                case TimingType.PageDownloadTime:
                    Payload += "&pdt=" + Milliseconds.ToString(); // Page Download Time
                    break;
                case TimingType.RedirectResponseTime:
                    Payload += "&rrt=" + Milliseconds.ToString(); // Redirect Response Time
                    break;
                case TimingType.TCPConnectTime:
                    Payload += "&tcp=" + Milliseconds.ToString(); // TCP Connect Time
                    break;
                case TimingType.ServerResponseTime:
                    Payload += "&srt=" + Milliseconds.ToString(); // Server Response Time
                    break;
                case TimingType.DOMInteractiveTime:
                    Payload += "&dit=" + Milliseconds.ToString(); // DOM Interactive Time
                    break;
                case TimingType.ContentLoadTime:
                    Payload += "&clt=" + Milliseconds.ToString(); // Content Load Time
                    break;
            }
            SendPayload(Payload);
        }


        #region Private functions
        private static string ExceptionParameter(string Message)
        {
            return "&exd=" + Message + "&exf=0";
        }
        private static void SendPayload(string Payload)
        {
            Payload = BasicPayLoad() +
                             AppNameParameter(StoredAppName) +
                             VersionParameter(StoredVersion) +
                             Payload;

            SendDataToGoogleAnalytics(Payload);
        }
        private static string AppNameParameter(string AppName)
        {
            return "&an=" + AppName;

        }
        private static string VersionParameter(string Version)
        {
            return "&av=" + Version;
        }
        private static string UserLanguageParameter(string Language)
        {
            return "&ul=" + Language;
        }
        private static string ScreenNameParameter(string ScreenName)
        {
            return "&cd=" + ScreenName;
        }
        private static string ExceptionDescriptionParameter(string Description)
        {
            return "&exd=" + Description;
        }
        private static string LanguageParameter(string Language)
        {
            return "&ul=" + Language;
        }
        private static async void SendDataToGoogleAnalytics(string PayLoad)
        {
            string URL = GoogleAnalyticsURL();
            string CacheBuster = GetCacheBuster();

            HttpClient client = new HttpClient();

            string TotalURL = URL + PayLoad + CacheBuster;

            StringContent queryString = new StringContent("");

            var Result = await client.PostAsync(new Uri(TotalURL, UriKind.RelativeOrAbsolute), queryString);
            //var Result = await client.GetAsync(new Uri(TotalURL, UriKind.RelativeOrAbsolute));//, queryString);
        }
        private static string GetCacheBuster()
        {
            Random R = new Random(DateTime.Now.Millisecond);
            return "&z=" + R.Next().ToString();
        }
        private static string BasicPayLoad()
        {
            string cid = GetCid();
            string PayLoad = "?v=1&tid=" + TrackerID + "&cid=" + cid + "&ds=" + DataSource;

            if ((Language != null) && (Language.Length > 0))
            {
                PayLoad += LanguageParameter(Language);
            }
            if ((DocumentHostName != null) && (DocumentHostName.Length > 0))
            {
                PayLoad += "&dh=" + DocumentHostName;
            }
            if ((DocumentPath != null) && (DocumentPath.Length > 0))
            {
                PayLoad += "&dp=" + DocumentPath;
            }
            if ((DocumentTitle != null) && (DocumentTitle.Length > 0))
            {
                PayLoad += "&dt=" + DocumentTitle;
            }
            if ((ExperimentID != null) && (ExperimentID.Length > 0))
            {
                PayLoad += "&xid=" + ExperimentID;

                if (ExperimentVariant != null)
                {
                    PayLoad += "&xvar=" + ExperimentVariant.ToString();
                }
            }

            return PayLoad;
        }
        private static string GoogleAnalyticsURL()
        {
            string URL = "https://ssl.google-analytics.com/collect";
            return URL;
        }
        private static string GetCid()
        {
            string cid = "";

            if ((GoogleAnalyticsCID != null) && (GoogleAnalyticsCID.Length > 0))
            {
                cid = GoogleAnalyticsCID;
            }
            else
            {
                cid = Guid.NewGuid().ToString();
            }
            return cid;
        }
        private static string HitTypeToString(HitType hit_type)
        {
            string ParameterName = "&t=";
            switch (hit_type)
            {
                case HitType.PageView:
                    return ParameterName + "pageview";
                case HitType.ScreenView:
                    return ParameterName + "pageview";
                case HitType.Event:
                    return ParameterName + "event";
                case HitType.Transaction:
                    return ParameterName + "transaction";
                case HitType.Item:
                    return ParameterName + "item";
                case HitType.Social:
                    return ParameterName + "social";
                case HitType.Exception:
                    return ParameterName + "exception";
                case HitType.Timing:
                    return ParameterName + "timing";

            }
            return ParameterName + "pageview";
        }
        #endregion
    }
}
