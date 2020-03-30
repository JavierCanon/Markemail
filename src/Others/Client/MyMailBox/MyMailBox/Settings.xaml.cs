using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace MyMailBox
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        private List<Account> listAccounts;
        private MainWindow mainWindow = null;
        private Account currentAccount = null;
        private String settingsSeparateValue = ",";

        public Settings()
        {
            this.InitializeComponent();
        }

        public void setMainWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void setSettingsSeparateValue(String value)
        {
            this.settingsSeparateValue = value;
        }

        public void setNewListAccount(List<Account> listAccounts)
        {
            this.listAccounts = listAccounts;
            settingsAccountChoice();
        }

        private void AddMailAccountClick(object sender, RoutedEventArgs e)
        {
            AddMailAccount addMailAccount = new AddMailAccount(this);
            addMailAccount.Show();
        }

        public void saveNewMailAccount(Account newAccount)
        {
            if (listAccounts == null)
            {
                listAccounts = new List<Account>();
            }
            listAccounts.Add(newAccount);
            addAccountIntoSettings(newAccount);
            preventNewAccountCreated(newAccount);
            settingsAccountChoice();
        }

        public void updateAccountFinished(Account account)
        {
            int index = listAccounts.IndexOf(currentAccount);
            listAccounts[index] = account;
            updateAccountFromSettings(account);
            deleteAccountFromSettings(currentAccount);
            displayAccount(account);
        }

        private void preventAccountDelete(Account account)
        {
            if (mainWindow != null)
            {
                if (MessageBox.Show("Tu veux vraiment supprimer ce compte (" + account.getEmail() + ") ?", "Suppression du compte email", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    mainWindow.deleteAccount(account);
                    deleteAccountFromSettings(account);
                    listAccounts.Remove(account);
                    currentAccount = null;
                    settingsAccountChoice();
                }
            }
        }

        private void preventNewAccountCreated(Account account)
        {
            if (mainWindow != null)
            {
                mainWindow.addNewAccount(account);
            }
        }

        private void updateAccountFromSettings(Account account)
        {
            StringCollection listStringAccount = Properties.Settings.Default.ListAccount;
            System.Diagnostics.Debug.WriteLine("Info : Account to update " + account.getID());
            int i = 0;
            foreach (String accountString in listStringAccount)
            {
                String substring = accountString.Substring(0, accountString.IndexOf(settingsSeparateValue) - 1);
                System.Diagnostics.Debug.WriteLine("Info : Search account... " + substring);
                if (accountString.Contains(account.getID() + settingsSeparateValue + ""))
                {
                    System.Diagnostics.Debug.WriteLine("Info : Account to update found");
                    Properties.Settings.Default.ListAccount.Insert(i + 1, getAccountString(account));
                    Properties.Settings.Default.Save();
                    return;
                }
                i++;
            }
            System.Diagnostics.Debug.WriteLine("Warning : Account to update not found");
        }

        private void deleteAccountFromSettings(Account account)
        {
            StringCollection listStringAccount = Properties.Settings.Default.ListAccount;
            System.Diagnostics.Debug.WriteLine("Info : Account to delete " + account.getID());
            int i = 0;
            foreach (String accountString in listStringAccount)
            {
                String substring = accountString.Substring(0, accountString.IndexOf(settingsSeparateValue) - 1);
                System.Diagnostics.Debug.WriteLine("Info : Search account... " + substring);
                if (accountString.Contains(account.getID() + settingsSeparateValue + ""))
                {
                    System.Diagnostics.Debug.WriteLine("Info : Account to delete found");
                    Properties.Settings.Default.ListAccount.RemoveAt(i);
                    Properties.Settings.Default.Save();
                    return;
                }
                i++;
            }
            System.Diagnostics.Debug.WriteLine("Warning : Account to delete not found");
        }

        private String getAccountString(Account account)
        {
            /*
             * ID
             * FULLNAME
             * EMAIL
             * USESSL
             * PORT
             * IDENTITY
             * MAILSERVICE
             * SERVER
             * SIGNATURE
             * REMEMBER PASSWORD
             * if rememberPassword PASSWORD
             * */
            String accountProperties = String.Empty;
            accountProperties += account.getID() + "";
            accountProperties += settingsSeparateValue + account.fullName;
            accountProperties += settingsSeparateValue + account.getEmail();
            accountProperties += settingsSeparateValue + account.getUseSSL().ToString();
            accountProperties += settingsSeparateValue + account.getPort() + "";
            accountProperties += settingsSeparateValue + account.getIdentity();
            accountProperties += settingsSeparateValue + account.getMailService();
            accountProperties += settingsSeparateValue + account.getServer();
            accountProperties += settingsSeparateValue + account.signature;
            accountProperties += settingsSeparateValue + account.rememberPassword.ToString();
            if (account.rememberPassword)
            {
                accountProperties += settingsSeparateValue + account.getPassword();
            }
            return accountProperties;
        }

        private void addAccountIntoSettings(Account account)
        {

            String accountString = getAccountString(account);
            if (Properties.Settings.Default.ListAccount == null)
            {
                Properties.Settings.Default.ListAccount = new StringCollection();
            }
            Properties.Settings.Default.ListAccount.Add(accountString);
            Properties.Settings.Default.Save();
        }

        private void displayAccount(Account account)
        {
            currentAccount = account;
            NameAccountBlock.Text = account.fullName;
            EmailAccountBlock.Text = account.getEmail();
            ServerAccountBlock.Text = account.getServer();
            PortAccountBlock.Text = account.getPort() + "";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = 0;
            while (i < listAccounts.Count)
            {
                if (comboBoxListAccount.SelectedIndex == i)
                {
                    if (currentAccount != null && listAccounts[i].getID() == currentAccount.getID())
                    {
                        return;
                    }
                    displayAccount(this.listAccounts[i]);
                    return;
                }
                i++;
            }
        }

        private void settingsAccountChoice()
        {
            comboBoxListAccount.Items.Clear();
            if (this.listAccounts == null)
            {
                return;
            }
            foreach (Account account in this.listAccounts)
            {
                comboBoxListAccount.Items.Add(account.getEmail());
            }
            if (currentAccount == null && this.listAccounts.Count > 0)
            {
                comboBoxListAccount.SelectedIndex = 0;
            }
            else if (this.listAccounts.Count > 0)
            {
                comboBoxListAccount.SelectedIndex = listAccounts.IndexOf(currentAccount);
            }
        }

        private void UpdateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentAccount != null)
            {
                AddMailAccount addMailAccount = new AddMailAccount(this, currentAccount);
                addMailAccount.Show();
            }
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentAccount != null)
            {
                preventAccountDelete(currentAccount);
            }
        }
    }
}
