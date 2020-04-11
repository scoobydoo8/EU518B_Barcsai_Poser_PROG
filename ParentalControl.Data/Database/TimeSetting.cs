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
    /// TimeSetting class.
    /// </summary>
    public partial class TimeSetting : ITimeSetting
    {
        internal TimeSetting(int userID, bool occasional, int minutes, bool orderly, TimeSpan fromTime, TimeSpan toTime)
        {
            this.UserID = userID;
            this.Occasional = occasional;
            this.Minutes = minutes;
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
        /// Occasional.
        /// </summary>
        public bool Occasional { get; set; }

        /// <summary>
        /// Minutes.
        /// </summary>
        public int Minutes { get; set; }

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
            return this.UserID.ToString();
        }
    }
}