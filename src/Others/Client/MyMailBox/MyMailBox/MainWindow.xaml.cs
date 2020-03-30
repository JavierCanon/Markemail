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
    public partial class MainWindow : Window
    {

        private MailBoxContainer mailBoxContainer;
        private Contact contact;
        private Calendar calendar;
        private Settings settings;
        private List<Account> listAccounts = null;
        private const String settingsSeparateValue = ",";

        public MainWindow()
        {
            this.InitializeComponent();
            ThreadInvoker.Instance.InitDispacter();
            //resetUserSettings();
            this.getAllViews();
            this.getAllAccount();
            this.settingsAllViews();
        }

        private void getAllAccount()
        {
            if (Properties.Settings.Default.ListAccount != null)
            {
                List<String> listStringAccounts = Properties.Settings.Default.ListAccount.Cast<String>().ToList();
                listAccounts = new List<Account>();
                foreach (String stringAccount in listStringAccounts)
                {
                    listAccounts.Add(new Account(stringAccount.Split(settingsSeparateValue[0])));
                }
            }
        }

        private void getAllViews()
        {
            this.settings = SettingsActivity;
            this.mailBoxContainer = MailBoxContainerActivity;
        }

        private void settingsAllViews()
        {
            this.settings.setMainWindow(this);
            this.settings.setSettingsSeparateValue(settingsSeparateValue);
            this.settings.setNewListAccount(listAccounts);
            this.mailBoxContainer.setNewListAccount(listAccounts);
        }

        public void deleteAccount(Account account)
        {
            if (listAccounts != null)
            {
                mailBoxContainer.deleteMailBox(account);
                listAccounts.Remove(account);
            }
        }

        public void addNewAccount(Account account)
        {
            mailBoxContainer.addMailBox(new MailBox(account));
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* rIENENNEnez FIJZENBFGIJEZ*/
        }

        private void resetUserSettings()
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
        }
    }
}
