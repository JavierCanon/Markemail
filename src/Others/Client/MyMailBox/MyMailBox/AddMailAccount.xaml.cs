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
using MailKit;
using MailKit.Net.Imap;
using MyMailBox.Models;

namespace MyMailBox
{
    /// <summary>
    /// Logique d'interaction pour AddMailAccount.xaml
    /// </summary>
    public partial class AddMailAccount : Window
    {
        private Account inCreationAccount;

        private int stepAddAccount = 0;
        private Settings resultClass;

        private Boolean isUpdateAccount = false;
        private int updateAccountId = -1;

        public AddMailAccount(Settings settings)
        {
            this.InitializeComponent();
            resultClass = settings;
        }

        public AddMailAccount(Settings settings, Account account)
        {
            this.InitializeComponent();
            isUpdateAccount = true;
            resultClass = settings;
            inCreationAccount = account;
            updateAccountId = account.getID();
            goNextStep(1);
            goNextStep(2);
            fillStep3Information(true);
        }

        private void NextActionAddAccount(object sender, RoutedEventArgs e)
        {
            Boolean canNext = false;
            switch (stepAddAccount)
            {
                case 0:
                    canNext = doStepOne();
                    break;
                case 1:
                    canNext = doStepTwo();
                    break;
                default:
                    break;
            }
            if (canNext)
            {
                goNextStep();
            }
        }

        private void goNextStep(int stepNumber = -1)
        {
            if (stepNumber <= 0)
            {
                stepAddAccount++;
            }
            else
            {
                stepAddAccount = stepNumber;
            }
            switch (stepAddAccount)
            {
                case 1:
                    frameStep1.Visibility = Visibility.Collapsed;
                    frameStep2.Visibility = Visibility.Visible;
                    break;
                case 2:
                    frameStep2.Visibility = Visibility.Collapsed;
                    frameStep3.Visibility = Visibility.Visible;
                    break;
                case 3:
                    frameStep3.Visibility = Visibility.Collapsed;
                    frameStep4.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private Boolean doStepOne()
        {
            Account newAccount = new Account(nameUserBox.Text, emailUserBox.Text, passwordUserBox.Password, rememberPasswordBox.IsChecked.Value);
            Boolean wrongField = false;

            if (newAccount.fullName == String.Empty)
            {
                nameUserBox.BorderBrush = new SolidColorBrush(Colors.Red);
                wrongField = true;
            }
            if (newAccount.getPassword() == String.Empty)
            {
                passwordUserBox.BorderBrush = new SolidColorBrush(Colors.Red);
                wrongField = true;
            }
            if (newAccount.getEmail() == String.Empty)
            {
                emailUserBox.BorderBrush = new SolidColorBrush(Colors.Red);
                wrongField = true;
            }
            if (!newAccount.getEmail().Contains("@"))
            {
                emailUserBox.BorderBrush = new SolidColorBrush(Colors.Red);
                wrongField = true;
            }
            if (!wrongField)
            {
                inCreationAccount = newAccount;
                goNextStep();
                return doStepTwo();
            }
            return false;
        }

        private Boolean doStepTwo()
        {

            Boolean result = inCreationAccount.testConnection();

            if (result == false)
            {
                showMessageCantConnect();
            }
            fillStep3Information(result);
            return true;
        }

        private void fillStep3Information(Boolean isAuthentificate)
        {
            NextButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            if (!isAuthentificate)
            {
                TitleStep3.Text = "Impossible de se connecter au serveur automatiquement, aide moi !";
            }
            identityConfirmBox.Text = inCreationAccount.getIdentity();
            serverConfirmBox.Text = inCreationAccount.getServer();
            portConfirmBox.Text = inCreationAccount.getPort() + "";
            SSLConfirmCheck.IsChecked = inCreationAccount.getUseSSL();
            passwordRememberConfirmCheck.IsChecked = inCreationAccount.rememberPassword;
            passwordConfirmBox.Password = inCreationAccount.getPassword() ;
            nameConfirmBox.Text = inCreationAccount.fullName;
        }

        private void CancelActionAddAccount(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void validateButtonClick(object sender, RoutedEventArgs e)
        {
            TitleStep3.Text = "Tentative de connexion ...";
            String email = identityConfirmBox.Text + "@" + serverConfirmBox.Text.Substring(serverConfirmBox.Text.IndexOf(".") + 1);
            int port = Int32.Parse(portConfirmBox.Text);
            String fullName = nameConfirmBox.Text;
            Boolean SSLCheck = SSLConfirmCheck.IsChecked.Value;
            String password = passwordConfirmBox.Password;
            Boolean rememberPassword = passwordRememberConfirmCheck.IsChecked.Value;

            Account newTryAccount = new Account(fullName, email, password, rememberPassword, SSLCheck, port);
            if (newTryAccount.testConnection() == false)
            {
                showMessageCantConnect();
                TitleStep3.Text = "Impossible de se connecter au serveur automatiquement, aide moi !";
            }
            else
            {
                inCreationAccount = newTryAccount;
                goNextStep();
            }
        }

        private void showMessageCantConnect()
        {
            MessageBox.Show("Impossible de se connecter à ton serveur de mail. Vérifies ces informations et tient moi au courant :)");
        }

        private void finalValidateClick(object sender, RoutedEventArgs e)
        {
            if (isUpdateAccount)
            {
                this.resultClass.updateAccountFinished(new Account(inCreationAccount, updateAccountId));
            } else
            {
                Properties.Settings.Default.UniqueAccountID++;
                this.resultClass.saveNewMailAccount(new Account(inCreationAccount, Properties.Settings.Default.UniqueAccountID));
            }
            this.Close();
        }
    }
}
