using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace RenLianShiBie
{
    class HttpServer
    {
        HttpListener httpListener = new HttpListener();
        TextBox logText;
        PictureBox picBox;
        public void Setup(int port = 8080 , TextBox logText = null , PictureBox picBox = null)
        {
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            httpListener.Prefixes.Add(string.Format("http://*:{0}/" , port));
            httpListener.Start();
            Receive();
            this.logText = logText;
            this.picBox = picBox;
        }


        private void Receive()
        {
            httpListener.BeginGetContext(new AsyncCallback(EndReciver), null);
        }

        void EndReciver(IAsyncResult ar)
        {
            var context = httpListener.EndGetContext(ar);
            Dispather(context);
            Receive();
        }

        RequestHelper RequestHelper;
        ResponseHelper ResponseHelper;
        void Dispather(HttpListenerContext context)
        {
            HttpListenerRequest request= context.Request;
            HttpListenerResponse response = context.Response;
            RequestHelper = new RequestHelper(request, logText , picBox);
            ResponseHelper = new ResponseHelper(response);
            RequestHelper.DispatchResources(state => { ResponseHelper.WriteToClient(state); });
            /*RequestHelper.DispatchResources(fs => {
                ResponseHelper.WriteToClient(fs);// 对相应的请求做出回应
            });*/
        }

    }

    

    public class RequestHelper
    {
        public FtpWebRequest GetRequest(string URI, string username, string password)
        {
            //根据服务器信息FtpWebRequest创建类的对象
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(URI);
            //提供身份验证信息
            result.Credentials = new System.Net.NetworkCredential(username, password);
            //设置请求完成之后是否保持到FTP服务器的控制连接，默认值为true
            result.KeepAlive = false;
            
            return result;
        }

        private HttpListenerRequest request;
         private TextBox logText;
         private PictureBox picBox;
         public RequestHelper(HttpListenerRequest request, TextBox logText , PictureBox picBox)
         {
             this.request = request;
             this.logText = logText;
             this.picBox = picBox;
         }
         public Stream RequestStream { get; set; }
         public void ExtracHeader()
         {
             RequestStream= request.InputStream;
         }
 
         public delegate void ExecutingDispatch(int state/*FileStream fs*/);
         public void DispatchResources(ExecutingDispatch action)
         {
             var rawUrl = request.RawUrl;
             try
             {
                 string expStr = "";
                 StreamReader sr = new StreamReader(request.InputStream);
                 String requestBody = sr.ReadToEnd();
                 //String[] requestData = requestBody.Split(new char[1] { '=' });
                 if (true/*requestData.Length == 2 && requestData[0] == "data"*/)
                 {
   
                    
                     if (Regex.IsMatch(rawUrl, "/attendance*"))
                     {
                        IdentityCallBack renshi;
                        expStr = HttpUtility.UrlDecode(requestBody/*requestData[1]*/);
                        renshi = JsonConvert.DeserializeObject<IdentityCallBack>(expStr);
                        expStr = "人脸识别=> IP：" + renshi.ip + ", 人员ID:" + renshi.personId + "，识别结果：" + renshi.type + ", 识别照片：" + renshi.path;
                        if(Regex.IsMatch(renshi.type , "face_?") )
                        {
                            int si = renshi.path.LastIndexOf("/") + 1;
                            int el = renshi.path.Length;
                            string fname = renshi.path.Substring(si, el - si);
                            string localfile = "ShiBieimages" + @"\" + fname;

                            if (!Directory.Exists("ShiBieimages"))
                                Directory.CreateDirectory("ShiBieimages");

                            System.Net.FtpWebRequest ftp = GetRequest(renshi.path, "", "");
                            ftp.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
                            ftp.UseBinary = true;
                            ftp.UsePassive = false;
                            using (FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
                            {
                                using (Stream responseStream = response.GetResponseStream())
                                {
                                    using (FileStream fs = new FileStream(localfile, FileMode.CreateNew))
                                    {
                                        try
                                        {
                                            byte[] buffer = new byte[2048];
                                            int read = 0;
                                            do
                                            {
                                                read = responseStream.Read(buffer, 0, buffer.Length);
                                                fs.Write(buffer, 0, read);
                                            } while (!(read == 0));
                                            responseStream.Close();
                                            fs.Flush();
                                            fs.Close();
                                        }
                                        catch (Exception)
                                        {
                                            //catch error and delete file only partially downloaded
                                            fs.Close();
                                            //delete target file as it's incomplete
                                            File.Delete(localfile);
                                        }
                                    }
                                }
                            }

                            /*byte[] bImg = Convert.FromBase64String(renshi.feature);
                            string createionPath = "ShiBieimages/" + renshi.personId + ".jpg";
                            FileStream _fs = File.Create(createionPath);
                            _fs.Write(bImg, 0, bImg.Length);
                            _fs.Close();*/

                            if (picBox.InvokeRequired)
                            {
                                picBox.Invoke(new Action<String>(s =>
                                {
                                    picBox.ImageLocation = s;
                                }), "ShiBieimages/" + fname);
                            }
                            else
                            {
                                picBox.ImageLocation = "ShiBieimages/" + fname;
                            }
                        }
    
                     }
                     else if (Regex.IsMatch(rawUrl, "/heartbeat*"))
                     {
                         DeviceHeartBeat heartbeat;
                         expStr = HttpUtility.UrlDecode(requestBody/*requestData[1]*/);
                         heartbeat = JsonConvert.DeserializeObject<DeviceHeartBeat>(expStr);
                         expStr = "设备动态=> 设备序列号：" + heartbeat.deviceKey + "，IP:" + heartbeat.ip + "，人员数量：" + heartbeat.personCount +
                             "，人脸数量：" + heartbeat.faceCount + "，设备时间：" + heartbeat.time + ", 版本：" + heartbeat.version;

                     }
                     else if (Regex.IsMatch(rawUrl, "/imgreg*"))
                     {
                         ImgRegCallBack zhaopian;
                         expStr = HttpUtility.UrlDecode(requestBody/*requestData[1]*/);
                         zhaopian = JsonConvert.DeserializeObject<ImgRegCallBack>(expStr);
                         expStr = "人脸登录=> 设备序列号：" + zhaopian.deviceKey + "，IP:" + zhaopian.ip + "，人员ID：" + zhaopian.personId +
                             "，照片ID：" + zhaopian.faceId + " 照片文件名：" + zhaopian.newImgPath;

                         if (!Directory.Exists("images"))
                             Directory.CreateDirectory("images");

                        int si = zhaopian.newImgPath.LastIndexOf("/") + 1;
                        int el = zhaopian.newImgPath.Length;
                        string fname = zhaopian.newImgPath.Substring(si, el - si);
                        string localfile = "images" + @"\" + fname;

                        if (!Directory.Exists("images"))
                            Directory.CreateDirectory("images");

                        System.Net.FtpWebRequest ftp = GetRequest(zhaopian.newImgPath, "", "");
                        ftp.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
                        ftp.UseBinary = true;
                        ftp.UsePassive = false;
                        using (FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
                        {
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                using (FileStream fs = new FileStream(localfile, FileMode.CreateNew))
                                {
                                    try
                                    {
                                        byte[] buffer = new byte[2048];
                                        int read = 0;
                                        do
                                        {
                                            read = responseStream.Read(buffer, 0, buffer.Length);
                                            fs.Write(buffer, 0, read);
                                        } while (!(read == 0));
                                        responseStream.Close();
                                        fs.Flush();
                                        fs.Close();
                                    }
                                    catch (Exception)
                                    {
                                        //catch error and delete file only partially downloaded
                                        fs.Close();
                                        //delete target file as it's incomplete
                                        File.Delete(localfile);
                                    }
                                }
                            }
                        }

                        
                      /*  byte[] bImg = Convert.FromBase64String(zhaopian.feature);
                         string createionPath = "images/" + zhaopian.faceId + ".jpg";
                         FileStream _fs = File.Create(createionPath);
                         _fs.Write(bImg, 0, bImg.Length);
                         _fs.Close();*/

                     }

                     // parse is sucessed
                     if (expStr != "")
                     {
                         try
                         {
                             if (logText.InvokeRequired)
                             {
                                 logText.Invoke(new Action<String>(s =>
                                 {
                                     logText.AppendText(s + Environment.NewLine);
                                     logText.ScrollToCaret();
                                 }), expStr);
                             }
                             else
                             {
                                 logText.AppendText(expStr + Environment.NewLine);
                                 logText.ScrollToCaret();
                             }

                             action.Invoke(200);
                         }
                         catch (Exception ex)
                         {
                             Console.WriteLine(ex.Message);
                         }
                     }
                 }
                 //else
                 //{
                 //    action.Invoke(404);
                 //}
             }
             catch (Exception e)
             {
                action.Invoke(405);
                Console.WriteLine(e.Message);
                return;
             }

            action.Invoke(404);

            /*string filePath = string.Format(@"{0}/wwwroot{1}", Environment.CurrentDirectory, rawUrl);//这里对应请求其他类型资源，如图片，文本等
            if (rawUrl.Length==1)
            filePath = string.Format(@"{0}/wwwroot/index.html", Environment.CurrentDirectory);//默认访问文件
            try {
                if (File.Exists(filePath))
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                    action.Invoke(fs);                     
                 
                }
            }
            catch { return; }*/
        }
         public void ResponseQuerys()
         {
             var querys = request.QueryString;
             foreach (string key in querys.AllKeys)
             {
                 VarityQuerys(key,querys[key]);
             }
         }
 
         private void VarityQuerys(string key,string value)
         {
             
         }
 
         private void Pictures(string id)
         {
 
         }
 
         private void Texts(string id)
         {
 
         }
 
         private void Defaults(string id)
         {
 
         }
 
     }

    public class ResponseHelper
    {
        private HttpListenerResponse response;
        public ResponseHelper(HttpListenerResponse response)
        {
            this.response = response;
            OutputStream = response.OutputStream;
           
        }
        public Stream OutputStream { get; set; }
        public class FileObject
        {
            public FileStream fs;
            public byte[] buffer;
        }

        public void WriteToClient(int state/*FileStream fs*/)
        {
            response.StatusCode = state;
            string outstring;
            if (state == 200)
            {
                outstring = "{\"result\":1,\"success\":true}";
            }
            else if (state == 400)
            {
                outstring = "{\"result\":0,\"msg\":\"access denied\"}";
            }
            else
            {
                outstring = "{\"result\":0,\"msg\":\"url missing\"}";
            }

            try
            {
                OutputStream.Write(System.Text.Encoding.ASCII.GetBytes(outstring), 0, outstring.Length);
                OutputStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            /*byte[] buffer = new byte[1024];
            FileObject obj = new FileObject() { fs = fs, buffer = buffer };      
            fs.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(EndWrite), obj);*/
        }
       
        void EndWrite(IAsyncResult ar)
        {
            
           /* var obj = ar.AsyncState as FileObject;
            var num= obj.fs.EndRead(ar);
            OutputStream.Write(obj.buffer,0,num);
            if (num < 1) { 
　　　　　　　　　　obj.fs.Close(); //关闭文件流
　　　　　　　　　　OutputStream.Close();//关闭输出流，如果不关闭，浏览器将一直在等待状态
 　　　　　　　　　　return; 
　　　　　　　　}
            obj.fs.BeginRead(obj.buffer, 0, obj.buffer.Length, new AsyncCallback(EndWrite), obj);*/
        }
    }

}
