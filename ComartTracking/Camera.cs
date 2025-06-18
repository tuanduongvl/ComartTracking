using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ComartTracking
{
    public class Camera
    {

        private CHCNetSDK.NET_DVR_SHOWSTRING_V30 m_struShowStrCfg;

        public string IPaddress { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        HttpClient client = new HttpClient();
        private int m_lChannel;
        private int m_lUserID;
        private uint iLastErr;
        private string str;
        private string strErr;
        private CHCNetSDK.NET_DVR_DEVICEINFO m_struDeviceInfo;

        public CHCNetSDK.NET_DVR_DEVICECFG_V40 m_struDeviceCfg;
        private bool m_bInitSDK;
        private int m_lPlayHandle = -1;
        private System.Timers.Timer timerPlayback = new System.Timers.Timer();
        private System.Timers.Timer timerDownload = new System.Timers.Timer();
        public ProgressBar PlaybackprogressBar = new ProgressBar();
        private int m_lDownHandle = -1;

        public Camera(string iPaddress, int port, string username, string password)
        {
            IPaddress = iPaddress;
            this.port = port;
            this.username = username;
            this.password = password;
            client.BaseAddress = new Uri($"http://{IPaddress}:{port}/");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                MessageBox.Show("NET_DVR_Init error!");
                return;
            }
            timerDownload.Interval = 100;
            timerPlayback.Elapsed += new System.Timers.ElapsedEventHandler(timerPlayback_Tick);
            timerDownload.Elapsed += new System.Timers.ElapsedEventHandler(timerDownload_Tick);
        }

        private void timerDownload_Tick(object? sender, ElapsedEventArgs e)
        {
            int percent = CHCNetSDK.NET_DVR_GetDownloadPos(m_lDownHandle); //Get the download progress
            if (percent == 100)
            {
                if (!CHCNetSDK.NET_DVR_StopGetFile(m_lDownHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopGetFile failed, error code= " + iLastErr; //Download controlling failed,print error code
                    MessageBox.Show(str);
                    return;
                }
                m_lDownHandle = -1;
                timerDownload.Stop();
                OnDownloadCompleted?.Invoke(); // Trigger the event when download is completed
            }
        }

        // Fix for CS1001, CS1514, CS1513, CS0065, and CS8124 errors
        // The issue is with the declaration of the `DownloadCompleted` event. It is incorrectly defined.
        // Correcting the event declaration to follow proper syntax.

        public event DownloadCompleted? OnDownloadCompleted; // Declare the event properly

        // Ensure the delegate is defined correctly
        public delegate void DownloadCompleted();

        private void timerPlayback_Tick(object? sender, ElapsedEventArgs e)
        {
            PlaybackprogressBar.Maximum = 100;
            PlaybackprogressBar.Minimum = 0;

            uint iOutValue = 0;
            int iPos = 0;

            IntPtr lpOutBuffer = Marshal.AllocHGlobal(4);

            //get playback process
            CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, CHCNetSDK.NET_DVR_PLAYGETPOS, IntPtr.Zero, 0, lpOutBuffer, ref iOutValue);

            iPos = (int)Marshal.PtrToStructure(lpOutBuffer, typeof(int));

            if ((iPos > PlaybackprogressBar.Minimum) && (iPos < PlaybackprogressBar.Maximum))
            {
                PlaybackprogressBar.Value = iPos;
            }

            if (iPos == 100)  //Playback finished
            {
                PlaybackprogressBar.Value = iPos;
                if (!CHCNetSDK.NET_DVR_StopPlayBack(m_lPlayHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopPlayBack failed, error code= " + iLastErr; //Download controlling failed,print error code
                    MessageBox.Show(str);
                    return;
                }
                m_lPlayHandle = -1;
                timerPlayback.Stop();
            }

            if (iPos == 200) //Network abnormal,playback failed
            {
                MessageBox.Show("The playback is abnormal for the abnormal network!");
                timerPlayback.Stop();
            }
            Marshal.FreeHGlobal(lpOutBuffer);
        }

        public void tryfeature()
        {
            UInt32 dwReturn = 0;
            Int32 nSize = Marshal.SizeOf(m_struShowStrCfg);
            IntPtr ptrShowStrCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(m_struShowStrCfg, ptrShowStrCfg, false);
            if (!CHCNetSDK.NET_DVR_GetDVRConfig(m_lUserID, CHCNetSDK.NET_DVR_GET_SHOWSTRING_V30, m_lChannel, ptrShowStrCfg, (UInt32)nSize, ref dwReturn))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "NET_DVR_GET_SHOWSTRING_V30 failed, error code= " + iLastErr;
                //Failed to get overlay parameters and output the error code
                MessageBox.Show(strErr);
            }
            else
            {
                m_struShowStrCfg = (CHCNetSDK.NET_DVR_SHOWSTRING_V30)Marshal.PtrToStructure(ptrShowStrCfg, typeof(CHCNetSDK.NET_DVR_SHOWSTRING_V30));

            }


            //else
            //{
            //    MessageBox.Show("Set OSD parameters successfully！");
            //}
        }

        public void connect()
        {
            string DVRIPAddress = "192.168.1.168";
            ushort DVRPortNumber = 8000;
            string DVRUserName = "admin";
            string DVRPassword = "legion@25";
            //connect to camera
            m_lUserID = CHCNetSDK.NET_DVR_Login(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref m_struDeviceInfo);
            if (m_lUserID == -1)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "NET_DVR_Login failed, error code= " + iLastErr;
                //Failed to login and output the error code
                MessageBox.Show(strErr);
                return;
            }
            m_lChannel = 33;
            UInt32 dwReturn = 0;
            Int32 nSize = Marshal.SizeOf(m_struShowStrCfg);
            IntPtr ptrShowStrCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(m_struShowStrCfg, ptrShowStrCfg, false);
            if (!CHCNetSDK.NET_DVR_GetDVRConfig(m_lUserID, CHCNetSDK.NET_DVR_GET_SHOWSTRING_V30, m_lChannel, ptrShowStrCfg, (UInt32)nSize, ref dwReturn))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "NET_DVR_GET_SHOWSTRING_V30 failed, error code= " + iLastErr;
                //Failed to get overlay parameters and output the error code
                MessageBox.Show(strErr);
            }
            else
                m_struShowStrCfg = (CHCNetSDK.NET_DVR_SHOWSTRING_V30)Marshal.PtrToStructure(ptrShowStrCfg, typeof(CHCNetSDK.NET_DVR_SHOWSTRING_V30));

            Marshal.FreeHGlobal(ptrShowStrCfg);
        }


        public bool setOSD(string [] e)
        {
            //refesh all line osd
            for (int i = 0; i <8; i++)
            {
                m_struShowStrCfg.struStringInfo[i].wShowString = 0;
            }
            for (int i =0 ; i < e.Length; i++)
            {
                m_struShowStrCfg.struStringInfo[i].sString = e[i];
                m_struShowStrCfg.struStringInfo[i].wStringSize = (ushort)e[i].Length;
                m_struShowStrCfg.struStringInfo[i].wShowString = 1;
                m_struShowStrCfg.struStringInfo[i].wShowStringTopLeftX = 720/2;
                m_struShowStrCfg.struStringInfo[i].wShowStringTopLeftY = 10;
            }

            int nSize = Marshal.SizeOf(m_struShowStrCfg);
            nint ptrShowStrCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(m_struShowStrCfg, ptrShowStrCfg, false);

            if (!CHCNetSDK.NET_DVR_SetDVRConfig(m_lUserID, CHCNetSDK.NET_DVR_SET_SHOWSTRING_V30, m_lChannel, ptrShowStrCfg, (UInt32)nSize))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "NET_DVR_SET_SHOWSTRING_V30 failed, error code= " + iLastErr;
                //Failed to set overlay parameters and output the error code
                MessageBox.Show(strErr);
                return false;
            }
            Marshal.FreeHGlobal(ptrShowStrCfg);
            return true;
        }

        public void downloadByTime(DateTime startTime, DateTime endTime, string filename)
        {
            if (m_lDownHandle >= 0)
            {
                MessageBox.Show("Downloading, please stop firstly!");//Please stop downloading
                return;
            }

            CHCNetSDK.NET_DVR_PLAYCOND struDownPara = new CHCNetSDK.NET_DVR_PLAYCOND();
            struDownPara.dwChannel = 33; //Channel number  

            //Set the starting time
            struDownPara.struStartTime.dwYear = (uint)startTime.Year;
            struDownPara.struStartTime.dwMonth = (uint)startTime.Month;
            struDownPara.struStartTime.dwDay = (uint)startTime.Day;
            struDownPara.struStartTime.dwHour = (uint)startTime.Hour;
            struDownPara.struStartTime.dwMinute = (uint)startTime.Minute;
            struDownPara.struStartTime.dwSecond = (uint)startTime.Second;

            //Set the stopping time
            struDownPara.struStopTime.dwYear = (uint)endTime.Year;
            struDownPara.struStopTime.dwMonth = (uint)endTime.Month;
            struDownPara.struStopTime.dwDay = (uint)endTime.Day;
            struDownPara.struStopTime.dwHour = (uint)endTime.Hour;
            struDownPara.struStopTime.dwMinute = (uint)endTime.Minute;
            struDownPara.struStopTime.dwSecond = (uint)endTime.Second;

            File.Delete(filename); //Delete the file if it exists

            //Download by time
            m_lDownHandle = CHCNetSDK.NET_DVR_GetFileByTime_V40(m_lUserID, filename, ref struDownPara);
            if (m_lDownHandle < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_GetFileByTime_V40 failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return;
            }

            uint iOutValue = 0;
            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lDownHandle, CHCNetSDK.NET_DVR_PLAYSTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_PLAYSTART failed, error code= " + iLastErr; //Download controlling failed,print error code
                MessageBox.Show(str);
                return;
            }

            else timerDownload.Start();

        }
        public void syncTime()
        {


            CHCNetSDK.NET_DVR_TIME m_struTimeCfg = new CHCNetSDK.NET_DVR_TIME();
            m_struTimeCfg.dwYear = (uint)DateTime.Now.Year;
            m_struTimeCfg.dwMonth = (uint)DateTime.Now.Month;
            m_struTimeCfg.dwDay = (uint)DateTime.Now.Day;
            m_struTimeCfg.dwHour = (uint)DateTime.Now.Hour;
            m_struTimeCfg.dwMinute = (uint)DateTime.Now.Minute;
            m_struTimeCfg.dwSecond = (uint)DateTime.Now.Second;

            Int32 nSize = Marshal.SizeOf(m_struTimeCfg);
            IntPtr ptrTimeCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(m_struTimeCfg, ptrTimeCfg, false);

            if (!CHCNetSDK.NET_DVR_SetDVRConfig(m_lUserID, CHCNetSDK.NET_DVR_SET_TIMECFG, -1, ptrTimeCfg, (UInt32)nSize))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "NET_DVR_SET_TIMECFG failed, error code= " + iLastErr;
                //Failed to set the time of device and output the error code
                MessageBox.Show(strErr);
            }
            else
            {
                //MessageBox.Show("Time sync succeeded！");
            }

            Marshal.FreeHGlobal(ptrTimeCfg);
        }
        public void startPlayback(DateTime startTime, DateTime stopTime, nint pictureBoxHandle)
        {
            if (m_lPlayHandle >= 0)
            {
                //Please stop playback if playbacking now.
                if (!CHCNetSDK.NET_DVR_StopPlayBack(m_lPlayHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopPlayBack failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }

            }

            CHCNetSDK.NET_DVR_VOD_PARA struVodPara = new CHCNetSDK.NET_DVR_VOD_PARA();
            struVodPara.dwSize = (uint)Marshal.SizeOf(struVodPara);
            struVodPara.struIDInfo.dwChannel = 33; //Channel number  
            struVodPara.hWnd = pictureBoxHandle;//handle of playback

            //Set the starting time to search video files
            struVodPara.struBeginTime.dwYear = (uint)startTime.Year;
            struVodPara.struBeginTime.dwMonth = (uint)startTime.Month;
            struVodPara.struBeginTime.dwDay = (uint)startTime.Day;
            struVodPara.struBeginTime.dwHour = (uint)startTime.Hour;
            struVodPara.struBeginTime.dwMinute = (uint)startTime.Minute;
            struVodPara.struBeginTime.dwSecond = (uint)startTime.Second;

            //Set the stopping time to search video files
            struVodPara.struEndTime.dwYear = (uint)stopTime.Year;
            struVodPara.struEndTime.dwMonth = (uint)stopTime.Month;
            struVodPara.struEndTime.dwDay = (uint)stopTime.Day;
            struVodPara.struEndTime.dwHour = (uint)stopTime.Hour;
            struVodPara.struEndTime.dwMinute = (uint)stopTime.Minute;
            struVodPara.struEndTime.dwSecond = (uint)stopTime.Second;

            //Playback by time
            m_lPlayHandle = CHCNetSDK.NET_DVR_PlayBackByTime_V40(m_lUserID, ref struVodPara);
            if (m_lPlayHandle < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_PlayBackByTime_V40 failed, error code= " + iLastErr;
                MessageBox.Show(str);
                return;
            }

            uint iOutValue = 0;
            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(m_lPlayHandle, CHCNetSDK.NET_DVR_PLAYSTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_PLAYSTART failed, error code= " + iLastErr; //Playback controlling failed,print error code.
                MessageBox.Show(str);
                return;
            }
            timerPlayback.Interval = 100;
            timerPlayback.Enabled = true;
        }
    }
}
