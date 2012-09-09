using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChokaBhara_Server
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }
        private void Room_1_SelectedIndexChanged(object sender, EventArgs e)
        {/*
            int RoomID = Room_1.SelectedIndex;
            bool Room_1Flag = false;
            if (-1 != RoomID)
            {
                if (Room_1.GetItemChecked(RoomID))
                {

                    List<string> _items = new List<string>();
                    string Roomname = Room_1.Items[RoomID].ToString();
                    string[] rooms = Roomname.Split(' ');
                    RoomID = (Convert.ToInt32(rooms[1])) - 1;
                    _items.Add("Room ID:" + RoomKey[RoomID].ToString());
                    foreach (var pair in SocketKey)
                    {
                        if (pair.Value == RoomID)
                            if (null != pair.Key)
                            {
                                Room_1Flag = true;
                                _items.Add(pair.Key.RemoteEndPoint.ToString());

                            }
                    }
                    if (Room_1Flag)
                    {
                        Rooms.DataSource = _items;
                    }
                    else
                    {
                        Room_1.Items.RemoveAt(Room_1.SelectedIndex);
                    }
                }
                else
                {
                    Rooms.DataSource = null;
                }
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {/*
            bool Flag = false;
            Room_1.Items.Clear();
            for (int i = 0; i < RoomIndex; i++)
            {
                foreach (var pair in SocketKey)
                {
                    if (pair.Value == i)
                        if (null != pair.Key)
                        {
                            Flag = true;
                        }
                }
                if (Flag)
                {
                    Room_1.Items.Add("Room " + (i + 1));
                    Flag = false;
                }
            }*/
        }
    }
}
