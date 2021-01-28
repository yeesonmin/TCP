using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TCP_Client_CS_
{
    public partial class Form1 : Form
    {
        private Socket socket;
        private Thread receiveThread;

        int SEVER_PORT = 4000;
        string SEVER_IP = "127.0.0.1";


        [StructLayout(LayoutKind.Sequential)]
        struct Data
        {
            [MarshalAs(UnmanagedType.I4)]
            public int X;

            [MarshalAs(UnmanagedType.I4)]
            public int Y;
        }

        Data GetData = new Data();


        public Form1()
        {
            InitializeComponent();

        }

        // 바이트를 구조체로 변환
        public static void BytesToStructure(byte[] bValue, ref object obj, Type t)
        {
            int size = Marshal.SizeOf(t);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.Copy(bValue, 0, buffer, size);
            obj = Marshal.PtrToStructure(buffer, t);
            Marshal.FreeHGlobal(buffer);
        }

        //Receive 스레드
        private void Receive()
        {
            while (true)
            {
                //데이터 수신
                byte[] recevieBuffer = new byte[1024];
                //int length = socket.Receive(recevieBuffer, recevieBuffer.Length, SocketFlags.None);
                socket.Receive(recevieBuffer, recevieBuffer.Length, SocketFlags.None);

                object data = new object();
                BytesToStructure(recevieBuffer, ref data, typeof(Data));
                GetData = (Data)data;

                //string msg = Encoding.UTF8.GetString(recevieBuffer, 0, length);
                lbl_X.Text = GetData.X.ToString();
                lbl_Y.Text = GetData.Y.ToString();
                txb_history.AppendText("x : " + GetData.X.ToString() + " y : " + GetData.Y.ToString() + "\r\n");
                
            }
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {


            IPAddress ipaddress = IPAddress.Parse(SEVER_IP);
            IPEndPoint endPoint = new IPEndPoint(ipaddress, SEVER_PORT);

            //연결하기
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);
            txb_history.Text = "연결됨\r\n";

            //스레드 실행
            receiveThread = new Thread(new ThreadStart(Receive));
            receiveThread.IsBackground = true;
            receiveThread.Start();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            socket.Close();
        }
    }
}
