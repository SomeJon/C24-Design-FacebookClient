﻿using System;
using System.Drawing;
using System.Windows.Forms;
using FacebookPages.Code.Buttons;

namespace FacebookPages.Pages
{
    public partial class LoginSettingPage : BasePage
    {
        public override Color BackColor {  get; set; }

        public LoginSettingPage()
        {
            InitializeComponent();
            appIdComboBox.Items.AddRange(new string[] {
                "867142571975316",
                "696056928008003",
                "1450160541956417"});

            appIdComboBox.SelectedIndex = 0;
        }

        private void addAppIdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            OnChangePage(sender, e);
        }

        private void addIdButton_Click(object sender, EventArgs e)
        {
            if (addAppIdTextBox.Text.Length > 0) {
                (sender as LoadInfoButton).ReceivedInfo = addAppIdTextBox.Text;
                appIdComboBox.Items.Add(addAppIdTextBox.Text);
                appIdComboBox.SelectedIndex = appIdComboBox.Items.Count - 1;
                OnRecivedInfo(sender, e);
            } else
            {
                MessageBox.Show("You need to first input an id in the Text Box!");
            }
        }

        private void pictureBoxReturn_Click(object sender, EventArgs e)
        {
           OnChangePage(sender, e);        
        }
    }
}
