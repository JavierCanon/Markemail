using System.Windows;
using System.Windows.Controls;
using EmailClient.Core;
using EmailClient.Core.Model;
using EmailClient.ViewModels;
using EmailClient.Dialogs;
using System.Collections.Generic;
using MailKit;
using EmailClient.Core.Service;
using System;
using System.Diagnostics;
using System.Windows.Media;

namespace EmailClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _mvm;
        private Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Constructor to initialize the UI thread.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// Actions after initializing the main window
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _mvm = new MainViewModel();
            if (_mvm.Users.Count == 0)
            {
                if (!NewUser()) Environment.Exit(0);
                SelectUser(_mvm.Users[0]);
            }
            else
            {
                SwitchUser();
            }
        }

        /// <summary>
        /// When an IMAP connection begins ConnectionStart event fires
        /// This eventhandler starts the timer to measure the connection time
        /// </summary>
        public void ConnectionStartEventHandler()
        {
            sw.Start();
            dwnldStatus.Foreground = Brushes.Green;
            dwnldStatus.Text = "Connecting...";
        }

        /// <summary>
        /// When an IMAP connection ends ConnectionFinish event fires
        /// This eventhandler stops the timer and changes the dwnldStatus indicator TextBlock,
        /// UI element's value to the Ellapsed time in milliseconds.
        /// </summary>
        public void ConnectionFinishEventHandler()
        {
            sw.Stop();
            dwnldStatus.Foreground = Brushes.Black;
            dwnldStatus.Text = $"{sw.ElapsedMilliseconds}ms";
            sw.Reset();
        }

        /// <summary>
        /// Event handler of the notification event. e.g: new email(s) arrived.
        /// </summary>
        /// <param name="message">Message to show in a blocking message box.</param>
        public void NotificationEventHandler(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// It starts the UI and backend flow of selecting the current (active) user.
        /// </summary>
        /// <param name="selectedUser">Selected user</param>
        private void SelectUser(User selectedUser)
        {
            if (_mvm.CurrentUser != null && _mvm.CurrentUser.CurrentMailbox != null)
            {
                _mvm.CurrentUser.CurrentMailbox.Control.StopGettingEmails();
            }
            if (selectedUser != null)
            {
                _mvm.CurrentUser = selectedUser;
                mailBoxCombo.ItemsSource = _mvm.CurrentUser.Mailboxes;
                listBox.ItemsSource = null;
                dwnldStatus.Text = "IDLE";
                userNameText.Text = _mvm.CurrentUser.Name;
                if (_mvm.CurrentUser.Mailboxes.Count == 0)
                {
                    NewMailboxBtn_Click(null, null);
                }
            }
        }

        public bool ConfirmDialog(string message)
        {
            var result = MessageBox.Show(message, "Warning!", MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes ? true : false;
        }

        /// <summary>
        /// It starts the UI and backend flow of selecting the current (active) mailbox of the current user
        /// </summary>
        /// <param name="selected">Selected mailbox</param>
        public void SelectMailbox(Mailbox selected)
        {
            _mvm.CurrentUser.CurrentMailbox = selected;
            _mvm.CurrentUser.CurrentMailbox.Control.StartGettingEmails();
            listBox.ItemsSource = _mvm.CurrentUser.CurrentMailbox.Emails;
            _mvm.CurrentUser.CurrentMailbox.Control.connectionStart += ConnectionStartEventHandler;
            _mvm.CurrentUser.CurrentMailbox.Control.connectionFinish += ConnectionFinishEventHandler;
            _mvm.CurrentUser.CurrentMailbox.Control.notification += NotificationEventHandler;
            dwnldStatus.Foreground = Brushes.Green;
            dwnldStatus.Text = "Connecting...";
        }

        /// <summary>
        /// Eventhandler on changing the current (active) mailbox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void MailBoxCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var selected = (Mailbox)comboBox.SelectedItem;
            if (selected == null) return;
            _mvm.CurrentUser.CurrentMailbox.Control.StopGettingEmails();
            SelectMailbox(selected);
        }

        /// <summary>
        /// Shows up a dialog window to create a user
        /// </summary>
        private bool NewUser()
        {
            var dlg = new NewUserDialog();
            if (dlg.ShowDialog() == true)
            {
                _mvm.AddUser(dlg.UserName, dlg.BirthDate);
                return true;
            }
            return false;
        }


        /// <summary>
        /// If the the current user have zero mailboxes it is sets the mailbox ComboBox to disabled
        /// else it sets to enabled
        /// </summary>
        private void SetMailboxComboUsage()
        {

            if (_mvm.CurrentUser == null  || _mvm.CurrentUser.Mailboxes.Count == 0)
            {
                mailBoxCombo.IsEnabled = false;
            }
            else
            {
                mailBoxCombo.IsEnabled = true;
            }
        }

        /// <summary>
        /// Shows up a dialog window to select a user
        /// </summary>
        private void SwitchUser()
        {
            if (_mvm.Users.Count != 0)
            {
                var dlg = new SwitchUserDialog(_mvm);
                if (dlg.ShowDialog() == true)
                {
                    SelectUser(dlg.SelectedUser);
                    SetMailboxComboUsage();
                }
            }
        }

        /// <summary>
        /// Starts the UI and backend process flow of deleting a user.
        /// </summary>
        public void DeleteUser()
        {
            if (_mvm.CurrentUser.CurrentMailbox != null)
            {
                _mvm.CurrentUser.CurrentMailbox.Control.StopGettingEmails();
            }
            _mvm.DeleteUser(_mvm.CurrentUser);
            _mvm.CurrentUser = null;
            mailBoxCombo.ItemsSource = null;
            listBox.ItemsSource = null;
            dwnldStatus.Text = "IDLE";
            userNameText.Text = "";
            SwitchUser();
        }

        /// <summary>
        /// Clicking on the switch user menu item shows up the the switch user dialog window
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void SwitchUserBtn_Click(object sender, RoutedEventArgs e)
        {
            SwitchUser();
        }

        /// <summary>
        /// Eventhandler on clicking on the new user menu item. 
        /// Ishows up the the new user dialog window
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void NewUserBtn_Click(object sender, RoutedEventArgs e)
        {
            NewUser();
            SwitchUser();
        }

        /// <summary>
        /// Eventhandler on clicking on the delete user menu item.
        /// The current user will be deleted
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void DeletUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_mvm.CurrentUser != null)
            {
                var userName = _mvm.CurrentUser.Name;
                var message = $"Do you really want to delete the current user: {userName}?";
                if (ConfirmDialog(message))
                {
                    DeleteUser();
                }
            }
        }

        /// <summary>
        /// Eventhandler on clicking on the new mailbox menu item.
        /// It shows up the new mailbox dialog window.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void NewMailboxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_mvm.CurrentUser != null)
            {
                var dlg = new NewMailBoxDialog(_mvm);
                if (dlg.ShowDialog() == true)
                {
                    MessageBox.Show("New mailbox has been created!");
                    SetMailboxComboUsage();
                }
            }
        }

        /// <summary>
        /// On closing event the aplication serializes the users.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var user in _mvm.Users)
            {
                User.Serialize(user, BackupHandler.root);
            }
        }

        /// <summary>
        /// Clicking on delete button event handler.
        /// It collects the UniqueId objects of the selected emails, and sends them for async deletion.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = listBox.SelectedItems;
            if (selectedItems.Count == 0) return;
            var message = $"Do you really want to delete the selected {selectedItems.Count} email(s) ?";
            if (ConfirmDialog(message))
            {
                var uids = new List<UniqueId>();
                foreach (var item in selectedItems)
                {
                    var email = (Email)item;
                    uids.Add(new UniqueId(email.ID));
                }
                _mvm.CurrentUser.CurrentMailbox.Control.DeleteEmailsAsync(uids);
            }
        }

        /// <summary>
        /// Click on the delete mailbox menu item event handler. 
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void DeleteMailboxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_mvm.CurrentUser != null && _mvm.CurrentUser.CurrentMailbox != null)
            {
                var mailboxName = _mvm.CurrentUser.CurrentMailbox.Properties.Name;
                var message = $"Do you really want to delete the current mailbox {mailboxName} ?";
                if (ConfirmDialog(message))
                {
                    _mvm.CurrentUser.DeleteMailbox();
                    dwnldStatus.Text = "IDLE";
                    listBox.ItemsSource = null;
                    SetMailboxComboUsage();
                }
            }
        }

        /// <summary>
        /// Shows up the ComposeDialog window to start creating a new email
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void ComposeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_mvm.CurrentUser != null && _mvm.CurrentUser.CurrentMailbox != null)
            {
                var dlg = new ComposeDialog(_mvm);
                if (dlg.ShowDialog() == true)
                {
                    MessageBox.Show("Email has been sent!");
                }
            }
        }

        /// <summary>
        /// Double click on a ListBox item will show the requested email's content.
        /// </summary>
        /// <param name="sender">sneder</param>
        /// <param name="e">event</param>
        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var src = (TextBlock)e.OriginalSource;
                var email = (Email)src.DataContext;
                var listBoxItem =  listBox.ItemContainerGenerator.ContainerFromItem(email) as ListBoxItem;
                listBoxItem.Background = Brushes.White;
                email.Seen = true;
                var dlg = new ViewEmailDialog(_mvm, email);
                dlg.ShowDialog();
            }
            catch (InvalidCastException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
