﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using FacebookPages.Code.Buttons.Interfaces;
using FacebookPages.Code.Pages.Data;
using FacebookPages.Code.Pages.Data.Post;
using FacebookWrapperEnhancements.Code.Collection;
using FacebookWrapperEnhancements.Code.Collection.Filter;
using FacebookWrapperEnhancements.Code.EnhancedObjects;
using static FacebookPages.Code.Pages.Data.WallPageData;

namespace FacebookPages.Code.Pages
{
    public partial class WallPage : BasePage
    {
        public override Color BackColor { get; set; }
        public WallPageData PageData { get; set; }
        public static readonly object sr_PostDataLock = new object();

        public WallPage()
        {
            InitializeComponent();
        }

        public void SetAsHomePage()
        {
            m_ExitButton.Text = @"Logout";
            m_ExitButton.PageChoice = ePageChoice.Logout;
        }

        protected virtual void PageSwitchButton_Click(object i_Sender, EventArgs i_EventArgs)
        {
            OnChangePage(i_Sender, i_EventArgs);
        }

        protected override void OnLoad(EventArgs i_EventArgs)
        {
            base.OnLoad(i_EventArgs);

            new Thread(fetchPostsDataInBackground).Start();
            new Thread(fetchNonPostsDataInBackground).Start();
        }

        private void fetchNonPostsDataInBackground()
        {
            try
            {
                PageData.FetchNonPostsData();

                this.Invoke(
                    (MethodInvoker)updatePageWithNonPostData);
            }
            catch (System.InvalidOperationException invalidOperation)
            {
                MessageBox.Show(invalidOperation.Message, @"Error");
            }
        }

        private void fetchPostsDataInBackground()
        {
            lock(sr_PostDataLock)
            {
                try
                {
                    List<EnhancedPost> postDataToLoad = PageData.GetPosts().CollectionData;

                    this.Invoke((MethodInvoker)delegate { updatePageWithPostData(postDataToLoad); });
                }
                catch(System.InvalidOperationException invalidOperation)
                {
                    MessageBox.Show(invalidOperation.Message, @"Error");
                }
            }
        }

        private void updatePageWithPostData(List<EnhancedPost> i_Data)
        {
            m_PostViewButton.LoadInfoListBox.DataSource = i_Data;
            m_PostViewButton.Refresh();
        }

        private void updatePageWithNonPostData()
        {
            wallPageDataBindingSource.DataSource = PageData;
            
            if (PageData?.ProfilePicUrl != null)
            {
                profilePicture.LoadAsync(PageData.ProfilePicUrl);
            }

            if (PageData?.CoverPicUrl != null)
            {
                coverPicture.LoadAsync(PageData.CoverPicUrl);
                coverPicture.SizeMode = PictureBoxSizeMode.CenterImage;
            }

            m_FillNumberOfFriends.Text = PageData?.Friends.Count.ToString();
        }

        private void m_PostViewButton_ChangeConnectionRequest(object i_Sender, EventArgs i_EventArgs)
        {
            string connection = (string)(i_Sender as System.Windows.Forms.ComboBox)?.SelectedItem;
            if(connection != null)
            {
                PageData.CurrentConnection =
                    (eConnectionOptions)Enum.Parse(typeof(eConnectionOptions), connection);

                new Thread(fetchPostsDataInBackground).Start();
            }
        }

        private void m_PostViewButton_MorePostsRequest(object i_Sender, EventArgs i_EventArgs)
        {
            new Thread(fetchMorePostsDataInBackground).Start();
        }

        private void fetchMorePostsDataInBackground()
        {
            lock(sr_PostDataLock)
            {
                try
                {
                    PageData.GetPosts().FetchNewPage();

                    this.Invoke(new Action(() => m_PostViewButton.Refresh()));
                }
                catch(System.InvalidOperationException invalidOperation)
                {
                    MessageBox.Show(invalidOperation.Message, @"Error");
                }
            }
        }

        private void m_PostViewButton_FilterRequest(object i_Sender, EventArgs i_EventArgs)
        {
            (i_Sender as IHasDataInfo).ReceivedInfo = PageData.CurrentFilterData;
            OnReceivedInfo(i_Sender, i_EventArgs);
            m_PostViewButton.Refresh();
        }

        private void m_PostViewButton_LoadAllPosts(object i_Sender, EventArgs i_EventArgs)
        {
            new Thread(fetchAllPostsDataInBackground).Start();
        }

        private void fetchAllPostsDataInBackground()
        {
            lock (sr_PostDataLock)
            {
                try
                {
                    PageData.GetPosts().FetchAllPages();

                    this.Invoke(new Action(() => m_PostViewButton.Refresh()));
                }
                catch (System.InvalidOperationException invalidOperation)
                {
                    MessageBox.Show(invalidOperation.Message, @"Error");
                }
            }
        }

        private void m_PostViewButton_PostSelected(object i_Sender, EventArgs i_EventArgs)
        {
            OnReceivedInfo(i_Sender, i_EventArgs);
        }

        private void m_PostViewButton_PostAnalyticRequest(object i_Sender, EventArgs i_EventArgs)
        {
            (i_Sender as IHasDataInfo).ReceivedInfo = PageData.GetPosts();
            OnReceivedInfo(i_Sender, i_EventArgs);
        }

        private void m_ViewFriendButton_Click(object i_Sender, EventArgs i_EventArgs)
        {
            (i_Sender as IHasDataInfo).ReceivedInfo = m_ChooseFriend.ReceivedInfo;
            (i_Sender as IHasDataInfo).InfoChoice = eInfoChoice.Friend;

            PageSwitchButton_Click(i_Sender, i_EventArgs);
        }

        public void LoadFilterData(FilterData i_FilterDate)
        {
            PageData.CurrentFilterData = i_FilterDate;

            new Thread(fetchPostsDataInBackground).Start();
        }
    }
}
