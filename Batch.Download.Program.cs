using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO.IsolatedStorage;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label2.Text = "等待选择文件";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filepath = "";
            OpenFileDialog opf = new OpenFileDialog();
            if (opf.ShowDialog() == DialogResult.OK)
            filepath = opf.FileName;
            textBox1.Text = opf.FileName;
            label2.Text = "等待读取文件";
        }

        private void button2_Click(object sender, EventArgs e)
        {
//            textBox1.Text = @"C:\Users\Yangit\Desktop\2.txt";
            dataGridView1.Rows.Clear();
            Var.Account.Clear();
            Var.Count = 0;
            if (textBox1.Text == "")
            {
                return;
            }
            StreamReader sr = new StreamReader(textBox1.Text); while (true)
            {
                string str = sr.ReadLine();
                if (str == "\n" || str == "" || str == " " || str == "\t")
                {
                    continue;
                }
                Var.Account.Add(str);
                Var.Count++;
                dataGridView1.Rows.Add(str, str, "未处理");
                if (String.IsNullOrEmpty(str))
                {
                    break;
                }
            }
            sr.Close();
            label2.Text = "账号读取完成";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int Count = 0;
            if (Var.Account.Count <= 1)
            {
                return;
            }
            for(Count=0;Count<Var.Count;Count++)
            {
                Download((string)Var.Account[Count]);
                if (Var.DownloadError != 0)
                {
                    dataGridView1.Rows[Count].Cells[2].Value = "失败(" + Var.DownloadError + ")";
                    label2.Text = "账号:" + (string)Var.Account[Count] + "数据获取失败";
                    WriteError((string)Var.Account[Count], Var.DownloadError);
                    Var.DownloadError = 0;
                    continue;
                }
                else
                {
                    dataGridView1.Rows[Count].Cells[2].Value = "下载成功";

                    label2.Text = "账号：" + (string)Var.Account[Count] + "数据获取成功";
                }
            }
        }

        private void WriteError(string UserName, int Code)
        {
            if (!File.Exists("Error.txt"))
            {
                FileStream WEC = new FileStream("Error.txt", FileMode.Create, FileAccess.Write);
                WEC.Close();
            }
            StreamWriter WE = new StreamWriter(@"Error.txt", true);
            WE.WriteLine("出错用户名：" + UserName + " 错误代码:" + Code);
            WE.Close();
        }

        private void Download(string Name)
        {
            if(Name=="")
            {
                Var.DownloadError = -1;
                return;
            }
            //获得VIEWSTATE和EVENTVALIDATION
            string __VIEWSTATE = "", __EVENTVALIDATION = "";
            string Account = "&Account=" + Name + "&PWD=" + Name + "&cmdok=";
            CookieContainer cookie = new CookieContainer();
            CookieContainer Retcookie = new CookieContainer();
            try
            {
                //请求页面
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.cdjwc.com/jiaowu/Login.aspx");
                request.Method = "GET";
                //request.ProtocolVersion = new Version(1, 1);
                request.KeepAlive = true;
                request.CookieContainer = new CookieContainer();
                request.CookieContainer = cookie;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                while (true)
                {
                    string retString = myStreamReader.ReadLine();
                    if (retString.IndexOf("__VIEWSTATE") >= 0)
                    {
                        //__VIEWSTATE
                        int IDXvalue = retString.IndexOf("value");
                        int IDXFormer = retString.IndexOf("\"", IDXvalue);
                        int LIDXFormer = retString.LastIndexOf("\"");
                        __VIEWSTATE = retString.Substring(IDXFormer + 2, LIDXFormer - IDXFormer - 2);
                        //读行
                        retString = myStreamReader.ReadLine();
                        retString = myStreamReader.ReadLine();
                        //__EVENTVALIDATION
                        IDXvalue = retString.IndexOf("value");
                        IDXFormer = retString.IndexOf("\"", IDXvalue);
                        LIDXFormer = retString.LastIndexOf("\"");
                        __EVENTVALIDATION = retString.Substring(IDXFormer + 2, LIDXFormer - IDXFormer - 2);
                        break;
                    }
                }
                cookie = request.CookieContainer;
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch
            {
                Var.DownloadError = -10;
                return;
            }
            //登陆
            
            string strMsg = "";
            string strUrl = @"http://www.cdjwc.com/jiaowu/Login.aspx";
            string retcode = "__VIEWSTATE=%2F" + __VIEWSTATE + "&__EVENTVALIDATION=%2F" + __EVENTVALIDATION + Account;
            try
            {
                List<Cookie> TEMP = GetAllCookies(cookie);
                byte[] retcodeBuffer = System.Text.Encoding.GetEncoding("gb2312").GetBytes(retcode);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "POST";
                //request.ProtocolVersion = new Version(1, 1);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = retcodeBuffer.Length;
                request.KeepAlive = true;
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(TEMP[0]);
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(retcodeBuffer, 0, retcodeBuffer.Length);
                    requestStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
                {
                    strMsg = reader.ReadToEnd();
                    reader.Close();
                }
                Retcookie = request.CookieContainer;
//                MessageBox.Show(Retcookie.Count.ToString() + "||" + strMsg);
                List<Cookie> TEMPB = GetAllCookies(Retcookie);
                if (Retcookie.Count == 0)
                {
                    Var.DownloadError = -3;
                    return;
                }
                if (Retcookie.Count == 1)
                {
                    Var.DownloadError = -4;
                    return;
                }
            }
            catch 
            {
                Var.DownloadError = -5;
                return;
            }
            try
            {
                label2.Text = "登陆成功";
                string tgUrl = "http://www.cdjwc.com/jiaowu/JWXS/xskp/jwxs_xskp_like.aspx?usermain=" + Name;
                string tgmsg = "";
                //请求页面
                List<Cookie> TEMP = GetAllCookies(Retcookie);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tgUrl);
                request.Method = "GET";
                request.ContentLength = 0;
//                request.ContentType = "text/html;charset=UTF-8";
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(TEMP[0]);
                request.CookieContainer.Add(TEMP[1]);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//                 MessageBox.Show(TEMP[0].ToString() + "||" + TEMP[1].ToString());
                using (StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312")))
                {
                    tgmsg = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                }
                FileStream CreateTEMPFILE = new FileStream("TEMPFILE", FileMode.Create, FileAccess.Write);
                CreateTEMPFILE.Close();
                StreamWriter Result = new StreamWriter("TEMPFILE", true);
                Result.Write(tgmsg);
                Result.Close();
//                process(tgmsg);
                //写入文件              
            }
            catch 
            {
                Var.DownloadError = -6;
                return;
            }
        }
        /*
                private void process(string Data)
                {
                    StreamReader FILE = new StreamReader("TEMPFILE");
                    try
                    {
                        while(true)
                        {
                            string str = FILE.ReadLine();
                            if(str.IndexOf("学生基本信息")>=0)
                            {
                                break;
                            }
                            if(str=="")
                            {
                                Var.DownloadError=-7;
                                return;
                            }
                            str="";
                        }
                        while(true)
                        {
                            string str = FILE.ReadLine();
                            if(str.IndexOf("</head>")>=0)
                            {
                                break;
                            }
                            str="";
                        }
                        while(true)//<=========================================================================院系
                        {
                            string str = FILE.ReadLine();
                            string CollegeName="";
                            if(str.IndexOf("院系")>=0)
                            {
                                int Count_Ct=0,Count_CN=0;
                                while(str[Count_Ct]!='>')
                                {
                                    Count_Ct++;
                                }
                                Count_Ct++;
                                while(str[Count_Ct]!='<')
                                {
                                    Var.CollegeName[Count_CN]=str[Count_Ct];
                                    Count_Ct++;
                                    Count_CN++;
                    }
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================专业
            {
                if(strstr(Content,"专业")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0;
                    while(Content[Count_Ct]!='>')
                    {
                        Count_Ct++;
                    }
                    Count_Ct++;
                    while(Content[Count_Ct]!='<')
                    {
                        Inf[0].Major[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================学制
            {
                if(strstr(Content,"学制")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0; 
                    char TEMPCT[30]={""};
                    while(Content[Count_Ct]!='>')
                    {
                        Count_Ct++;
                    }
                    Count_Ct++;
                    while(Content[Count_Ct]!='<')
                    {
                        TEMPCT[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    switch (atoi(TEMPCT)) 
                    {
                        case 3:strcpy(Inf[0].CollegeType,"专科");break;
                        case 4:strcpy(Inf[0].CollegeType,"本科"); break;
                        default :strcpy(Inf[0].CollegeType,"其他"); break;
                    }
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================班级 
            {
                if(strstr(Content,"班级")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0;
                    while(Content[Count_Ct]!='>')
                    {
                        Count_Ct++;
                    }
                    Count_Ct++;
                    while(Content[Count_Ct]!='<')
                    {
                        Inf[0].Class[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================学号
            {
                if(strstr(Content,"学号")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0;
                    while(Content[Count_Ct]!='>')
                    {
                        Count_Ct++;
                    }
                    Count_Ct++;
                    while(Content[Count_Ct]!='<')
                    {
                        Inf[0].SchoolID[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================姓名
            {
                if(strstr(Content,"tbxsxm")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0;
                    while(Content[Count_Ct]!='v')
                    {
                        Count_Ct++;
                    }
                    Count_Ct=Count_Ct+7;
                    while(Content[Count_Ct]!='"')
                    {
                        Inf[0].StudentName[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================性别
            {
                if(strstr(Content,"selected"))
                {
                    if(strstr(Content,"1"))
                    {
                        strcpy(TEMP,"男");
                    }
                    if(strstr(Content,"2"))
                    {
                        strcpy(TEMP,"女");
                    }
                    strcpy(Inf[0].Sex,TEMP);
                    memset(TEMP,0,sizeof(TEMP));
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================出生日期
            {
                if(strstr(Content,"出生日期"))
                {
                    break;
                }
            }
            while(fgets(Content,2000,Input))
            {
                if(strstr(Content,"tbcsrq")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0;
                    while(Content[Count_Ct]!='v')
                    {
                        Count_Ct++;
                    }
                    Count_Ct=Count_Ct+7;
                    while(Content[Count_Ct]!='"')
                    {
                        TEMP[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    strcpy(Inf[0].StudentBirth,TEMP);
                    memset(TEMP,0,sizeof(TEMP));
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================本人电话
            {
                if(strstr(Content,"本人电话"))
                {
                    break;
                }
            }
            while(fgets(Content,2000,Input))
            {
                if(strstr(Content,"tbbrlxdh")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0;
                    while(Content[Count_Ct]!='v')
                    {
                        Count_Ct++;
                    }
                    Count_Ct=Count_Ct+7;
                    while(Content[Count_Ct]!='"')
                    {
                        Inf[0].PhoneNum[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================高中 
            {
                if(strstr(Content,"入学前工作单位"))
                {
                    break;
                }
            }
            while(fgets(Content,2000,Input))
            {
                if(strstr(Content,"tbrxqgzdw")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0;
                    while(Content[Count_Ct]!='v')
                    {
                        Count_Ct++;
                    }
                    Count_Ct=Count_Ct+7;
                    while(Content[Count_Ct]!='"')
                    {
                        Inf[0].HighschoolName[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================家庭住址 
            {
                if(strstr(Content,"家庭现住址"))
                {
                    break;
                }
            }
            while(fgets(Content,2000,Input))
            {
                if(strstr(Content,"tbjtxzdz")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0;
                    while(Content[Count_Ct]!='v')
                    {
                        Count_Ct++;
                    }
                    Count_Ct=Count_Ct+7;
                    while(Content[Count_Ct]!='"')
                    {
                        Var.HomeAddress[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    break;
                }
                memset(Content,0,2000);
            }
            while(fgets(Content,2000,Input))//<=========================================================================身份证编号 
            {
                if(strstr(Content,"身份证编号"))
                {
                    break;
                }
            }
            while(fgets(Content,2000,Input))
            {
                if(strstr(Content,"tbsfzh")!=NULL)
                {
                    int Count_Ct=0,Count_CN=0;
                    while(Content[Count_Ct]!='v')
                    {
                        Count_Ct++;
                    }
                    Count_Ct=Count_Ct+7;
                    while(Content[Count_Ct]!='"')
                    {
                        Inf[0].PersonalID[Count_CN]=Content[Count_Ct];
                        Count_Ct++;
                        Count_CN++;
                    }
                    break;
                }


                    }


                    string filename = Name + ".txt";
                    StreamWriter Result = new StreamWriter(filename, true);
                    Result.Write();
                    Result.Close();
                }

                private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
                {

                }*/

        public static List<Cookie> GetAllCookies(CookieContainer cc)
                {
                    List<Cookie> lstCookies = new List<Cookie>();

                    Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                        System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

                    foreach (object pathList in table.Values)
                    {
                        SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                            | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                        foreach (CookieCollection colCookies in lstCookieCol.Values)
                            foreach (Cookie c in colCookies) lstCookies.Add(c);
                    }
                    return lstCookies;
                }
    }
    public class Var
    {
        //普通全局变量
        public static int Count = 0;
        public static ArrayList Account = new ArrayList();
        public static int DownloadError = 0;
        public static string DownloadFile = "";
        //分离变量
        public static string CollegeName="";
        public static string Major="";
        public static string CollegeType="";
        public static string Class="";
        public static string SchoolID="";
        public static string StudentName="";
        public static string Sex="";
        public static string StudentBirth="";
        public static string PhoneNum="";
        public static string HighschoolName="";
        public static string HomeAddress="";
        public static string PersonalID="";
    } 
}