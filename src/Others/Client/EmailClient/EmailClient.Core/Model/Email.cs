using MailKit;
using MimeKit;
using System;

namespace EmailClient.Core.Model
{
    /// <summary>
    /// An instance of this class represents a MimeMessage object (E-mail)
    /// This class is necessary becaue a MimeMessage object can not be serialized
    /// </summary>
    [Serializable]
    public class Email : IEquatable<Email>
    {
        /// <summary>
        /// Gets the unique id of the email
        /// </summary>
        public uint ID { get; }

        /// <summary>
        /// Gets the sender of the email
        /// </summary>
        public string SenderAddress { get; }

        /// <summary>
        /// Gets the name of the sender
        /// </summary>
        public string SenderName { get; }

        /// <summary>
        /// Gets the obejct of the email
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// Gets the Timestamp of the email
        /// </summary>
        public DateTime Timestamp { get; }        

        /// <summary>
        /// Gets the Message of the email
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Get, sets wether the email seen or not.
        /// </summary>
        public bool Seen { get; set; }

        /// <summary>
        /// This property gets the string representation if the object.
        /// </summary>
        public string Str => this.ToString();

        /// <summary>
        /// Constructor overload of the class
        /// Initializes a new object with a MimeMessage object
        /// </summary>
        /// <param name="email">MimeMessage object, that is created by the MailKit service after downloading an email</param>
        public Email(UniqueId ID, MimeMessage email)
        {
            this.ID = ID.Id;
            var enumerator = email.From.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SenderName = enumerator.Current.Name;
                SenderAddress = ((MailboxAddress)enumerator.Current).Address;
            }
            Subject = email.Subject;
            Timestamp = email.Date.DateTime;
            Message = email.TextBody;
        }

        /// <summary>
        /// Returns the string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Sender: {SenderAddress}\nSubject: {Subject}\n";
        }


        /// <summary>
        /// Implementation of the type specific Equals method of the IEquatable inteface, for value equality check
        /// </summary>
        /// <param name="other">Other Email object to compare to</param>
        /// <returns></returns>
        public bool Equals(Email other)
        {
            return ID.Equals(other.ID);
        }
    }
}
