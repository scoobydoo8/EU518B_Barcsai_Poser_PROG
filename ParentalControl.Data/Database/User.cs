//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ParentalControl.Data.Database
{
    using ParentalControl.Interface.Database;
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// User class.
    /// </summary>
    public partial class User : IUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        private User()
        {
            this.ProgramSettings = new HashSet<ProgramSetting>();
            this.TimeSettings = new HashSet<TimeSetting>();
            this.WebSettings = new HashSet<WebSetting>();
        }

        internal User(string username, string password, string securityQuestion, string securityAnswer) : this()
        {
            this.Username = username;
            this.Password = password;
            this.SecurityQuestion = securityQuestion;
            this.SecurityAnswer = securityAnswer;
        }

        /// <summary>
        /// ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Security question.
        /// </summary>
        public string SecurityQuestion { get; set; }

        /// <summary>
        /// Security answer.
        /// </summary>
        public string SecurityAnswer { get; set; }

        /// <summary>
        /// Program settings.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProgramSetting> ProgramSettings { get; set; }

        /// <summary>
        /// Time settings.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TimeSetting> TimeSettings { get; set; }

        /// <summary>
        /// Web settings.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WebSetting> WebSettings { get; set; }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns>String.</returns>
        public override string ToString()
        {
            return this.Username;
        }
    }
}