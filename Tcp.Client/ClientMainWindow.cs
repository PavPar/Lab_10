using System;
using System.Windows.Forms;
using SomeProject.Library.Client;
using SomeProject.Library;
using System.IO;

namespace SomeProject.TcpClient
{
    public partial class ClientMainWindow : Form
    {
        Client client;
        public ClientMainWindow()
        {
            InitializeComponent();
            
        }

        private void OnMsgBtnClick(object sender, EventArgs e)
        {
            client = new Client();
            Result res = client.SendMessageToServer(textBox.Text).Result;
            if (res == Result.OK)
            {
                textBox.Text = "";
                labelRes.Text = "Message was sent succefully!";
            }
            else
            {
                labelRes.Text = "Cannot send the message to the server.";
            }
            timer.Interval = 2000;
            timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            labelRes.Text = "";
            timer.Stop();
        }

        private void btn_sendFile_Click(object sender, EventArgs e)
        {
            client = new Client();
            string fileContent;
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                string filePath = openFileDialog1.FileName;

              
                Result res = client.SendFileToServer(filePath).Result;
                if (res == Result.OK)
                {
                    textBox.Text = "";
                    labelRes.Text = "Message was sent succefully!";
                }
                else
                {
                    labelRes.Text = "Cannot send the message to the server.";
                }
                timer.Interval = 2000;
                timer.Start();
            }
        }

        private void ClientMainWindow_Load(object sender, EventArgs e)
        {
            client = new Client();
            Result res = client.SendAckToServer("Hi i am new User!").Result;
        }

        private void ClientMainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.CloseClient();
        }
    }
}
