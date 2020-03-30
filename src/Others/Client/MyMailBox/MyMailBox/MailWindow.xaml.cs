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
using System.Windows.Shapes;
using MyMailBox.Models;

namespace MyMailBox
{
    /// <summary>
    /// Logique d'interaction pour MailWindow.xaml
    /// </summary>
    public partial class MailWindow : Window
    {
        public MailWindow(Mail mail)
        {
            InitializeComponent();

            FromBlock.Text = mail.mailFrom;
            SubjectBlock.Text = mail.mailObject;
            ToBlock.Text = mail.getStringListTo();
            MailContentWebView.NavigateToString(mail.body);
        }
    }
}
