using System;
using System.Collections.ObjectModel;
using EmailClient.Core.Model;
using System.Runtime.Serialization;
using EmailClient.Core.Service;
//nzaxrbfyucvagsse

namespace EmailClient.Core
{
    /// <summary>
    /// This class represent an E-mail account
    /// </summary>
    [Serializable]
    public class Mailbox : IDeserializationCallback
    {
        /// <summary>
        /// Gets, sets the Mailboxproperties object
        /// MailBoxProperties object contains the neccessary data of the mailbox
        /// </summary>
        public MailBoxProperties Properties { get; private set; }

        /// <summary>
        /// Gets, the MailControl object. (This contains the a)
        /// </summary>
        [field: NonSerialized]
        public MailControl Control { get; private set; }

        /// <summary>
        /// Gets the downloaded E-mails.
        /// This collection observed by the UI to render the inbox.
        /// </summary>
        public ObservableCollection<Email> Emails { private set; get; } = new ObservableCollection<Email>();

        /// <summary>
        /// Initializes a new inbox object, with the values of the properties struct given in the argument.
        /// </summary>
        /// <param name="properties">MailboxProperties struct, that carries the necessary data for the mailbox.</param>
        public Mailbox(MailBoxProperties properties)
        {
            Properties = properties;
            Control = new MailControl(properties, Emails);
        }

        /// <summary>
        /// Returns the string representation of the Mailbox object.
        /// </summary>
        /// <returns>The name of the mailbox.</returns>
        public override string ToString()
        {
            return Properties.Name;
        }

        /// <summary>
        /// Callback method after deserialization
        /// </summary>
        /// <param name="sender">sender</param>
        public void OnDeserialization(object sender)
        {
            Control = new MailControl(Properties, Emails);
        }
    }
}