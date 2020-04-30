using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Runtime.Serialization;
//using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;


namespace RenLianShiBie
{
    public partial class main : Form
    {
        RemoteManager mRemote;
        HttpServer httpServer;
        String DevIp = "192.168.8.200"; //发生例外时可使用地址   
        String DevPassword = "";
        Boolean IsSearch = false;
        Boolean SearchThreadIsRunning = false;
        int BroadCastPort = 8091; //根据协议规定
        int DevPort = 8090; //可以任何设定，但不是协议文件上的值（8080）。
        int EditID = -1;    //添加新项目为-1 , 其他编辑模式.

        private BackgroundWorker mWorker = new BackgroundWorker();
        public main()
        {
            InitializeComponent();
            mRemote = new RemoteManager();
#if DEBUG
            mRemote.SelfTest();

#endif
            System.Timers.Timer t = new System.Timers.Timer(1000);
            t.Elapsed += new System.Timers.ElapsedEventHandler(theout); //到达时间的时候执行事件；   
            t.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
            t.Enabled = true;
            t.Start();

            InitializeBackgroundWorker();
            InitInterface();

            try
            {
                SqliteHelper.NewDbFile();
                //SqliteHelper.ExecuteCommand("drop table MachineList");
                // Check Table.
                if (SqliteHelper.IsTableExists("MachineList") == 0)
                {
                    if (SqliteHelper.CreateTable("CREATE TABLE 'MachineList'('id' INTEGER PRIMARY KEY ASC AUTOINCREMENT, 'Name' varchar , 'IPAddr' varchar , 'Port' int ,'Pwd' varchar)") != 0)
                    {
                        MessageBox.Show("Create MachineList failed.");
                    }
                }

                httpServer = new HttpServer();
                httpServer.Setup(8090, HuiTiaoLog, ZuiHouZhaoPian);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
#if DEBUG
            //--- 如下指令，在于调试模式。

            //PasswordText.Text = SqliteHelper.ExecuteCommand("select IPAddr from AppConfig");
            //string mm = "MTUzOTE0OTU2NDMzMg==";
            // byte[] aa = Convert.FromBase64String(mm);
            // String xx = Encoding.Default.GetString(aa);
            //string jsonStr = @"

            //    [{'Languages':['C#%','Java'],'Name':'李志伟','Sex':true},

            //    {'Languages':['C#','C++'],'Name':'Coder2','Sex':false},

            //    {'Languages':['C#','C++','C','Java'],'Name':'Coder3','Sex':true}]";

            //IdentityCallBack renshi = new IdentityCallBack();
            //renshi.DeviceKey = "60BA9BF9F4";
            //renshi.IP = "fe80::12d0:7aff:fe00:759%eth0";
            //renshi.PersonID = "朴";
            //renshi.PersonName = "www";
            //renshi.Time = "2018-10-29 21:13:36";
            //renshi.Type = "1";
            //String aaa = JsonConvert.SerializeObject(renshi);
            //IdentityCallBack ren1;
            //ren1 = /*@"{\"deviceKey\":\"60BA9BF9F4\",\"ip\":\"fe80::12d0:7aff:fe00:759%eth0\",\"personId\":\"朴\",\"time\":\"2018-10-29 21:13:36\",\"type\":\"0\"}";//*/ JsonConvert.DeserializeObject<IdentityCallBack>(aaa);
            //JArray ja = (JArray)JsonConvert.DeserializeObject(jsonStr);
            //foreach (JToken jt in ja)
            {
                //   JObject jo = (JObject)jt;

                //   JArray temp = (JArray)jo["Languages"];

                //   foreach (JToken token in temp)
                {

                    //      Console.Write(token + " ");

                }

                //  Console.WriteLine("\t" + jo["Name"] + "\t" + jo["Sex"]);
            }

            // json test
            //PersonInfo[] infos = new PersonInfo[2];

            //infos[0] = new PersonInfo("123123", "aaa", "aaa", "xxx", "123123", "2012:00:00,2012:00:01", "2018-00-00 00:00:00");
            //infos[1] = new PersonInfo("123123", "aaa", "ccc", "sss", "123123", "2012:00:00,2012:00:01", "2018-00-00 00:00:00");
            //FindData fd = new FindData(0 , 2 , infos);
            //string szFind = JsonConvert.SerializeObject(fd);

            // FindData fd2 = JsonConvert.DeserializeObject<FindData>(szFind);
            //string szOutput;
            //foreach (PersonInfo info in fd2.Infos)
            // {
            //    szOutput = JsonConvert.SerializeObject(info);
            // Logd(szOutput);
            //}
            //JsonString
            //string testSz = "eyJjb3VudCI6MSwiaW5kZXgiOjAsImluZm9zIjpbeyJJRENhcmQiOiIiLCJjcmVhdGVUaW1lIjoiMTU0MDI4NDIzNzYwMSIsIm5hbWUiOiJ3d3ciLCJwZXJzb25JRCI6Ind3dyJ9XX0=";
            //JpgString
            string testSz = "L04AAPAxXLkMAgAA8t7VPY6kG7tQYaO8YrzqvTjLAT5PErq8nGbUvSBvrj28oxY7O3awvKm+nb3kKQc94XTEvYWtWT7noVs9lSFlvaKBDT2acE09rZYzvJ+fgDzV+j+9amq1vOQxCz60PxW81YuZPZymMj03pfs9apdOvr/tJz5JPLk9dg7SvZFfSz5Yray9gleUPYmlEj78Mga+n1vbverh9L1sjRS9+bj2PVojsjwD/Aa+99Qbuyl10j0rWTK97tBVPmodMb3HBm09nj8cvONkIT7AqyU9tm34PGhzw70/XPS7JRz/veaZhr1acfa9YJC0POC9QbpW6/u8S5e9Pcb+TT2T1Y+8UC19PbNyGz7Fy1E8M6AJPRIrg703y0s8vDCOvdpxFr1ziNE9R1jyPHYsTz2onCg9M/ZxOTTEBTtUJz2+PQEkvpMz8TsVSzo+OgF0vBcJfb0OL4K9mGBPvXyAqL0i8Zg965UzPpgdPL5dLC09eD4UvqOWUD3VCUA9tMCxvbyLujq3gN28p9v6vDHSHL1rbaE9GyKVvSzbNr6tlxG+zPO2Pc0WCr1hwRo9JMMKvrCfpT0WtZe7jW8ePmsJnTwK1mI9cHOKPZMf+TzgjZY8IlSTvJxqojz64rE9zxW+PdKuQL2yWUM8VQFTPRKghD0j0WG9ZNa+vBberjwNmUk9PfuSPU1BOL0=";

            byte[] bImg = Convert.FromBase64String(testSz);
            String sss = System.Text.Encoding.UTF8.GetString(bImg);
            string testPath = "d:/test.jpg";
            FileStream testfs = File.Create(testPath);
            testfs.Write(bImg, 0, bImg.Length);
            testfs.Close();

            // RegEx Test
            String mStr = "This is test string.";
            Regex exp = new Regex("[^0-9A-Za-z]"); // Caution: android
            if (mStr != "-1" && exp.Match(mStr).Success == true)
            {
                Console.WriteLine(mStr);
            }
#endif
        }

        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {

        }

        private void InitInterface()
        {
            RenYuanThirdStartTime.Mask = "90:00:00";
            RenYuanThirdStartTime.ValidatingType = typeof(String);
            RenYuanThirdStartTime.TypeValidationCompleted += new TypeValidationEventHandler(RenYuanThirdStartTime_TypeValidationCompleted);
            RenYuanThirdStartTime.Text = "18:30:00";

            RenYuanThirdEndTime.Mask = "90:00:00";
            RenYuanThirdEndTime.ValidatingType = typeof(String);
            RenYuanThirdEndTime.TypeValidationCompleted += new TypeValidationEventHandler(RenYuanThirdEndTime_TypeValidationCompleted);
            RenYuanThirdEndTime.Text = "21:00:00";

            RenYuanSecondEndTime.Mask = "90:00:00";
            RenYuanSecondEndTime.ValidatingType = typeof(String);
            RenYuanSecondEndTime.TypeValidationCompleted += new TypeValidationEventHandler(RenYuanSecondEndTime_TypeValidationCompleted);
            RenYuanSecondEndTime.Text = "14:30:00";

            RenYuanSecondStartTime.Mask = "90:00:00";
            RenYuanSecondStartTime.ValidatingType = typeof(String);
            RenYuanSecondStartTime.TypeValidationCompleted += new TypeValidationEventHandler(RenYuanSecondStartTime_TypeValidationCompleted);
            RenYuanSecondStartTime.Text = "12:00:00";

            RenYuanFirstStartTime.Mask = "90:00:00";
            RenYuanFirstStartTime.ValidatingType = typeof(String);
            RenYuanFirstStartTime.TypeValidationCompleted += new TypeValidationEventHandler(RenYuanFirstStartTime_TypeValidationCompleted);
            RenYuanFirstStartTime.Text = "08:00:00";

            RenYuanFirstEndTime.Mask = "90:00:00";
            RenYuanFirstEndTime.ValidatingType = typeof(String);
            RenYuanFirstEndTime.TypeValidationCompleted += new TypeValidationEventHandler(RenYuanFirstEndTime_TypeValidationCompleted);
            RenYuanFirstEndTime.Text = "08:30:00";
            // 
            SheBeiName.Text = "人脸识别";
            //

            ShiBieDistance.Items.Add("0: 无限制");
            ShiBieDistance.Items.Add("1: 0.5米以内");
            ShiBieDistance.Items.Add("2: 1米以内");
            ShiBieDistance.Items.Add("3: 1.5米以内");
            ShiBieDistance.Items.Add("4: 2米以内");
            ShiBieDistance.Items.Add("5: 3米以内");
            ShiBieDistance.SelectedIndex = 3;
            SpoofSwitch.Items.Add("关");
            SpoofSwitch.Items.Add("开");
            SpoofSwitch.SelectedIndex = 1;
            WeiGenShuChu.Items.Add("输出人员ID(WG26)");
            WeiGenShuChu.Items.Add("输出卡号(WG26)");
            WeiGenShuChu.Items.Add("输出人员ID(WG34)");
            WeiGenShuChu.Items.Add("输出卡号(WG34)");
            WeiGenShuChu.SelectedIndex = 0;

            DHCPModeList.Items.Add("DHCP 模式");
            DHCPModeList.Items.Add("手动配置模式");
            DHCPModeList.SelectedIndex = 0;

            KaiMenDelayCB.Items.Add("1000ms");
            KaiMenDelayCB.Items.Add("3000ms");
            KaiMenDelayCB.Items.Add("5000ms");
            KaiMenDelayCB.Items.Add("10000ms");
            KaiMenDelayCB.SelectedIndex = 0;

            ShiBieJianGeCB.Items.Add("3000ms");
            ShiBieJianGeCB.Items.Add("5000ms");
            ShiBieJianGeCB.Items.Add("7000ms");
            ShiBieJianGeCB.Items.Add("10000ms");
            ShiBieJianGeCB.SelectedIndex = 0;

            RefreshMachineList();
            SheBeiViewPanel.Visible = true;
            SheBeiTianJiaPanel.Visible = false;
            RemoteFrame.Visible = false;
            HuiTiaoPanel.Visible = false;

            WifiOption.Checked = false;
            EthernetOption.Checked = true;
            WiFiName.Enabled = false;
            WiFiPwd.Enabled = false;
        }

        String ThirdStartTimeString;
        void RenYuanThirdStartTime_TypeValidationCompleted(object obj, TypeValidationEventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)obj;
            if (!e.IsValidInput)
            {
                String v = mtb.MaskedTextProvider.ToString();
                if (v != "")
                {
                    MessageBox.Show("无效的值. 请输入准确的值", "无效的值", MessageBoxButtons.OK);
                    //e.Cancel = true;
                    mtb.Text = ThirdStartTimeString;
                }
            }
            else
            {
                ThirdStartTimeString = mtb.MaskedTextProvider.ToString();
            }
        }

        String ThirdEndTimeString;
        void RenYuanThirdEndTime_TypeValidationCompleted(object obj, TypeValidationEventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)obj;
            if (!e.IsValidInput)
            {
                String v = mtb.MaskedTextProvider.ToString();
                if (v != "")
                {
                    MessageBox.Show("无效的值. 请输入准确的值", "无效的值", MessageBoxButtons.OK);
                    //e.Cancel = true;
                    mtb.Text = ThirdEndTimeString;
                }

            }
            else
            {
                ThirdEndTimeString = mtb.MaskedTextProvider.ToString();
            }
        }

        String SecondEndTimeString;
        void RenYuanSecondEndTime_TypeValidationCompleted(object obj, TypeValidationEventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)obj;
            if (!e.IsValidInput)
            {
                String v = mtb.MaskedTextProvider.ToString();
                if (v != "")
                {
                    MessageBox.Show("无效的值. 请输入准确的值", "无效的值", MessageBoxButtons.OK);
                    //e.Cancel = true;
                    mtb.Text = SecondEndTimeString;
                }

            }
            else
            {
                SecondEndTimeString = mtb.MaskedTextProvider.ToString();
            }
        }

        String SecondStartTimeString;
        void RenYuanSecondStartTime_TypeValidationCompleted(object obj, TypeValidationEventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)obj;
            if (!e.IsValidInput)
            {
                String v = mtb.MaskedTextProvider.ToString();
                if (v != "")
                {
                    MessageBox.Show("无效的值. 请输入准确的值", "无效的值", MessageBoxButtons.OK);
                    //e.Cancel = true;
                    mtb.Text = SecondStartTimeString;
                }

            }
            else
            {
                SecondStartTimeString = mtb.MaskedTextProvider.ToString();
            }
        }


        String FirstStartTimeString;
        void RenYuanFirstStartTime_TypeValidationCompleted(object obj, TypeValidationEventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)obj;
            if (!e.IsValidInput)
            {
                String v = mtb.MaskedTextProvider.ToString();
                if (v != "")
                {
                    MessageBox.Show("无效的值. 请输入准确的值", "无效的值", MessageBoxButtons.OK);
                    //e.Cancel = true;
                    mtb.Text = FirstStartTimeString;
                }

            }
            else
            {
                FirstStartTimeString = mtb.MaskedTextProvider.ToString();
            }
        }

        String FirstEndTimeString;
        void RenYuanFirstEndTime_TypeValidationCompleted(object obj, TypeValidationEventArgs e)
        {
            MaskedTextBox mtb = (MaskedTextBox)obj;
            if (!e.IsValidInput)
            {
                String v = mtb.MaskedTextProvider.ToString();
                if (v != "")
                {
                    MessageBox.Show("无效的值. 请输入准确的值", "无效的值", MessageBoxButtons.OK);
                    //e.Cancel = true;
                    mtb.Text = FirstEndTimeString;
                }

            }
            else
            {
                FirstEndTimeString = mtb.MaskedTextProvider.ToString();
            }
        }

        private void InitializeBackgroundWorker()
        {
            mWorker.DoWork +=
                new DoWorkEventHandler(mWorker_DoWork);
            mWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            mWorker_RunWorkerCompleted);
            mWorker.ProgressChanged +=
                new ProgressChangedEventHandler(
            mWorker_ProgressChanged);
        }

        private void mWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (mRemote.Connect(DevIp, DevPort, DevPassword))
            {
                e.Result = 0;
            }
            else
            {
                e.Result = 1;
            }

        }

        private void mWorker_RunWorkerCompleted(
            object sender,
            RunWorkerCompletedEventArgs e)
        {
            SheBeiView.Enabled = true;
            if ((int)e.Result == 0)
            {
                DevParameter devParam;
                EnableControls();
                devParam = mRemote.GetParameters(DevPassword);
                if (devParam == null)
                {
                    Logd("No support read-paramter function.");
                }

                try
                {
                    if (devParam != null)
                    {
                        SheBeiName.Text = devParam.companyName;
                        if (devParam.identifyDistance >= 5)
                            devParam.identifyDistance = 5;
                        ShiBieDistance.SelectedIndex = devParam.identifyDistance;
                        //KaiMenDelayCB.SelectedIndex = devParam.delayTimeForCloseDoor;
                        KaiMenDelayTime.Text = devParam.delayTimeForCloseDoor.ToString();

                        if (devParam.wg == "#34WG{idcardNum}#")
                            WeiGenShuChu.SelectedIndex = 3;
                        else if (devParam.wg == "#34WG{id}#")
                            WeiGenShuChu.SelectedIndex = 2;
                        else if (devParam.wg == "#WG{idcardNum}#")
                            WeiGenShuChu.SelectedIndex = 1;
                        else
                            WeiGenShuChu.SelectedIndex = 0; //#WG{id}#

                        if (devParam.recRank == 1)
                            SpoofSwitch.SelectedIndex = 0; // 不能拒绝照片
                        else
                            SpoofSwitch.SelectedIndex = 1; // 拒绝照片

                        ShiBieJianGe.Text = devParam.saveIdentifyTime.ToString();
                        //ShiBieJianGeCB.SelectedIndex = devParam.saveIdentifyTime;
                        ShiBieThreshold.Value = (devParam.identifyScores / 10);
                        lbFenshu.Text = ((devParam.identifyScores / 10) * 10).ToString();
                    }
                    string hostAddr = "";
                    IPAddress[] ips = Dns.GetHostAddresses(hostAddr);
                    foreach (IPAddress ip in ips)   //循环遍历得到的IP地址
                    {
                        if(ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            byte[] b = ip.GetAddressBytes();
                            hostAddr = b[0].ToString() + "." + b[1].ToString() + "." + b[2].ToString() + "." + b[3].ToString();
                            break;
                        }
                    }

                    if (hostAddr == "")
                        hostAddr = "127.0.0.1";

                    txtRecTuneURL.Text =  "http://" + hostAddr + ":8090/attendance";
                    txtSetHeartBeatURL.Text = "http://" + hostAddr + ":8090/heartbeat";  
                    txtPictureRegisterURL.Text = "http://" + hostAddr + ":8090/imgreg"; 

                }
                catch (Exception ex)
                {
                    Logd(ex.Message);
                }

                TextDeviceId.Text = "";
                EmployeeIDText.Text = "";
                EmployeeNameText.Text = "";
                EmployeeCertificateText.Text = "";
                SouSuoView.Items.Clear();
                FaceIDText.Text = "";
                RenLianTuPian.ImageLocation = "";
                // Remote Control Frame Enable.
                Animation.Init();
                Animation.ShowControl(MainManagerPanel, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(RemoteFrame, true, AnchorStyles.Right);
                MainButtonsLock();
                EnableControls();
                Logd("Server has been connected.");
            }
            else
            {
                MainButtonsUnlock();
                Logd("Network interface failed to prepare." + "(" + mRemote.GetErrorString() + ")");

            }
        }

        private void Logd(String s)
        {
            HistoryText.Text += s + "\r\n";
        }

        private void ClsLogd()
        {
            HistoryText.Text = "";
        }

        private void mWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void main_Leave(object sender, EventArgs e)
        {

        }

        private void EnableControls()
        {
            try
            {
                RemoteControlPanel.Enabled = true;
                ParameterPanel.Enabled = true;
                ReadSerialPanel.Enabled = true;
                PeopleManagePanel.Enabled = true;
                HistoryPanel.Enabled = true;
                LogoPanel.Enabled = true;
                MachinePasswordPanel.Enabled = true;
                CallBackPanel.Enabled = true;
            }
            catch (Exception)
            {

            }
        }

        private void DisableControls()
        {
            try
            {
                RemoteControlPanel.Enabled = false;
                ParameterPanel.Enabled = false;
                ReadSerialPanel.Enabled = false;
                PeopleManagePanel.Enabled = false;
                HistoryPanel.Enabled = false;
                LogoPanel.Enabled = false;
                MachinePasswordPanel.Enabled = false;
                CallBackPanel.Enabled = false;
            }
            catch (Exception)
            {
            }
        }


        private void LianJieButton_Click(object sender, EventArgs e)
        {
            Animation.Init();
            Animation.ShowControl(RemoteFrame, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(MainManagerPanel, true, AnchorStyles.Right);
            MainButtonsUnlock();

        }

        private void main_Load(object sender, EventArgs e)
        {
            DisableControls();
            Animation.ShowControl(MainManagerPanel, true, AnchorStyles.Right);
        }

        private void groupBoxEmployeeManage_Enter(object sender, EventArgs e)
        {

        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClsLogd();
        }

        private void MainTooltip_Popup(object sender, PopupEventArgs e)
        {

        }

        private void ControlPanel_MouseEnter(object sender, EventArgs e)
        {
            MainTooltip.AutoPopDelay = 1000;
            MainTooltip.InitialDelay = 500;
            MainTooltip.ReshowDelay = 500;
            MainTooltip.ShowAlways = false;
            //MainTooltip.Show(@"你必须连接网络", btnConnect);
        }

        private void ControlPanel_MouseLeave(object sender, EventArgs e)
        {
            //MainTooltip.Hide(btnConnect);
        }

        private void MainThread_Tick(object sender, EventArgs e)
        {

        }

        private void btnSelectLogo_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                picBoxLogo.ImageLocation = dlg.FileName;
            }
        }

        private void btnRemoteOpenDoor_Click(object sender, EventArgs e)
        {

            Logd(mRemote.SendCommand(DevPassword, RemoteManager.REMOTE_COMMAND.OPENDOOR, ""));

        }

        private void RebootButton_Click(object sender, EventArgs e)
        {
            Logd(mRemote.SendCommand(DevPassword, RemoteManager.REMOTE_COMMAND.RESTART, ""));
        }

        private DateTime GetTime(double d)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddMilliseconds(d);
            return time;
        }

        private void SetTimeButton_Click(object sender, EventArgs e)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime nowTime = DateTime.Now;
            long unixTime = (long)System.Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);

            // string testTime = unixTime.ToString();
            // DateTime testData = GetTime(unixTime);

            Logd(mRemote.SendCommand(DevPassword, RemoteManager.REMOTE_COMMAND.SETTIME, unixTime.ToString()));

        }

        private void SetDefaultButton_Click(object sender, EventArgs e)
        {
            Logd(mRemote.SendCommand(DevPassword, RemoteManager.REMOTE_COMMAND.RESET, "true"));
        }

        private void btnReadSequenceNumber_Click(object sender, EventArgs e)
        {
            string szSerial = mRemote.ReadDeviceSN(DevPassword);
            if (szSerial == "")
            {
                Logd("Failed to read device serial number. " + mRemote.GetErrorString());
            }
            else
            {
                Logd("Device serial number was read successfully.");
                TextDeviceId.Text = szSerial;
            }
        }

        private void btnSetRecTuneURL_Click(object sender, EventArgs e)
        {
            if (!mRemote.SendCallback(DevPassword, txtRecTuneURL.Text, RemoteManager.CALLBACK_TYPE.ATTENDANCE))
                Logd(mRemote.GetErrorString());
            else
                Logd("The attendance url setting was sucessful.");
        }

        private void btnSetHeartBeatTuneURL_Click(object sender, EventArgs e)
        {
            if (!mRemote.SendCallback(DevPassword, txtSetHeartBeatURL.Text, RemoteManager.CALLBACK_TYPE.HEARTBEAT))
                Logd(mRemote.GetErrorString());
            else
                Logd("The Heartbeat url setting was sucessful.");
        }

        private void btnSetRegisterTuneURL_Click(object sender, EventArgs e)
        {
            if (!mRemote.SendCallback(DevPassword, txtPictureRegisterURL.Text, RemoteManager.CALLBACK_TYPE.IMGREG))
                Logd(mRemote.GetErrorString());
            else
                Logd("The Image register url setting was sucessful.");
        }

        private void btnChangeLogo_Click(object sender, EventArgs e)
        {
            if (!mRemote.LogoCommand(DevPassword, picBoxLogo.ImageLocation, RemoteManager.LOGO_TYPE.CHANGE))
                Logd(mRemote.GetErrorString());
            else
                Logd("Logo has been changed.");
        }

        private void btnDeleteLogo_Click(object sender, EventArgs e)
        {
            if (!mRemote.LogoCommand(DevPassword, "", RemoteManager.LOGO_TYPE.DELETE))
                Logd(mRemote.GetErrorString());
            else
                Logd("Logo has been deleted.");
        }

        private void btnSetDevParam_Click(object sender, EventArgs e)
        {
            string WeiGenStr;
            try
            {
                switch (WeiGenShuChu.SelectedIndex)
                {
                    case 0:
                        WeiGenStr = "#WG{id}#";
                        break;
                    case 1:
                        WeiGenStr = "#WG{idcardNum}#";
                        break;
                    case 2:
                        WeiGenStr = "#34WG{id}#";
                        break;
                    case 3:
                        WeiGenStr = "#34WG{idcardNum}#";
                        break;
                    default:
                        WeiGenStr = "#WG{id}#";
                        break;
                }

                DevParameter info = mRemote.SendParameter(DevPassword, SheBeiName.Text, ShiBieDistance.SelectedIndex, int.Parse(KaiMenDelayTime.Text), WeiGenStr
                    , SpoofSwitch.SelectedIndex, int.Parse(ShiBieThreshold.Value + "0"), int.Parse(ShiBieJianGe.Text));
                if (info == null)
                {
                    Logd(mRemote.GetErrorString());
                }
                else
                {
                    SheBeiName.Text = info.companyName;
                    if (info.identifyDistance >= 5)
                        info.identifyDistance = 5;
                    ShiBieDistance.SelectedIndex = info.identifyDistance;
                    KaiMenDelayTime.Text = info.delayTimeForCloseDoor.ToString();
                    if (info.recRank == 1)
                        SpoofSwitch.SelectedIndex = 0;
                    else
                        SpoofSwitch.SelectedIndex = 1;

                    ShiBieThreshold.Value = (info.identifyScores / 10);
                    ShiBieJianGe.Text = info.saveIdentifyTime.ToString();
                    lbFenshu.Text = ((info.identifyScores / 10) * 10).ToString();

                    if (info.wg == "#34WG{idcardNum}#")
                        WeiGenShuChu.SelectedIndex = 3;
                    else if (info.wg == "#34WG{id}#")
                        WeiGenShuChu.SelectedIndex = 2;
                    else if (info.wg == "#WG{idcardNum}#")
                        WeiGenShuChu.SelectedIndex = 1;
                    else
                        WeiGenShuChu.SelectedIndex = 0; //#WG{id}#

                    Logd("The parameter setting was sucessful.");

                }
            }
            catch (Exception)
            {
                MessageBox.Show("您输入的信息是错误的", "输入错误", MessageBoxButtons.OK);
            }
        }

        private void SetDeviceIPButton_Click(object sender, EventArgs e)
        {
            string szParameters;
            RemoteManager.REMOTE_COMMAND cmdType = RemoteManager.REMOTE_COMMAND.SETIP;
            if (EthernetOption.Checked)
            {
                szParameters = "isDHCPMod=" + (DHCPModeList.SelectedIndex + 1) +
                    "&ip=" + txtDeviceIP.Text +
                    "&gateway=" + GatewayText.Text +
                    "&subnetMask=" + MaskText.Text +
                    "&DNS=" + DNS1Text.Text;
                cmdType = RemoteManager.REMOTE_COMMAND.SETIP;
            }
            else
            {
                wifiMsg msg = new wifiMsg();
                if (WiFiName.Text == "")
                {
                    MessageBox.Show("请输入WiFi名称", "输入错误", MessageBoxButtons.OK);
                    return;
                }
                msg.ssId = WiFiName.Text;
                msg.pwd = WiFiPwd.Text;
                msg.dns = DNS1Text.Text;
                msg.ip = txtDeviceIP.Text;
                msg.gateway = GatewayText.Text;
                msg.isDHCPMod = DHCPModeList.SelectedIndex == 0 ? true : false;
                szParameters = "wifiMsg=" + JsonConvert.SerializeObject(msg);
                cmdType = RemoteManager.REMOTE_COMMAND.SETWIFI;
            }

            string resStr = mRemote.SendCommand(DevPassword, cmdType, szParameters);
            Logd(resStr);
        }

        private void btnSetPassword_Click(object sender, EventArgs e)
        {
            if (NewPwdText.Text == "" /*PasswordText.Text == "" != txtOldPassword.Text*/)
            {
                MessageBox.Show("请输入准确的密码.", "输入错误", MessageBoxButtons.OK);
                return;
            }

            string curPwd;
            if (DevPassword == "")
                curPwd = NewPwdText.Text; // 这是为了初次设置密码。
            else
                curPwd = DevPassword; // 一般情况下的输入的密码。

            if (!mRemote.ChangePassword(curPwd, NewPwdText.Text))
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("The password has been changed.");
            }
        }

        private void CreateEmployeeButton_Click(object sender, EventArgs e)
        {
            if (EmployeeNameText.Text == "")
            {
                MessageBox.Show(@"请输入名称", @"提示");
                EmployeeNameText.Focus();
                return;
            }

            if (EmployeeIDText.Text == "-1")
            {
                EmployeeIDText.Text = "";
                //MessageBox.Show(@"输入错误", @"提示");
                //EmployeeIDText.Focus();
                //return;
            }

            Regex exp = new Regex("[^0-9A-Za-z]"); // Caution: android
            if (EmployeeIDText.Text != "-1" && exp.Match(EmployeeIDText.Text).Success == true)
            {
                MessageBox.Show(@"输入错误", @"提示");
                return;
            }

            PersonInfo createdPerson = mRemote.CreatePerson(DevPassword, EmployeeIDText.Text, EmployeeCertificateText.Text, EmployeeNameText.Text);
            if (createdPerson == null)
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                try
                {
                    EmployeeIDText.Text = createdPerson.id;
                    EmployeeCertificateText.Text = createdPerson.idcardNum;
                    EmployeeNameText.Text = createdPerson.name;
                    Logd("The person information has been created.");
                    FaceIDText.Text = "";
                }
                catch (Exception ex)
                {
                    Logd("Creation person information failed. " + ex.Message);
                }

            }
        }

        private void KaHaoDengLu_Click(object sender, EventArgs e)
        {
            if (EmployeeIDText.Text == "" || EmployeeIDText.Text == "-1")
            {
                MessageBox.Show(@"请输入正确的人员ID", @"提示");
                return;
            }

            try
            {
                if (mRemote.RegisterCard(DevPassword, EmployeeIDText.Text))
                {
                    Logd("IC Card regist succeed.");
                }
                else
                {
                    Logd(mRemote.GetErrorString());
                }
            }
            catch (Exception ex)
            {
                Logd("IC Card register failed. (" + ex.Message + ")");
            }
        }

        private void UpdateEmployeeButton_Click(object sender, EventArgs e)
        {
            if (EmployeeNameText.Text == "" || EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入姓名和人员ID", @"提示");
                return;
            }

            PersonInfo updatedPerson = mRemote.UpdatePerson(DevPassword, EmployeeIDText.Text, EmployeeCertificateText.Text, EmployeeNameText.Text);
            if (updatedPerson == null)
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                try
                {

                    EmployeeIDText.Text = updatedPerson.id;
                    EmployeeCertificateText.Text = updatedPerson.idcardNum;
                    EmployeeNameText.Text = updatedPerson.name;
                    Logd("The person information has been updated.");
                    FaceIDText.Text = "";

                }
                catch (Exception ex)
                {
                    Logd("Updating person information failed. (" + ex.Message + ")");
                }

            }
        }

        private void DeleteEmployeeButton_Click(object sender, EventArgs e)
        {
            if (EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入人员ID", @"提示");
                return;
            }

            string szQuery = mRemote.DeletePerson(DevPassword, EmployeeIDText.Text);
            if (szQuery == "")
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Deleting person information successed.");
                Logd(szQuery);
            }
        }

        private void QueryEmployeeButton_Click(object sender, EventArgs e)
        {
            if (EmployeeIDText.Text == "")
            {
                // MessageBox.Show(@"请输入人员ID", @"提示");
                // return;
                EmployeeIDText.Text = "-1"; // 自動設置為-1.
            }

            SouSuoView.Items.Clear();
            PersonInfo[] persons = mRemote.FindPerson(DevPassword, EmployeeIDText.Text, 0, int.Parse(RenLianSouSuoShuLiang.Text));
            if (persons == null)
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Finding person information sucessfully.");
                try
                {
                    {
                        // string testTime = unixTime.ToString();
                        DateTime unixTime;

                        //string szOutput;
                        foreach (PersonInfo info in persons)
                        {
                            //szOutput = JsonConvert.SerializeObject(info);
                            ListViewItem lvi = new ListViewItem();

                            lvi.Text = info.name;
                            lvi.SubItems.Add(info.id);
                            lvi.SubItems.Add(info.idcardNum);
                            unixTime = GetTime(info.createTime);
                            lvi.SubItems.Add(unixTime.ToShortDateString() + " " + unixTime.ToShortTimeString());
                            lvi.Tag = info.id;
                            SouSuoView.Items.Add(lvi);
                            //Logd(szOutput);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logd(ex.Message);
                }

            }
        }

        private void btnSetTimePeriod_Click(object sender, EventArgs e)
        {
            string jsonStr;
            if (EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入人员ID", @"提示");
                EmployeeIDText.Focus();
                return;
            }

            PathTime pt = new PathTime(EmployeeIDText.Text);
            if (RenYuanFirstStartTime.Text != "  :  :" && RenYuanFirstEndTime.Text != " : : ")
            {
                pt.AddTime(RenYuanFirstStartTime.Text, RenYuanFirstEndTime.Text);
            }

            if (RenYuanSecondStartTime.Text != "  :  :" && RenYuanSecondEndTime.Text != "  :  :")
            {
                pt.AddTime(RenYuanSecondStartTime.Text, RenYuanSecondEndTime.Text);
            }

            if (RenYuanThirdStartTime.Text != "  :  :" && RenYuanThirdEndTime.Text != "  :  :")
            {
                pt.AddTime(RenYuanThirdStartTime.Text, RenYuanThirdEndTime.Text);
            }

            if (pt.TimeIsEmpty())
            {
                MessageBox.Show(@"请输入时间段权限", @"提示");
                RenYuanFirstStartTime.Focus();
            }


            jsonStr = JsonConvert.SerializeObject(pt);
            if (!mRemote.CreatePassTime(DevPassword, jsonStr))
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Passtime set was successful.");
            }
        }

        private void RenYuanFirstStartTime_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;

            }
        }

        private void RenYuanFirstStartTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                //e.SuppressKeyPress = true;

            }
        }

        private void btnDeleteTimePeriod_Click(object sender, EventArgs e)
        {
            if (EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入人员ID", @"提示");
                EmployeeIDText.Focus();
                return;
            }

            if (!mRemote.DeletePassTime(DevPassword, EmployeeIDText.Text))
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Deleting passtime was successed.");
            }

        }

        private void SetPermButton_Click(object sender, EventArgs e)
        {
            if (EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入人员ID", @"提示");
                EmployeeIDText.Focus();
                return;
            }

            string szTime;

            szTime =
                PermDataPicker.Value.Year.ToString() + "-" +
                string.Format("{0:D2}", PermDataPicker.Value.Month) + "-" + 
                string.Format("{0:D2}" , PermDataPicker.Value.Day) + " " + 
                PermTimePicker.Text;
            if (!mRemote.CreatePermission(DevPassword, EmployeeIDText.Text, szTime))
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Person's permission was created.");
            }
        }

        private void DelPermButton_Click(object sender, EventArgs e)
        {
            if (EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入人员ID", @"提示");
                EmployeeIDText.Focus();
                return;
            }


            if (!mRemote.PermissionDelete(DevPassword, EmployeeIDText.Text))
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Person's permission was deleted.");
            }
        }

        private void searchShiBieJiLu_Click(object sender, EventArgs e)
        {
            if (JiLuPersonID.Text == "")
            {
                JiLuPersonID.Text = "-1"; //自動設置为-1
                //MessageBox.Show(@"请输入人员ID", @"提示");
                //JiLuPersonID.Focus();
                //return;
            }

            string szSTime, szETime;
            JiLuView.Items.Clear();
            szSTime =
                string.Format("{0:D2}", JiLuKaiShiTime.Value.Year) + "-" +
                string.Format("{0:D2}", JiLuKaiShiTime.Value.Month) + "-" +
                string.Format("{0:D2}", JiLuKaiShiTime.Value.Day) + " " +
                /*dateTimePickerStart.Value.Hour.ToString()*/"00" + ":" +
                /*dateTimePickerStart.Value.Minute.ToString()*/"00" + ":" +
                /*dateTimePickerStart.Value.Second.ToString()*/"00" + "";

            szETime =
                string.Format("{0:D2}", JiLuJieShuTime.Value.Year) + "-" +
                string.Format("{0:D2}", JiLuJieShuTime.Value.Month) + "-" +
                string.Format("{0:D2}", JiLuJieShuTime.Value.Day) + " " +

                /*dateTimePickerEnd.Value.Hour.ToString()*/ "24" + ":" +
                /*dateTimePickerEnd.Value.Minute.ToString()*/"00" + ":" +
                /*dateTimePickerEnd.Value.Second.ToString()*/"00" + "";

            FindRecordResult<FaceRecord> records = mRemote.FindRecords(DevPassword, JiLuPersonID.Text, JiLuSearhCount.Text, JiLuSearchIndex.Text, szSTime, szETime);
            if (records == null)
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Finding records was sucessed.");
                try
                {


                    if (records.records == null || records.pageinfo.total == 0)
                    {
                        Logd("But there is no any record.");
                    }
                    else
                    {
                        DateTime unixTime;
                        //string szOutput;
                        Logd("PageInfo total data:" + records.pageinfo.total + " , page count: " + records.pageinfo.size);
                        foreach (FaceRecord info in records.records)
                        {
                            //szOutput = JsonConvert.SerializeObject(info);
                            ListViewItem lvi = new ListViewItem();

                            if (info.name == null || info.name == "")
                                lvi.Text = "未定";
                            else
                                lvi.Text = info.name; //info.id.ToString();
                            lvi.SubItems.Add(info.personId);
                            unixTime = GetTime(info.time);
                            lvi.SubItems.Add(unixTime.ToShortDateString() + " " + unixTime.ToShortTimeString());
 
                            if (info.type == 0)
                                lvi.SubItems.Add("时间段内");
                            else if (info.type == 1)
                                lvi.SubItems.Add("时间段外");
                            else
                                lvi.SubItems.Add("陌生人/识别失败");

                            lvi.Tag = info.id;
                            JiLuView.Items.Add(lvi);
                            //Logd(szOutput);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logd(ex.Message);
                }

            }
        }

        private void searchShuaKaJiLu_Click(object sender, EventArgs e)
        {
            if (JiLuPersonID.Text == "")
            {
                JiLuPersonID.Text = "-1"; //自動設置为-1
                //MessageBox.Show(@"请输入人员ID", @"提示");
                //JiLuPersonID.Focus();
                //return;
            }

            string szSTime, szETime;
            JiLuView.Items.Clear();
            szSTime =
                string.Format("{0:D2}", JiLuKaiShiTime.Value.Year) + "-" +
                string.Format("{0:D2}", JiLuKaiShiTime.Value.Month) + "-" +
                string.Format("{0:D2}", JiLuKaiShiTime.Value.Day) + " " +
                /*dateTimePickerStart.Value.Hour.ToString()*/"00" + ":" +
                /*dateTimePickerStart.Value.Minute.ToString()*/"00" + ":" +
                /*dateTimePickerStart.Value.Second.ToString()*/"00" + "";

            szETime =
                string.Format("{0:D2}", JiLuJieShuTime.Value.Year) + "-" +
                string.Format("{0:D2}", JiLuJieShuTime.Value.Month) + "-" +
                string.Format("{0:D2}", JiLuJieShuTime.Value.Day) + " " +

                /*dateTimePickerEnd.Value.Hour.ToString()*/ "24" + ":" +
                /*dateTimePickerEnd.Value.Minute.ToString()*/"00" + ":" +
                /*dateTimePickerEnd.Value.Second.ToString()*/"00" + "";

            FindRecordResult<CardRecord> records = mRemote.FindCardRecords(DevPassword, JiLuPersonID.Text, JiLuSearhCount.Text, JiLuSearchIndex.Text, szSTime, szETime);
            if (records == null)
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Finding records was sucessed.");
                try
                {


                    if (records.records == null || records.pageinfo.total == 0)
                    {
                        Logd("But there is no any record.");
                    }
                    else
                    {
                        DateTime unixTime;
                        //string szOutput;
                        Logd("PageInfo total data:" + records.pageinfo.total + " , page count: " + records.pageinfo.size);
                        foreach (CardRecord info in records.records)
                        {
                            //szOutput = JsonConvert.SerializeObject(info);
                            ListViewItem lvi = new ListViewItem();

                            lvi.Text = info.name; //info.name;
                            lvi.SubItems.Add(info.personId);
                            unixTime = GetTime(info.time);
                            lvi.SubItems.Add(unixTime.ToShortDateString() + " " + unixTime.ToShortTimeString());
                            if (info.type == 0)
                                lvi.SubItems.Add("时间段内");
                            else if (info.type == 2)
                                lvi.SubItems.Add("时间段外");
                            else
                                lvi.SubItems.Add("陌生人/识别失败");

                            lvi.Tag = info.id;
                            JiLuView.Items.Add(lvi);
                            //Logd(szOutput);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logd(ex.Message);
                }

            }
        }

        private void DeleteShiBieJiLu_Click(object sender, EventArgs e)
        {

            string szSTime, szETime;

            if (JiLuPersonID.Text == "")
            {
                JiLuPersonID.Text = "-1"; //自動設置为-1
                //MessageBox.Show(@"请输入人员ID", @"提示");
                //JiLuPersonID.Focus();
                //return;
            }

            szSTime =
                 string.Format("{0:D2}", JiLuKaiShiTime.Value.Year) + "-" +
                 string.Format("{0:D2}", JiLuKaiShiTime.Value.Month) + "-" +
                 string.Format("{0:D2}", JiLuKaiShiTime.Value.Day) + " " +
                 /*dateTimePickerStart.Value.Hour.ToString()*/"00" + ":" +
                 /*dateTimePickerStart.Value.Minute.ToString()*/"00" + ":" +
                 /*dateTimePickerStart.Value.Second.ToString()*/"00" + "";

            szETime =
                string.Format("{0:D2}", JiLuJieShuTime.Value.Year) + "-" +
                string.Format("{0:D2}", JiLuJieShuTime.Value.Month) + "-" +
                string.Format("{0:D2}", JiLuJieShuTime.Value.Day) + " " +

                /*dateTimePickerEnd.Value.Hour.ToString()*/ "24" + ":" +
                /*dateTimePickerEnd.Value.Minute.ToString()*/"00" + ":" +
                /*dateTimePickerEnd.Value.Second.ToString()*/"00" + "";
            JiLuView.Items.Clear();
            string deleteResult = mRemote.DeleteRecords(DevPassword, JiLuPersonID.Text, szETime, false);
            Logd(deleteResult);

            // 测试用
           // deleteResult = mRemote.DeleteRecordsByUnixTime(DevPassword, JiLuPersonID.Text, szETime, false);
           // Logd(deleteResult);
        }

        private void deleteShuaKaJiLu_Click(object sender, EventArgs e)
        {
            string szSTime, szETime;
            if (JiLuPersonID.Text == "")
            {
                JiLuPersonID.Text = "-1"; //自動設置为-1
                //MessageBox.Show(@"请输入人员ID", @"提示");
                //JiLuPersonID.Focus();
                //return;
            }

            szSTime =
                string.Format("{0:D2}", JiLuKaiShiTime.Value.Year) + "-" +
                string.Format("{0:D2}", JiLuKaiShiTime.Value.Month) + "-" +
                string.Format("{0:D2}", JiLuKaiShiTime.Value.Day) + " " +
                /*dateTimePickerStart.Value.Hour.ToString()*/"00" + ":" +
                /*dateTimePickerStart.Value.Minute.ToString()*/"00" + ":" +
                /*dateTimePickerStart.Value.Second.ToString()*/"00" + "";

            szETime =
                string.Format("{0:D2}", JiLuJieShuTime.Value.Year) + "-" +
                string.Format("{0:D2}", JiLuJieShuTime.Value.Month) + "-" +
                string.Format("{0:D2}", JiLuJieShuTime.Value.Day) + " " +

                /*dateTimePickerEnd.Value.Hour.ToString()*/ "24" + ":" +
                /*dateTimePickerEnd.Value.Minute.ToString()*/"00" + ":" +
                /*dateTimePickerEnd.Value.Second.ToString()*/"00" + "";

            JiLuView.Items.Clear();
            string delResult = mRemote.DeleteRecords(DevPassword, JiLuPersonID.Text, szETime, true);
            Logd(delResult);

            // 测试用
            //delResult = mRemote.DeleteRecordsByUnixTime(DevPassword, JiLuPersonID.Text, szETime, true);
            //Logd(delResult);
        }

        private void SelectEmployeePictureButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                RenLianTuPian.ImageLocation = dlg.FileName;
            }
        }

        private void SetEmployeePictureButton_Click(object sender, EventArgs e)
        {
            /*if (picBoxEmployeeFace.ImageLocation == "" || picBoxEmployeeFace.ImageLocation == null)
            {
                MessageBox.Show(@"请加载人脸照片", @"提示");
                return;
            }*/

            if (EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入人员ID", @"提示");
                EmployeeIDText.Focus();
                return;
            }

            if (!mRemote.TakeImage(DevPassword, EmployeeIDText.Text))
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Take image was successed.");
            }

        }

        private void CreateEmployeeFaceButton_Click(object sender, EventArgs e)
        {
            if (RenLianTuPian.ImageLocation == "" || RenLianTuPian.ImageLocation == null)
            {
                MessageBox.Show(@"请加载人脸照片", @"提示");
                return;
            }

            string createdFaceID;
            createdFaceID = mRemote.CreateFace(DevPassword, EmployeeIDText.Text, FaceIDText.Text, RenLianTuPian.ImageLocation);
            if (createdFaceID == null || createdFaceID == "")
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Face information was created.");
                FaceIDText.Text = createdFaceID;
            }

            // 下个部分只是测试用。
            //mRemote.FeatureReg(DevPassword, EmployeeIDText.Text," "," "," "); // 在我的设备上没有特征登录功能。
            //Logd(mRemote.GetErrorString());
            //mRemote.CompPhoto(DevPassword, " ", " "); // 也不提供照片比较功能。
            //Logd(mRemote.GetErrorString());

        }

        private void UpdateEmployeeFacePictureButton_Click(object sender, EventArgs e)
        {
            if (RenLianTuPian.ImageLocation == "" || RenLianTuPian.ImageLocation == null)
            {
                MessageBox.Show(@"请加载人脸照片", @"提示");
                return;
            }

            if (EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入人员ID", @"提示");
                EmployeeIDText.Focus();
                return;
            }

            if (FaceIDText.Text == "")
            {
                MessageBox.Show(@"请输入人脸ID", @"提示");
                FaceIDText.Focus();
                return;
            }

            if (!mRemote.UpdateFace(DevPassword, EmployeeIDText.Text, FaceIDText.Text, RenLianTuPian.ImageLocation))
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Face information was updated.");
            }
        }

        private void DeleteEmployeeFacePictureButton_Click(object sender, EventArgs e)
        {
            if (FaceIDText.Text == "")
            {
                MessageBox.Show(@"请输入人脸ID", @"提示");
                FaceIDText.Focus();
                return;
            }

            if (!mRemote.DeleteFace(DevPassword, FaceIDText.Text))
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Face information was deleted.");
            }
        }

        private void DeleteEmployeeFaceButton_Click(object sender, EventArgs e)
        {
            if (EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入人员ID", @"提示");
                EmployeeIDText.Focus();
                return;
            }

            if (!mRemote.ClearFace(DevPassword, EmployeeIDText.Text))
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Face information was  cleared.");
            }
        }

        public FtpWebRequest FtpGetRequest(string URI, string username, string password)
        {
            //根据服务器信息FtpWebRequest创建类的对象
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(URI);
            //提供身份验证信息
            result.Credentials = new System.Net.NetworkCredential(username, password);
            //设置请求完成之后是否保持到FTP服务器的控制连接，默认值为true
            result.KeepAlive = false;
            return result;
        }

        private void QueryEmployeeFaceButton_Click(object sender, EventArgs e)
        {
            if (EmployeeIDText.Text == "")
            {
                MessageBox.Show(@"请输入人员ID", @"提示");
                EmployeeIDText.Focus();
                return;
            }
            FindFace[] faceinfo;
            faceinfo = mRemote.FindFace(DevPassword, EmployeeIDText.Text);
            if (faceinfo == null)
            {
                Logd(mRemote.GetErrorString());
            }
            else
            {
                Logd("Finding face information sucessfully.");
                if (!Directory.Exists("images"))
                    Directory.CreateDirectory("images");

                foreach (FindFace info in faceinfo)
                {
                    try
                    {
                        int si = info.path.LastIndexOf("/") + 1;
                        int el = info.path.Length;
                        string fname = info.path.Substring(si, el - si);
                        string localfile = "images" + @"\" + fname;

                        System.Net.FtpWebRequest ftp = FtpGetRequest(info.path, "", "");
                        ftp.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
                        ftp.UseBinary = true;
                        ftp.UsePassive = false;
                        ftp.Timeout = 5000;
                        ftp.UsePassive = true;
                        using (FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
                        {
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                using (FileStream fs = new FileStream(localfile, FileMode.Create))
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

                        /* int seqence = 0;
                         string createionPath;

                         {
                             Logd("  Face id:" + faceinfo.faceId);
                             byte[] bImg = Convert.FromBase64String(faceinfo.feature);
                             createionPath = "images/" + faceinfo.faceId + ".jpg";
                             FaceIDText.Text = faceinfo.faceId;
                             FileStream _fs = File.Create(createionPath);
                             _fs.Write(bImg, 0, bImg.Length);
                             _fs.Close();*/
                        RenLianTuPian.ImageLocation = localfile; //createionPath;
                        FaceIDText.Text = info.faceId;
                        Logd(info.faceId);
                        /*   seqence++;
                       }*/

                    }
                    catch (Exception ex)
                    {
                        Logd(ex.Message);
                    }
                }

            }
        }

        private void txtThreshold_Scroll(object sender, EventArgs e)
        {
            lbFenshu.Text = ShiBieThreshold.Value + "0";
        }

        private void ClearLog_Click(object sender, EventArgs e)
        {
            HuiTiaoLog.Text = "";
        }

        void MainButtonsLock()
        {
            SheBeiLianJieButton.Enabled = false;
            SheBeiTianJiaButton.Enabled = false;
            BianJiButton.Enabled = false;
            ShanChuButton.Enabled = false;
            if (!IsSearch)
                SheBeiSouSuoButton.Enabled = false;
            SheHuiTiaoButton.Enabled = false;
        }

        void MainButtonsUnlock()
        {
            if (!IsSearch)
            {
                SheBeiLianJieButton.Enabled = true;
                SheBeiTianJiaButton.Enabled = true;
                BianJiButton.Enabled = true;
                ShanChuButton.Enabled = true;
                SheBeiSouSuoButton.Enabled = true;
                SheHuiTiaoButton.Enabled = true;
            }
        }

        private void SheBeiTianJiaButton_Click(object sender, EventArgs e)
        {
            EditID = -1;
            Animation.Init();
            Animation.ShowControl(SheBeiViewPanel, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(HuiTiaoPanel, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(HuiTiaoPanel, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(RemoteFrame, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(SheBeiTianJiaPanel, true, AnchorStyles.Right);
            SheBeiTianJiaMingCheng.Text = "";
            SheBeiTianJiaIP.Text = "";
            SheBeiTianJiaMiMa.Text = "";
            SheBeiTianJiaPort.Text = "";
            SheBeiTianJiaMingCheng.Focus();
            MainButtonsLock();

        }

        private void MainManagerPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        public static bool ValidateIPAddress(string ipAddress)
        {
            Regex validipregex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            return (ipAddress != "" && validipregex.IsMatch(ipAddress.Trim())) ? true : false;
        }

        private void RefreshMachineList()
        {
            if (IsSearch)
                return;

            try
            {
                int ndx = 1;
                // refresh list view control
                List<MachineInfo> info = SqliteHelper.QueryMachineList();
                if (info != null)
                {
                    SheBeiView.Items.Clear();
                    SheBeiView.BeginUpdate();
                    foreach (MachineInfo ifs in info)
                    {

                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = ndx/*ifs.id*/.ToString();
                        ndx++;
                        lvi.SubItems.Add(ifs.name);
                        lvi.SubItems.Add(ifs.ip);
                        lvi.SubItems.Add(ifs.port.ToString());
                        lvi.Tag = ifs.id;
                        SheBeiView.Items.Add(lvi);
                    }
                    SheBeiView.EndUpdate();
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private void SheBeiTianJiaOK_Click(object sender, EventArgs e)
        {
            if (SheBeiTianJiaMingCheng.Text == "")
            {
                MessageBox.Show(@"请输入设备名称", @"提示");
                SheBeiTianJiaMingCheng.Focus();
                return;
            }

            if (SheBeiTianJiaIP.Text == "" || !ValidateIPAddress(SheBeiTianJiaIP.Text))
            {
                MessageBox.Show(@"请输入设备正确的IP", @"提示");
                SheBeiTianJiaIP.Focus();
                return;
            }

            Regex exp = new Regex("[0-9]"); // Caution: android
            if (SheBeiTianJiaPort.Text == "" || !exp.Match(SheBeiTianJiaPort.Text).Success)
            {
                MessageBox.Show(@"请输入设备正确的PORT", @"提示");
                SheBeiTianJiaPort.Focus();
                return;
            }

            /*if (SheBeiTianJiaMiMa.Text == "")
            {
                MessageBox.Show(@"请输入通讯密码", @"提示");
                SheBeiTianJiaMiMa.Focus();
                return;
            }*/

            try
            {
                string szQuery, szResult;
                if (EditID >= 0)
                {
                    // existing info edit
                    szQuery = "update MachineList SET " +
                        "Name='" + SheBeiTianJiaMingCheng.Text + "'," +
                        "IPAddr='" + SheBeiTianJiaIP.Text + "'," +
                        "Port='" + SheBeiTianJiaPort.Text + "'," +
                        "Pwd='" + SheBeiTianJiaMiMa.Text + "' where id='" + EditID + "'";
                    SqliteHelper.ExecuteCommand(szQuery);
                }
                else
                {
                    // New info add.
                    szQuery = "select id from MachineList where IPAddr='" + SheBeiTianJiaIP.Text + "'";
                    szResult = SqliteHelper.ExecuteCommand(szQuery);
                    if (szResult != "")
                    {
                        MessageBox.Show(@"已存在与您输入的IP一致的设备", @"提示");
                        SheBeiTianJiaIP.Focus();
                        return;
                    }
                    szQuery = "insert into MachineList(Name,IPAddr,Port,Pwd) values('" +
                    SheBeiTianJiaMingCheng.Text + "','" + SheBeiTianJiaIP.Text + "','" + int.Parse(SheBeiTianJiaPort.Text) + "','" + SheBeiTianJiaMiMa.Text + "')";
                    SqliteHelper.ExecuteCommand(szQuery);
                }
                RefreshMachineList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"提示");
            }

            Animation.Init();
            Animation.ShowControl(SheBeiTianJiaPanel, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(HuiTiaoPanel, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(RemoteFrame, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(SheBeiViewPanel, true, AnchorStyles.Right);
            MainButtonsUnlock();
        }

        private void SheBeiTianJiaCancel_Click(object sender, EventArgs e)
        {
            if (IsSearch)
            {
                Animation.Init();
                Animation.ShowControl(SheBeiTianJiaPanel, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(HuiTiaoPanel, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(RemoteFrame, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(SheBeiViewPanel, true, AnchorStyles.Right);
            }
            else
            {
                Animation.Init();
                Animation.ShowControl(SheBeiTianJiaPanel, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(HuiTiaoPanel, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(RemoteFrame, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(SheBeiViewPanel, true, AnchorStyles.Right);
                MainButtonsUnlock();
            }
        }

        private void BianJiButton_Click(object sender, EventArgs e)
        {
            if (SheBeiView.SelectedItems.Count > 1 && !IsSearch)
            {
                MessageBox.Show(@"不能同时编辑1个以上的项目", @"提示");
                return;
            }

            if (SheBeiView.SelectedItems.Count == 0 && !IsSearch)
            {
                MessageBox.Show(@"请选择欲编辑的项目", @"提示");
                return;
            }

            try
            {
                if (!IsSearch)
                {
                    foreach (ListViewItem lvi in SheBeiView.SelectedItems)
                    {
                        string szQuery, szResult;
                        szQuery = "select Name from MachineList where id='" + lvi.Tag + "'";
                        szResult = SqliteHelper.ExecuteCommand(szQuery);
                        SheBeiTianJiaMingCheng.Text = szResult;

                        szQuery = "select IPAddr from MachineList where id='" + lvi.Tag + "'";
                        szResult = SqliteHelper.ExecuteCommand(szQuery);
                        SheBeiTianJiaIP.Text = szResult;

                        szQuery = "select Port from MachineList where id='" + lvi.Tag + "'";
                        szResult = SqliteHelper.ExecuteCommand(szQuery);
                        SheBeiTianJiaPort.Text = szResult;

                        szQuery = "select Pwd from MachineList where id='" + lvi.Tag + "'";
                        szResult = SqliteHelper.ExecuteCommand(szQuery);
                        SheBeiTianJiaMiMa.Text = szResult;

                        EditID = (int)lvi.Tag;
                    }
                }
                else
                {
                    // called by sarch state.
                    foreach (ListViewItem lvi in SheBeiView.SelectedItems)
                    {
                        SheBeiTianJiaMingCheng.Text = lvi.SubItems[1].Text;
                        SheBeiTianJiaIP.Text = lvi.SubItems[2].Text;
                        SheBeiTianJiaPort.Text = "";
                        SheBeiTianJiaMiMa.Text = "";
                        string szQuery = "select id from MachineList where IPAddr='" + lvi.SubItems[2].Text + "'";
                        string szResult = SqliteHelper.ExecuteCommand(szQuery);
                        if (szResult != "")
                        {
                            MessageBox.Show(@"已登记与您选择的IP一致的设备", @"提示");
                            return;
                        }
                    }
                }


                Animation.Init();
                Animation.ShowControl(SheBeiViewPanel, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(RemoteFrame, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(HuiTiaoPanel, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(SheBeiTianJiaPanel, true, AnchorStyles.Right);
                SheBeiTianJiaMingCheng.Focus();
                MainButtonsLock();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void SheBeiView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SheBeiView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            BianJiButton_Click(this, null);
        }

        private void ShanChuButton_Click(object sender, EventArgs e)
        {

            if (SheBeiView.SelectedItems.Count == 0)
            {
                MessageBox.Show(@"请选择欲删除的项目", @"提示");
                return;
            }

            if (MessageBox.Show("确定要删除真正选择的项目吗?", @"提示", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            try
            {
                foreach (ListViewItem lvi in SheBeiView.SelectedItems)
                {
                    string szQuery;
                    // existing info edit
                    szQuery = "delete from  MachineList where id='" + lvi.Tag + "'";
                    SqliteHelper.ExecuteCommand(szQuery);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            RefreshMachineList();
        }

        private void SheBeiSouSuoButton_Click(object sender, EventArgs e)
        {
            try
            {

                Animation.Init();
                Animation.ShowControl(SheBeiTianJiaPanel, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(HuiTiaoPanel, false, AnchorStyles.Right);
                Animation.Init();
                Animation.ShowControl(RemoteFrame, false, AnchorStyles.Right);

                if (IsSearch)
                {
                    IsSearch = false;
                    Animation.Init();
                    Animation.ShowControl(SheBeiViewPanel, true, AnchorStyles.Left);
                    MainButtonsUnlock();
                    SheBeiSouSuoButton.Text = "搜索";
                    RefreshMachineList();

                }
                else
                {
                    Animation.Init();
                    Animation.ShowControl(SheBeiViewPanel, true, AnchorStyles.Right);
                    SheBeiSouSuoButton.Text = "停止搜索";
                    IsSearch = true;
                    MainButtonsLock(); // must call after IsSearch setting.
                    // serach 
                    SheBeiView.Items.Clear();
                    //BroadCastPort
                    Thread t = new Thread(BroadCastRecvTh);
                    t.IsBackground = true;
                    t.Start();
                }
            }
            catch (Exception)
            {
            }

        }

        public void BroadCastRecvTh()
        {
            UdpClient client = null;
            IPEndPoint endpoint = null;
            try
            {
                client = new UdpClient(new IPEndPoint(System.Net.IPAddress.Any, BroadCastPort));
                endpoint = new IPEndPoint(IPAddress.Any, 0);
            }
            catch (SocketException e)
            {
                if (!SearchThreadIsRunning)
                {
                    MessageBox.Show(e.Message, @"提示");
                }
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, @"提示");
            }

            while (IsSearch)
            {
                SearchThreadIsRunning = true;
                try
                {
                    byte[] buf = client.Receive(ref endpoint);
                    string _msg = Encoding.UTF8.GetString(buf);
                    BroadCastMsg msg = JsonConvert.DeserializeObject<BroadCastMsg>(_msg);

                    if (msg.ident == "Hysoon" && IsSearch)
                    {
                        Boolean IsExist = false;
                        ListView.ListViewItemCollection items = null;
                        ListViewItem lvi = new ListViewItem();

                        lvi.Text = "*";
                        lvi.SubItems.Add(msg.msg);
                        lvi.SubItems.Add(msg.ip);
                        lvi.SubItems.Add("***");
                        lvi.Tag = -1;

                        // Check duplicate ip address.
                        if (SheBeiView.InvokeRequired)
                        {
                            SheBeiView.Invoke(new Action<int>(s =>
                            {
                                items = SheBeiView.Items;
                                foreach (ListViewItem e in items)
                                {
                                    if (e.SubItems[2].Text == msg.ip)
                                        IsExist = true;
                                }
                            }), 0);
                        }
                        else
                        {
                            items = SheBeiView.Items;
                            foreach (ListViewItem e in items)
                            {
                                if (e.SubItems[2].Text == msg.ip)
                                    IsExist = true;
                            }
                        }

                        if (!IsExist)
                        {
                            // add search result to list view.
                            if (SheBeiView.InvokeRequired)
                            {
                                SheBeiView.Invoke(new Action<ListViewItem>(s =>
                                {
                                    SheBeiView.Items.Add(s);
                                }), lvi);
                            }
                            else
                            {
                                SheBeiView.Items.Add(lvi);
                            }
                        }
                    }


                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
                Thread.Sleep(500);
            }

            if (client != null) client.Close();
            SearchThreadIsRunning = false;
        }

        private void SheBeiLianJieButton_Click(object sender, EventArgs e)
        {
            if (SheBeiView.SelectedItems.Count > 1)
            {
                MessageBox.Show(@"很抱歉,不能同时连接多个设备", @"提示");
                return;
            }

            if (SheBeiView.SelectedItems.Count == 0)
            {
                MessageBox.Show(@"请至少选一个设备", @"提示");
                return;
            }

            MainButtonsLock();
            SheBeiView.Enabled = false;
            foreach (ListViewItem lvi in SheBeiView.SelectedItems)
            {
                string szQuery;
                szQuery = "select IPAddr from MachineList where id='" + lvi.Tag + "'";
                DevIp = SqliteHelper.ExecuteCommand(szQuery);

                szQuery = "select Port from MachineList where id='" + lvi.Tag + "'";
                DevPort = int.Parse(SqliteHelper.ExecuteCommand(szQuery));

                szQuery = "select Pwd from MachineList where id='" + lvi.Tag + "'";
                DevPassword = SqliteHelper.ExecuteCommand(szQuery);
                break;
            }
            try
            {

                mWorker.RunWorkerAsync();
                while (mWorker.IsBusy)
                {
                    // ...
                    Application.DoEvents();
                }
            }
            catch (Exception)
            {

            }
        }

        private void ControlPanel_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = ControlPanel.ClientRectangle;


            // Brush b11 = new SolidBrush(Color.FromArgb(255, 255, 255)); //选中的项的背景色;
            // g.FillRectangle(b11, rect); //改变选项卡标签的背景色; 

            Rectangle r = this.ControlPanel.GetTabRect(e.Index);
            if (e.Index == this.ControlPanel.SelectedIndex)    //当前选中的Tab页，设置不同的样式以示选中;
            {
                Brush selected_color = new SolidBrush(Color.FromArgb(17, 105, 173)); //选中的项的背景色;
                g.FillRectangle(selected_color, r); //改变选项卡标签的背景色; 
                string title = this.ControlPanel.TabPages[e.Index].Text + "   ";
                g.DrawString(title, this.Font, new SolidBrush(Color.White), new PointF(r.X + 3, r.Y + 6));//PointF选项卡标题的位置;               
            }
            else//非选中的;
            {
                Brush biaocolor = new SolidBrush(Color.FromArgb(44, 157, 249));
                g.FillRectangle(biaocolor, r); //改变选项卡标签的背景色; 
                string title = this.ControlPanel.TabPages[e.Index].Text + "   ";
                g.DrawString(title, this.Font, new SolidBrush(Color.White), new PointF(r.X + 3, r.Y + 6));//PointF选项卡标题的位置;               
            }
        }

        private void SheHuiTiaoButton_Click(object sender, EventArgs e)
        {
            Animation.Init();
            Animation.ShowControl(SheBeiViewPanel, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(RemoteFrame, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(SheBeiTianJiaPanel, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(HuiTiaoPanel, true, AnchorStyles.Right);
            MainButtonsLock();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Animation.Init();
            Animation.ShowControl(RemoteFrame, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(SheBeiTianJiaPanel, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(HuiTiaoPanel, false, AnchorStyles.Right);
            Animation.Init();
            Animation.ShowControl(SheBeiViewPanel, true, AnchorStyles.Right);
            MainButtonsUnlock();
        }

        private void HuiTiaoClearButton_Click(object sender, EventArgs e)
        {
            HuiTiaoLog.Text = "";
        }

        private void RemoteControlPanel_Click(object sender, EventArgs e)
        {

        }

        private void SouSuoResult_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SouSuoResult_MouseClick(object sender, MouseEventArgs e)
        {
            if (SouSuoView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in SouSuoView.SelectedItems)
                {
                    EmployeeIDText.Text = lvi.SubItems[1].Text;
                    EmployeeNameText.Text = lvi.SubItems[0].Text;
                    EmployeeCertificateText.Text = lvi.SubItems[2].Text;
                }
            }
        }

        private void SouSuoResult_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SouSuoView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in SouSuoView.SelectedItems)
                {
                    EmployeeIDText.Text = lvi.SubItems[1].Text;
                    FaceIDText.Text = lvi.SubItems[2].Text;
                }
            }
        }

        private void JiLuView_MouseClick(object sender, MouseEventArgs e)
        {
            if (JiLuView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in JiLuView.SelectedItems)
                {
                    if (JiLuPersonID.Visible)
                        JiLuPersonID.Text = lvi.SubItems[1].Text;
                }
            }
        }

        private void NetOptionSwitch()
        {
            if (WifiOption.Checked)
            {
                WiFiName.Enabled = true;
                WiFiPwd.Enabled = true;
                MaskText.Enabled = false;
            }
            else
            {
                MaskText.Enabled = true;
                WiFiName.Enabled = false;
                WiFiPwd.Enabled = false;
            }
        }

        private void EthernetOption_CheckedChanged(object sender, EventArgs e)
        {
            NetOptionSwitch();
        }

        private void WifiOption_CheckedChanged(object sender, EventArgs e)
        {
            NetOptionSwitch();
        }

        private void JiLuView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CallBackPanel_Click(object sender, EventArgs e)
        {

        }
    }
}
