using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace RenLianShiBie
{


    public class RemoteManager
    {
        NetworkManager mNetworkManager = new NetworkManager();
        string mErrorStr = "";
        string AuthAddress;
        int AuthPort;

        // Connect to device via network
        public bool Connect(String address, int port , string password)
        {
            if (!ConnectionCheck(address, port))
            {
                mErrorStr = mNetworkManager.ErrorStr;
                AuthAddress = "";
                return false;
            }
            else
            {
                AuthAddress = address;
                AuthPort = port;
            }

            return true;
        }

        public string GetErrorString()
        {
            string szErr = mErrorStr;
            mErrorStr = "";
            return szErr;
        }

#if DEBUG
        public void SelfTest()
        {
            MemoryStream headerStream = new MemoryStream();

            XmlWriter writer = XmlWriter.Create(headerStream);
            writer.WriteStartElement("Hysoon");
            writer.WriteStartAttribute("Request");
            writer.WriteValue("Response");
            writer.WriteEndAttribute();
            writer.WriteElementString("Session", "123456798");
            writer.WriteEndElement();
            writer.Flush();

            //string testStr = GetResult(headerStream.GetBuffer(), RESULT_TYPE.STATUS);
            

        }
#endif

        private string GetWebRequest(string getUrl)
        {
            string responseContent = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
                request.ContentType = "application/json";
                request.Method = "GET";
                request.Timeout = 20000;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                
                using (Stream resStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                    {
                        responseContent = reader.ReadToEnd().ToString();
                    }
                }
            }
            catch(WebException ex)
            {
                mErrorStr = ex.Message;
            }
            return responseContent;
        }

        private string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string responseContent = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); 
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = byteArray.Length;
                webReq.Timeout = 20000;

                using (Stream reqStream = webReq.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                    //reqStream.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseContent = sr.ReadToEnd().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                mErrorStr =  ex.Message;
            }
            return responseContent;
        }
        /*
        public enum RESULT_TYPE {STATUS,DATA};
        protected string GetResult(byte[] data, RESULT_TYPE type)
        {
            string retString = "";
            try
            {
                MemoryStream readStream = new MemoryStream();
                readStream.Write(data, 0, data.Length);
                readStream.Seek(0, SeekOrigin.Begin);

                XmlReader reader = XmlReader.Create(readStream);
                reader.Read();
                if (reader.Name != "xml")
                {
                    mErrorStr = "Response header is corrupted.";
                }
                else
                {
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Hysoon")
                    {
                        string request = reader.GetAttribute("Request");
                        if (request == "Response")
                        {
                            reader.Read();
                            mErrorStr = "Response data is not valid.";
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Result")
                            {
                                string status;
                                status = reader.GetAttribute("Status");
                                if (status == "Success" && type == RESULT_TYPE.DATA)
                                {
                                    retString = reader.ReadElementContentAsString();
                                }
                                else if(type == RESULT_TYPE.STATUS)
                                {
                                    retString = reader.ReadElementContentAsString();
                                }
                                else
                                {
                                    mErrorStr = reader.ReadElementContentAsString();
                                }
                            }
                        }
                    }
                    else
                    {
                        mErrorStr = "Response header is corrupted.";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return retString;
        }*/


        public enum LOGO_TYPE { CHANGE, DELETE };
        public bool LogoCommand(string pwd, String path, LOGO_TYPE type)
        {
            if (type == LOGO_TYPE.CHANGE && (path == "" || path == null))
            {
                MessageBox.Show("请加载LOGO图片", "图片错误", MessageBoxButtons.OK);
                return false;
            }

            string szParams = "";

            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/";
                if (type == LOGO_TYPE.CHANGE)
                {
                    Bitmap bmp;
                    ImageCodecInfo[] encoders;
                    ImageCodecInfo encorder = null;

                    System.Drawing.Imaging.Encoder bmpEncorder;
                    bmpEncorder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameter encoderParameter = new EncoderParameter(bmpEncorder, (long)60);

                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = encoderParameter;

                    bmp = (Bitmap)Bitmap.FromFile(path);
                    encoders = ImageCodecInfo.GetImageEncoders();
                    for (int j = 0; j < encoders.Length; ++j)
                    {
                        if (encoders[j].FormatDescription.Equals("JPEG")/*encoders[j].FormatID == bmp.RawFormat.Guid*/)
                        {
                            encorder = encoders[j];
                            break;
                        }
                    }
                    MemoryStream bmpStream = new MemoryStream();

                    //bmp.Save(bmpStream, bmp.RawFormat);
                    bmp.Save(bmpStream, encorder, encoderParameters);
                    String base64Str = Convert.ToBase64String(bmpStream.GetBuffer());
                    base64Str = base64Str.Replace("=", "%3d");
                    base64Str = base64Str.Replace("/", "%2f");
                    base64Str = base64Str.Replace("+", "%2b");
                    base64Str = base64Str.Replace("+", "%2d");
                    base64Str = base64Str.Replace("*", "%2a");

                    // string savePath = @"d:\ss.txt";
                    //string ctx = File.ReadAllText(savePath);
                    //string savePath1 = @"d:\re_sample.bmp";
                    //FileStream fs = File.Create(savePath);

                    //ctx=ctx.Replace("%2F", "/");
                    // ctx=ctx.Replace("%2B", "+");

                    //byte[] reConv = Convert.FromBase64String(/*ctx*/base64Str);
                    //FileStream fs1 = File.Create(savePath1);
                    // fs1.Write(reConv, 0, reConv.Length);
                    // fs1.Close();
                    //fs.Write(Encoding.ASCII.GetBytes(base64Str) , 0 , base64Str.Length);
                    //fs.Close();
                    szParams = "pass=" + pwd + "&imgBase64=" + base64Str ;
                    url += "changeLogo";
                }
                else
                {
                    szParams = "pass=" + pwd + "&imgBase64=-1";
                    url += "changeLogo";
                    //url += "deleteLogo";
                }

                string response = PostWebRequest(url, szParams , Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return false;
                    }

                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return false;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }

            return true;
 
        }

        public bool ChangePassword(string pwd , string newpwd)
        {
            string szUrl = "", szParams = "";
            szUrl = "http://" + AuthAddress + ":" + AuthPort + "/setPassWord";
            szParams = "oldPass=" + pwd + "&newPass=" + newpwd;
            try
            {
                string response = PostWebRequest(szUrl, szParams, Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return false;
                    }

                }
                catch (Exception )
                {
                    mErrorStr = "Response data is invalid";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }

            return true;
        }

        public enum CALLBACK_TYPE {ATTENDANCE , HEARTBEAT , IMGREG};
        public bool SendCallback(string pwd , string url , CALLBACK_TYPE type)
        {
            string szUrl = "",szParams = "";

            try
            {
               
                switch (type)
                {
                    case CALLBACK_TYPE.ATTENDANCE:
                        szUrl = "http://" + AuthAddress + ":" + AuthPort + "/setIdentifyCallBack";
                        szParams = "pass=" + pwd + "&callbackUrl=" + url;
                        break;
                    case CALLBACK_TYPE.IMGREG:
                        szUrl = "http://" + AuthAddress + ":" + AuthPort + "/setImgRegCallBack";
                        szParams = "pass=" + pwd + "&url=" + url;
                        break;
                    case CALLBACK_TYPE.HEARTBEAT:
                        szUrl = "http://" + AuthAddress + ":" + AuthPort + "/setDeviceHeartBeat";
                        szParams = "pass=" + pwd + "&url=" + url;
                        break;
                }

                string response = PostWebRequest(szUrl, szParams, Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return false;
                    }

                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return false;
                }

            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }
            return true;
        }

        public string ReadDeviceSN(string pwd)
        {
            string szSN  ="";
            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/getDeviceKey";
                string response = PostWebRequest(url, "" , Encoding.UTF8);
                if (response == null || response == "")
                    return "";
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return "";
                    }
                    szSN = /*Encoding.UTF8.GetString(*/info.data/*)*/;
                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return "";
                }


            }
            catch (Exception)
            {
                return "";
            }

            return szSN;
        }

        public enum REMOTE_COMMAND{SETIP , SETWIFI, SETTIME , RESTART , RESET , OPENDOOR};
        public string SendCommand(string pwd , REMOTE_COMMAND type , string param)
        {

            try
            {
                string szUrl = "http://" + AuthAddress + ":" + AuthPort + "/";
                string reqStr = "" , szPwd;
                szPwd = "pass=" + pwd;
                switch(type)
                {
                    case REMOTE_COMMAND.SETIP:
                        szUrl += "setNetInfo";
                        reqStr = szPwd + "&" + param;
                        break;
                    case REMOTE_COMMAND.SETWIFI:
                        szUrl += "setWifi";
                        reqStr = szPwd + "&" + param;
                        break;
                    case REMOTE_COMMAND.SETTIME:
                        szUrl += "setTime";
                        reqStr = szPwd + "&timestamp=" + param; 
                        break;
                    case REMOTE_COMMAND.RESTART:
                        szUrl += "restartDevice";
                        reqStr = szPwd; 
                        break;
                    case REMOTE_COMMAND.RESET:
                        szUrl += "device/reset";
                        reqStr = szPwd + "&delete=" + param; 
                        break;
                    case REMOTE_COMMAND.OPENDOOR:
                        szUrl += "device/openDoorControl";
                        reqStr = szPwd;
                        break;
                }
                //string response = GetWebRequest("http://" + AuthAddress + ":" + AuthPort + "/setConfig?config=" + jsonStr);
                string response = PostWebRequest(szUrl, reqStr, Encoding.UTF8);
                if (response == null || response == "")
                    return mErrorStr;
                try
                {
                    if (type == REMOTE_COMMAND.SETIP)
                    {
                        _ResultInfo<EthernetMsg> info = JsonConvert.DeserializeObject<_ResultInfo<EthernetMsg>>(response);
                        if (info.IsSucceed())
                        {
                            MessageBox.Show("IP地址已变更, 请于5秒钟后重新连接", "提示", MessageBoxButtons.OK);
                        }
                        return info.msg;
                    }
                    else
                    {
                        _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                        if (info.IsSucceed() && type == REMOTE_COMMAND.SETWIFI)
                        {
                            MessageBox.Show("WiFi地址已变更, 请于5秒钟后重新连接", "提示", MessageBoxButtons.OK);
                        }
                        return info.msg;
                    }
                    
                    
                }
                catch (Exception e)
                {
                    return e.Message;
                }

                
            }
            catch (Exception e)
            {
                return e.Message;
            }

            //return "Remote command succeeded.";

        }

        public bool ConnectionCheck(string address , int port)
        {
            return mNetworkManager.ConncectCheck(address,port);
        }

        public PersonInfo CreatePerson(string pwd, string id, string cardnum, string name)
        {
            PersonInfo szInfo = null;
            try
            {
                Person ps = new Person(id, cardnum, name);
                String jsonStr = JsonConvert.SerializeObject(ps);
                String url = "http://" + AuthAddress + ":" + AuthPort + "/person/create";
                String response = PostWebRequest(url, "pass=" + pwd + "&" + "person=" + jsonStr + "", Encoding.UTF8);
                if (response == null || response == "")
                    return null;

                try
                {
                    _ResultInfo<PersonInfo> info = JsonConvert.DeserializeObject<_ResultInfo<PersonInfo>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return null;
                    }

                    szInfo =  info.data;

                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return null;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return szInfo;
        }

        public bool RegisterCard(string pwd, string id)
        {
            try
            {
                string szUrl = "http://" + AuthAddress + ":" + AuthPort + "/face/icCardRegist";
                string response = PostWebRequest(szUrl, "pass=" + pwd + "&personId=" + id, Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return false;
                    }
                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return false;
                }


            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public PersonInfo UpdatePerson(string pwd, string id, string cardnum, string name)
        {
            PersonInfo szInfo = null;
            try
            {
                Person ps = new Person(id, cardnum, name);
                String jsonStr = JsonConvert.SerializeObject(ps);
                String url = "http://" + AuthAddress + ":" + AuthPort + "/person/update";
                String response = PostWebRequest(url, "pass=" + pwd + "&" + "person=" + jsonStr + "", Encoding.UTF8);
                if (response == null || response == "")
                    return null;

                try
                {
                    _ResultInfo<PersonInfo> info = JsonConvert.DeserializeObject<_ResultInfo<PersonInfo>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return null;
                    }

                    szInfo = info.data;

                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return null;
                }

            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return szInfo;
        }


        public FindRecordResult<FaceRecord> FindRecords(string pwd, string id, string count, string index, string stime, string etime)
        {
            FindRecordResult<FaceRecord> szInfo = null;

            try
            {
                String url;
                url = "http://" + AuthAddress + ":" + AuthPort + "/findRecords?"+
                    "pass="   + pwd   + "&" +
                    "personId="   + id    + "&" +
                    "length="      + count + "&" +
                    "index="      + index + "&" + 
                    "startTime="  + stime + "&" + 
                    "endTime="    + etime;

                String response = GetWebRequest(url);
                if (response == null || response == "")
                    return szInfo;

                try
                {
                    _ResultInfo<FindRecordResult<FaceRecord>> info = JsonConvert.DeserializeObject<_ResultInfo<FindRecordResult<FaceRecord>>>(response);
 
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return szInfo;
                    }

                    szInfo = info.data;


                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return szInfo;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return szInfo;
        }

        public FindRecordResult<CardRecord> FindCardRecords(string pwd, string id, string count, string index, string stime, string etime)
        {
            FindRecordResult<CardRecord> szInfo = null;

            try
            {

                String url;
                url = "http://" + AuthAddress + ":" + AuthPort + "/findICRecords?" +
                     "pass=" + pwd + "&" +
                    "personId=" + id + "&" +
                    "length=" + count + "&" +
                    "index=" + index + "&" +
                    "startTime=" + stime + "&" +
                    "endTime=" + etime;

                String response = GetWebRequest(url);

                if (response == null || response == "")
                    return szInfo;

                try
                {
                    _ResultInfo<FindRecordResult<CardRecord>> info = JsonConvert.DeserializeObject<_ResultInfo<FindRecordResult<CardRecord>>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return szInfo;
                    }

                    szInfo = info.data;


                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return szInfo;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return szInfo;
        }

        public PersonInfo[] FindPerson(string pwd, string id, int index, int count)
        {
            try
            {
                String szLength = count.ToString();
                String szIndex = index.ToString();

                //String url = "http://" + AuthAddress + ":" + AuthPort + "/person/find?" + "pass=" + pwd + "&id=" + id;
                String url = "http://" + AuthAddress + ":" + AuthPort + "/person/findByPage?" + "pass=" + pwd + "&personId=" + id + 
                    "&length=" + szLength + "&index=" + szIndex;
                String response = GetWebRequest(url);
                if (response == null || response == "")
                    return null;

                try
                {
                    if(id == "-1")
                    {
                        _ResultInfo<FindPersonResult> info = JsonConvert.DeserializeObject<_ResultInfo<FindPersonResult>>(response);
                        if (!info.IsSucceed())
                        {
                            mErrorStr = info.msg;
                            return null;
                        }
                        return  info.data.personInfos;

                    }else{
                        _ResultInfo<PersonInfo[]> info = JsonConvert.DeserializeObject<_ResultInfo<PersonInfo[]>>(response);
                        if (!info.IsSucceed())
                        {
                            mErrorStr = info.msg;
                            return null;
                        }
                        return info.data;
                    }
  
                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return null;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return null;
        }

        public string DeleteRecords(string pwd, string personID , string time , bool isCard)
        {
  
            try
            {
                String url;
                if(isCard)
                    url = "http://" + AuthAddress + ":" + AuthPort + "/deleteICRecords";
                else
                    url = "http://" + AuthAddress + ":" + AuthPort + "/deleteRecords";

                String response = PostWebRequest(url, 
                    "pass="    + pwd      + "&" + 
                    "time="   + time , Encoding.UTF8);
                if (response == null || response == "")
                    return "Server does not respond to delete request.";

                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                        return info.msg;
                    else
                        return info.data;

                }
                catch (Exception e)
                {
                    return  e.Message;
                }


            }
            catch (Exception ex)
            {
                return  ex.Message;
            }
            //return "Delete record succeeded.";
        }

        public string DeleteRecordsByUnixTime(string pwd, string personID, string time, bool isCard)
        {

            try
            {
                String url;
                if (isCard)
                    url = "http://" + AuthAddress + ":" + AuthPort + "/deleteICRecordsByUnixTime";
                else
                    url = "http://" + AuthAddress + ":" + AuthPort + "/deleteRecordsByUnixTime";

                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                DateTime nowTime = DateTime.Parse(time);
                long unixTime = (long)System.Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);

                // string testTime = unixTime.ToString();
                // DateTime testData = GetTime(unixTime);

                String response = PostWebRequest(url,
                    "pass=" + pwd + "&" +
                    "unixTime=" + unixTime.ToString() , Encoding.UTF8);
                if (response == null || response == "")
                    return "Server does not respond to time stamp delete request.";

                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                        return info.msg;
                    else
                        return info.data;
                    

                }
                catch (Exception e)
                {
                    return e.Message;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return "Delete record succeeded.";
        }


        public string DeletePerson(string pwd, string id)
        {
            string queryResult = "";
            try
            {
                String url = "http://" + AuthAddress + ":" + AuthPort + "/person/delete";
                String response = PostWebRequest(url, "pass=" + pwd + "&" + "id=" + id + "", Encoding.UTF8);
                if (response == null || response == "")
                    return "";

                try
                {
                    _ResultInfo<PersonDeleteEffective> info = JsonConvert.DeserializeObject<_ResultInfo<PersonDeleteEffective>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return "";
                    }

                    if(info.data != null && id != "-1")
                    {
                        queryResult = "Effective:" + info.data.effective + "Invalid:" + info.data.invalid;
                    } else
                    {
                        queryResult = info.msg;
                    }

                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return "";
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return queryResult;
        }

        public DevParameter GetParameters(string pwd)
        {
            DevParameter devParam = null;

            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/getConfig";
                string response = PostWebRequest(url, "pass=" + pwd, Encoding.UTF8);
                if (response == null || response == "")
                    return devParam;

                _ResultInfo<DevParameter> info = JsonConvert.DeserializeObject<_ResultInfo<DevParameter>>(response);
                if (!info.IsSucceed())

                {
                    mErrorStr = info.msg;
                    return devParam;
                }

                devParam = info.data;
                
            }
            catch (Exception e)
            {
                mErrorStr = e.Message;
            }

            return devParam;
        }

        public DevParameter SendParameter(string pwd, string name, int distance, int opendelay, string weigen, int level, int threshold, int interval)
        {

            try
            {
                DevParameter p = new DevParameter();
                p.companyName = name;
                p.identifyDistance = distance;
                p.delayTimeForCloseDoor = opendelay;
                p.wg = weigen;
                if (level == 0)
                    p.recRank = 1; // 不能拒绝照片
                else
                    p.recRank = 2; // 能拒绝照片

                p.identifyScores = threshold;
                p.saveIdentifyTime = interval;
                //p.enrollRank = 0;
                p.multiplayerDetection = 1;
                p.comModContent = "hello";
                p.comModType = 100;
                p.displayModContent = "{name}欢迎你";
                p.displayModType = 100;
                p.intro = ""; // 公司介绍
                p.recStrangerTimesThreshold = 3;
                p.recStrangerType = 2;
                p.slogan = "浩顺公司";
                p.ttsModContent = "欢迎{name}";
                p.ttsModStrangerContent = "陌生人啊你好";
                p.ttsModStrangerType = 100;
                p.ttsModType = 100;
                //Parameters param = new Parameters(name, distance, opendelay, weigen, level, threshold, interval);
                string jsonStr = JsonConvert.SerializeObject(p);
                string url = "http://" + AuthAddress + ":" + AuthPort + "/setConfig";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "config=" + jsonStr, Encoding.UTF8);
                if (response == null || response == "")
                    return null;
                try
                {
                    _ResultInfo<DevParameter> info = JsonConvert.DeserializeObject<_ResultInfo<DevParameter>>(response);
                    if(!info.IsSucceed())
                    {
                        mErrorStr = info.msg;

                        return null;
                    }
                    return info.data;
                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return null;
                }

                
            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return null;
            }

            //return null;
        }

        public bool CreatePermission(string pwd, string id, string time)
        {
            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/person/permissionsCreate";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "personId=" + id + "&time=" + time + "", Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return false;
                    }
                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return false;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }

            return true;
        }

        public bool PermissionDelete(string pwd, string id)
        {
            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/person/permissionsDelete";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "personId=" + id + "", Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;

                        return false;
                    }
                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return false;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }

            return true;
        }

        public bool DeletePassTime(string pwd, string id)
        {
            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/person/deletePasstime";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "personId=" + id + "", Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;

                        return false;
                    }
                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return false;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }

            return true;
        }

       
        public bool CreatePassTime(string pwd, string jsonPerson)
        {
            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/person/createPasstime";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "passtime=" + jsonPerson + "", Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;

                        return false;
                    }
                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return false;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }

            return true;
        }
        
        public string CreateFace(string pwd, string personID , string faceID , string path)
        {
            string createdFaceID = null;
            try
            {
                Bitmap bmp;
                ImageCodecInfo[] encoders;
                ImageCodecInfo encorder = null;
                
                System.Drawing.Imaging.Encoder bmpEncorder;
                bmpEncorder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameter encoderParameter = new EncoderParameter(bmpEncorder , (long)60);

                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = encoderParameter;

                bmp = (Bitmap)Bitmap.FromFile(path);
                encoders = ImageCodecInfo.GetImageEncoders();
                for (int j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].FormatDescription.Equals("JPEG")/*encoders[j].FormatID == bmp.RawFormat.Guid*/)
                    {
                        encorder = encoders[j];
                        break;
                    }
                }
                MemoryStream bmpStream = new MemoryStream();
                
                //bmp.Save(bmpStream, bmp.RawFormat);
                bmp.Save(bmpStream, encorder, encoderParameters);
                String base64Str = Convert.ToBase64String(bmpStream.GetBuffer());
                base64Str = base64Str.Replace("=", "%3d");
                base64Str = base64Str.Replace("/", "%2f");
                base64Str = base64Str.Replace("+", "%2b");
                base64Str = base64Str.Replace("+", "%2d");
                base64Str = base64Str.Replace("*", "%2a");

              
                string url = "http://" + AuthAddress + ":" + AuthPort + "/face/create";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "personId=" + personID + "&faceId=" + faceID + "&imgBase64=" + base64Str, Encoding.UTF8);
                if (response == null || response == "")
                    return null;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;

                        return null;
                    }

                    createdFaceID = info.data;
                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return null;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return null;
            }

            return createdFaceID;
        }

        public bool UpdateFace(string pwd, string personID, string faceID, string path)
        {
            try
            {
                Bitmap bmp;
                ImageCodecInfo[] encoders;
                ImageCodecInfo encorder = null;

                System.Drawing.Imaging.Encoder bmpEncorder;
                bmpEncorder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameter encoderParameter = new EncoderParameter(bmpEncorder, (long)60);

                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = encoderParameter;

                bmp = (Bitmap)Bitmap.FromFile(path);
                encoders = ImageCodecInfo.GetImageEncoders();
                for (int j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].FormatDescription.Equals("JPEG")/*encoders[j].FormatID == bmp.RawFormat.Guid*/)
                    {
                        encorder = encoders[j];
                        break;
                    }
                }
                MemoryStream bmpStream = new MemoryStream();

                //bmp.Save(bmpStream, bmp.RawFormat);
                bmp.Save(bmpStream, encorder, encoderParameters);
                String base64Str = Convert.ToBase64String(bmpStream.GetBuffer());
                base64Str = base64Str.Replace("=", "%3d");
                base64Str = base64Str.Replace("/", "%2f");
                base64Str = base64Str.Replace("+", "%2b");
                base64Str = base64Str.Replace("+", "%2d");
                base64Str = base64Str.Replace("*", "%2a");

                string url = "http://" + AuthAddress + ":" + AuthPort + "/face/update";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "personId=" + personID + "&faceId=" + faceID + "&imgBase64=" + base64Str, Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return false;
                    }

                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return false;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }

            return true;
        }

        public bool TakeImage(string pwd, string personID)
        {
            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/face/takeImg";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "personId=" + personID, Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;

                        return false;
                    }
                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return false;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }

            return true;
        }

        public bool DeleteFace(string pwd, string faceID)
        {
            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/face/delete";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "faceId=" + faceID, Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return false;
                    }
                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return false;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }

            return true;
        }

        public bool ClearFace(string pwd, string personID)
        {
            try
            {
                string url = "http://" + AuthAddress + ":" + AuthPort + "/face/deletePerson";
                string response = PostWebRequest(url, "pass=" + pwd + "&" + "personId=" + personID, Encoding.UTF8);
                if (response == null || response == "")
                    return false;
                try
                {
                    _ResultInfo<string> info = JsonConvert.DeserializeObject<_ResultInfo<string>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;

                        return false;
                    }
                }
                catch (Exception e)
                {
                    mErrorStr = e.Message;
                    return false;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
                return false;
            }

            return true;
        }

        public Boolean CompPhoto(string pwd , string img1 , string img2)
        {
            Boolean szInfo = false;

            try
            {
                String url = "http://" + AuthAddress + ":" + AuthPort + "/photoComparison";
                String response = PostWebRequest(url,
                    "pass=" + pwd + "&" + "&img1=" + img1 + "&img2=" + img2, Encoding.UTF8);
                if (response == null || response == "")
                    return szInfo;

                try
                {
                    _ResultInfo<FindFace> info = JsonConvert.DeserializeObject<_ResultInfo<FindFace>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return szInfo;
                    }

                    szInfo = true;


                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return szInfo;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return szInfo;
        }

        public FindFace FeatureReg(string pwd , string personID , string faceID , string feature , string featureKey)
        {
            FindFace szInfo = null;

            try
            {
                String url = "http://" + AuthAddress + ":" + AuthPort + "/face/featureReg";
                String response = PostWebRequest(url,
                    "pass=" + pwd + "&" +
                    "personId=" + personID + "&faceId=" + faceID +"&feature=" + feature +  "&featureKey=" + featureKey, Encoding.UTF8);
                if (response == null || response == "")
                    return szInfo;

                try
                {
                    _ResultInfo<FindFace> info = JsonConvert.DeserializeObject<_ResultInfo<FindFace>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return szInfo;
                    }

                    szInfo = info.data;


                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return szInfo;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return szInfo;
        }

        public FindFace[] FindFace(string pwd, string id)
        {
            FindFace[] szInfo = null;

            try
            {
                String url = "http://" + AuthAddress + ":" + AuthPort + "/face/find";
                String response = PostWebRequest(url,
                    "pass=" + pwd + "&" +
                    "personId=" + id  , Encoding.UTF8);
                if (response == null || response == "")
                    return szInfo;

                try
                {
                    _ResultInfo<FindFace[]> info = JsonConvert.DeserializeObject<_ResultInfo<FindFace[]>>(response);
                    if (!info.IsSucceed())
                    {
                        mErrorStr = info.msg;
                        return szInfo;
                    }

                    szInfo = info.data;


                }
                catch (Exception)
                {
                    mErrorStr = "Response data is invalid";
                    return szInfo;
                }


            }
            catch (Exception ex)
            {
                mErrorStr = ex.Message;
            }
            return szInfo;
        }

    }
}
