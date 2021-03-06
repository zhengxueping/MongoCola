﻿using System;
using System.Windows.Forms;

namespace Common.UI
{
    public partial class frmEasyMessage : Form
    {
        public frmEasyMessage()
        {
            InitializeComponent();
            cmdOK.BackColor = MyMessageBox.SuccessColor;
        }

        /// <summary>
        ///     Set Text
        /// </summary>
        internal void SetText(string OK)
        {
            cmdOK.Text = OK;
        }

        /// <summary>
        ///     SetMessage
        /// </summary>
        /// <param name="Message"></param>
        internal void SetMessage(string Message)
        {
            lblMessage.Text = Message;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}