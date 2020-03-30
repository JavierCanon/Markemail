using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyMailBox.Models;
using MyMailBox.Utils;

namespace MyMailBox
{
    /// <summary>
    /// Logique d'interaction pour MailBoxContainer.xaml
    /// </summary>
    /// 
    public partial class MailBoxContainer : UserControl
    {

        private List<MailBox> listMailBox = new List<MailBox>();
        private MailBox currentMailBox = null;

        public MailBoxContainer()
        {
            InitializeComponent();
        }

        public void addMailBox(MailBox newMailBox)
        {
            this.listMailBox.Add(newMailBox);
            settingsMailBoxChoice();
        }

        public void deleteMailBox(Account account)
        {
            MailBox mailBoxToDelete = null;
            foreach (MailBox mailBox in this.listMailBox)
            {
                if (mailBox.getID() == account.getID())
                {
                    mailBoxToDelete = mailBox;
                    break;
                }
            }
            if (mailBoxToDelete != null)
            {
                if (currentMailBox == mailBoxToDelete)
                {
                    currentMailBox = null;
                }
                this.listMailBox.Remove(mailBoxToDelete);
                settingsMailBoxChoice();
            }
        }

        public void setNewListAccount(List<Account> listAccounts)
        {
            if (Properties.Settings.Default.ListAccount != null)
            {
                List<String> listStringAccounts = Properties.Settings.Default.ListAccount.Cast<String>().ToList();
                foreach (Account account in listAccounts)
                {
                    listMailBox.Add(new MailBox(account));
                }
                settingsMailBoxChoice();
            }
        }

        private void settingsMailBoxChoice()
        {
            AccountChoiceComboBox.Items.Clear();
            foreach (MailBox mailBox in this.listMailBox)
            {
                AccountChoiceComboBox.Items.Add(mailBox.getMailBoxName());
            }
            if (currentMailBox == null && this.listMailBox.Count > 0)
            {
                AccountChoiceComboBox.SelectedIndex = 0;
            }
            else if (this.listMailBox.Count > 0)
            {
                AccountChoiceComboBox.SelectedIndex = listMailBox.IndexOf(currentMailBox);
            }
        }

        private void displayMailBox(MailBox mailbox)
        {
            if (currentMailBox != null)
            {
                GridMailBoxContainer.Children.Remove(currentMailBox);
            }
            GridMailBoxContainer.Children.Add(mailbox);
            mailbox.SetValue(Grid.RowProperty, 1);
            currentMailBox = mailbox;
            putLoadingSpinner(mailbox);
            ThreadInvoker.Instance.RunByNewThread(() =>
            {
                mailbox.Show();
                ThreadInvoker.Instance.RunByUiThread(() =>
                {
                    removeLoadingSpinner(mailbox);
                });
            });
            //mailbox.Show();
        }

        private void putLoadingSpinner(MailBox mailbox)
        {
            mailbox.Visibility = Visibility.Collapsed;
            LoadingSpinnerView.Visibility = Visibility.Visible;
        }

        private void removeLoadingSpinner(MailBox mailbox)
        {
            mailbox.Visibility = Visibility.Visible;
            LoadingSpinnerView.Visibility = Visibility.Collapsed;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = 0;
            while (i < listMailBox.Count)
            {
                if (AccountChoiceComboBox.SelectedIndex == i)
                {
                    if (currentMailBox != null && listMailBox[i].getID() == currentMailBox.getID())
                    {
                        return;
                    }
                    displayMailBox(this.listMailBox[i]);
                    return;
                }
                i++;
            }
        }
    }
}
