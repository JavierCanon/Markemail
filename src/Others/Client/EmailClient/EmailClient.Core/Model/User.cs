using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EmailClient.Core.Model
{
    /// <summary>
    /// This class represenets a User of the program.
    /// A user can have many different Mailboxes. 
    /// </summary>
    [Serializable]
    public class User : IDeserializationCallback
    {
        private static int instanceCounter = 0;

        /// <summary>
        /// Gets the unique ID of ther User.
        /// </summary>
        public int ID { get; }
        
        /// <summary>
        /// Gets the name of ther user.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Birth date of the User
        /// </summary>
        public DateTime BirthDate { get; }

        /// <summary>
        /// Gets the collection of the user's mailboxes.
        /// </summary>
        public ObservableCollection<Mailbox> Mailboxes { get; } = new ObservableCollection<Mailbox>();

        /// <summary>
        /// Gets or sets the currently selected Mailbox.
        /// </summary>
        public Mailbox CurrentMailbox { get; set; }

        /// <summary>
        /// Static method to binary serialize a user.
        /// </summary>
        /// <param name="user">Person object to serialize</param>
        /// <param name="destPath">The path of the destination folder</param>
        public static void Serialize(User user, string destPath)
        {
            var fileName = Path.Combine(destPath, $"{user.Name}.dat");
            using (var fs = File.Open(fileName, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fs, user);
                }
                catch (SerializationException exception)
                {
                    Console.WriteLine("Serialization: " + exception.Message);
                }
            }
        }

        /// <summary>
        /// Static method to binary deserialie a user object from file
        /// </summary>
        /// <param name="fileName">Full path of the serialized user object</param>
        /// <returns>A deserialized user</returns>
        public static User Deserialize(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                try
                {
                    return (User)formatter.Deserialize(fs);
                }
                catch (SerializationException exception)
                {
                    Console.WriteLine("Deserialization: " + exception.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Initializes a user object.
        /// </summary>
        /// <param name="name">Name of the new user</param>
        /// <param name="birthDate">Birth data of the new user</param>
        /// <param name="dwnldStatus">refference of the UI's dwnldstatus textblock</param>
        public User(string name, DateTime birthDate)
        {
            ID = ++instanceCounter;
            Name = name;
            BirthDate = birthDate;
        }

        /// <summary>
        /// Adds a new mailbox to the user.
        /// The firstly added Mailbox will be the default one.
        /// </summary>
        /// <param name="properties">MailBoxProperties struct with the necessary data to initialize a new mailbox</param>
        public Mailbox AddMailbox(MailBoxProperties properties)
        {
            var newMailbox = new Mailbox(properties);
            Mailboxes.Add(newMailbox);
            if(Mailboxes.Count == 1)
            {
                CurrentMailbox = newMailbox;
            }
            return newMailbox;
        }

        /// <summary>
        /// Deletes the current mailbox
        /// </summary>
        public void DeleteMailbox()
        {
            CurrentMailbox.Control.StopGettingEmails();
            Mailboxes.Remove(CurrentMailbox);
            CurrentMailbox = null;
        }

        /// <summary>
        /// After deserialization it restorse the actual value of the instanceCounter static field.
        /// </summary>
        /// <param name="sender">sender</param>
        public void OnDeserialization(object sender)
        {
            instanceCounter = Mailboxes.Count;
        }

        /// <summary>
        /// Return the string representation of the User class.
        /// </summary>
        /// <returns>Returns the name of the user</returns> 
        public override string ToString()
        {
            return Name;
        }
    }
}
