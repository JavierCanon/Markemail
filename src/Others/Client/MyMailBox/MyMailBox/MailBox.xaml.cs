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
using MailKit;
using MyMailBox.Models;
using MyMailBox.Utils;

namespace MyMailBox
{
    /// <summary>
    /// Logique d'interaction pour MailBox.xaml
    /// </summary>
    public partial class MailBox : UserControl
    {
        private Account account;
        private List<MailPreview> listMailPreview = null;

        public MailBox(Account account)
        {
            InitializeComponent();
            this.account = account;
        }

        private void showAllEmail(int max = 300, Boolean forceUpdate = true)
        {
            if (!account.connection())
            {
                System.Diagnostics.Debug.WriteLine("Not connected ERROR");
                return;
            }
            if (forceUpdate || this.listMailPreview == null)
            {
                this.listMailPreview = account.getAllMailPreview();
            }
            ThreadInvoker.Instance.RunByUiThread(() =>
            {
                ListMailPreview.ItemsSource = this.listMailPreview;
            });
        }

        public void Show()
        {
            showAllEmail();
        }

        public String getMailBoxName()
        {
            return this.account.getEmail();
        }

        public int getID()
        {
            return account.getID();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            MailPreview mailPreview = row.Item as MailPreview;
            Mail mail = account.getEmailById(mailPreview.getUniqueID());
            MailWindow mailWindow = new MailWindow(mail);
            mailWindow.Show();
        }
    }
}
