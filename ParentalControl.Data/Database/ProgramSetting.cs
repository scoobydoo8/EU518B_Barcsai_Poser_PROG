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
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Program setting class.
    /// </summary>
    public partial class ProgramSetting
    {
        internal ProgramSetting(int userID, string name, string path, bool occasional, int minutes, bool repeat, int pause, int quantity, bool orderly, TimeSpan fromTime, TimeSpan toTime)
        {
            this.UserID = userID;
            this.Name = name;
            this.Path = path;
            this.Occasional = occasional;
            this.Minutes = minutes;
            this.Repeat = repeat;
            this.Pause = pause;
            this.Quantity = quantity;
            this.Orderly = orderly;
            this.FromTime = fromTime;
            this.ToTime = toTime;
        }

        /// <summary>
        /// ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// UserID.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Occasional.
        /// </summary>
        public bool Occasional { get; set; }

        /// <summary>
        /// Minutes.
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Repeat.
        /// </summary>
        public bool Repeat { get; set; }

        /// <summary>
        /// Pause.
        /// </summary>
        public int Pause { get; set; }

        /// <summary>
        /// Quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Orderly.
        /// </summary>
        public bool Orderly { get; set; }

        /// <summary>
        /// From time.
        /// </summary>
        public System.TimeSpan FromTime { get; set; }

        /// <summary>
        /// To time.
        /// </summary>
        public System.TimeSpan ToTime { get; set; }

        /// <summary>
        /// User.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns>String.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Name, this.Path);
        }
    }
}