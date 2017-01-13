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

namespace RVIP_41
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint eplLocal, epRemote;
        List<string> messageClient;
        List<string> messageServer;
        public Form1()
        {
            //Create Socket with parametres(IP,type,UDP)
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            InitializeComponent();
            messageClient = new List<string>();
            messageServer = new List<string>();
            messageClient.Add("Image Send on Server");
            messageServer.Add("Processing request");
            messageServer.Add("Resource allocation");
            messageServer.Add("Image processing");
            messageServer.Add("Image Update");
        }
      

        //CallBack 
        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] receiveData = new byte[1464];
                    receiveData = (byte[])aResult.AsyncState;

                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receiveMessage = eEncoding.GetString(receiveData);
                    
                    listMessage.Items.Add("Server: " + receiveMessage);
                }
                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

       //Connected Client with Server
        private void Startbtn_Click(object sender, EventArgs e)
        {
            try
            {
                //Client
                eplLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(textLocalPort.Text));
                sck.Bind(eplLocal);
                //Server
                epRemote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(textFriendsPort.Text));
                sck.Connect(epRemote);
                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

                Startbtn.Text = "Connected";
                Startbtn.Enabled = false;
                Sendbtn.Enabled = true;
                textMessage.Focus();
                SendMessage();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void listMessage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listMessage_Click(object sender, EventArgs e)
        {
            
            try
            {
                count_old = listMessage.Items.Count;
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                string text = "";

                foreach (string mCl in messageClient)
                {
                    text = mCl;
                    msg = enc.GetBytes(mCl);
                    sck.Send(msg);
                    listMessage.Items.Add("You: " + text);

                }
                if (listMessage.Items.Count > count_old)
                {
                  
                    foreach (string mS in messageServer)
                    {
                        text = mS;
                        msg = enc.GetBytes(mS);
                        sck.Send(msg);
                        listMessage.Items.Add("You2: " + text);

                    }

                }

                textMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        int count_old;

        Random r;
        //SendMessage
        void SendMessage() {
            try
            {
                r = new Random();
                int threadcount = r.Next(5, 11);
                count_old = listMessage.Items.Count;
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                string text = "";
                for (int i = 0; i < threadcount; i++)
                {

                    foreach (string mCl in messageClient)
                    {
                        text = mCl;
                        msg = enc.GetBytes(mCl);
                        sck.Send(msg);
                        listMessage.Items.Add("Client: " + i.ToString() + " " + text);

                    }

                }
                for (int j = 0; j < threadcount; j++)
                {
                    foreach (string mS in messageServer)
                    {
                        text = mS;
                        msg = enc.GetBytes(mS);
                        sck.Send(msg);
                        listMessage.Items.Add("Server: "+ j.ToString()+" " + text);

                    }
                }
                textMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
