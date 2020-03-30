using System;
using System.Windows;
using EmailClient.Core;
using EmailClient.ViewModels;

namespace EmailClient.Dialogs
{
    /// <summary>
    /// Interaction logic for ComposeDialog.xaml
    /// </summary>
    public partial class ComposeDialog : Window
    {
        private readonly MainViewModel _mvm;

        /// <summary>
        /// Gets the recipients email address
        /// </summary>
        public string To => toField.Text;

        /// <summary>
        /// Gets the subject of the email
        /// </summary>
        public string Subject => subjectField.Text;

        /// <summary>
        /// Gets the content of the email
        /// </summary>
        public string Message => messageField.Text;

        /// <summary>
        /// Initializes the componenets of the ComposeDialog window
        /// <param name="mvm">Refference to the MainViewModel</param>
        /// </summary>
        public ComposeDialog(MainViewModel mvm)
        {
            _mvm = mvm;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// Initializes the componenets of the ComposeDialog window
        /// This constructor is used by the reply feature of the ViewDialog window.
        /// </summary>
        /// <param name="mvm">Refference to the MainViewModel</param>
        /// <param name="toAddress">Email address of the recipient</param>
        /// <param name="subject">Subject of the email</param>
        public ComposeDialog(MainViewModel mvm, string toAddress, string subject)
        {
            _mvm = mvm;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            toField.Text = toAddress;
            subjectField.Text = "RE: " + subject;
        }

        /// <summary>
        /// Pressing the cancel button interrupts the email composing process
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Sending the composed Email message
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (To.Equals(string.Empty))
                {
                    DialogResult = false;
                }
                else
                {
                    var newEmail = _mvm.CurrentUser.CurrentMailbox.Control.CreateNewEmailToSend(To, Subject, Message);
                    _mvm.CurrentUser.CurrentMailbox.Control.SendEmailAsync(newEmail);
                    DialogResult = true;
                }
            }
            catch (InvalidOperationException)
            {
                // ++ log
                Close();
            }
            
        }
    }
}
