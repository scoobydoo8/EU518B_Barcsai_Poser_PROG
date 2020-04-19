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
    /// Program limitation class.
    /// </summary>
    public partial class ProgramLimitation : IProgramLimitation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramLimitation"/> class.
        /// </summary>
        /// <param name="userID">UserID.</param>
        /// <param name="name">Name.</param>
        /// <param name="path">Path.</param>
        /// <param name="occasional">Occasional.</param>
        /// <param name="minutes">Minutes.</param>
        /// <param name="repeat">Repeat.</param>
        /// <param name="pause">Pause.</param>
        /// <param name="quantity">Quantity.</param>
        /// <param name="orderly">Orderly.</param>
        /// <param name="fromTime">From time.</param>
        /// <param name="toTime">To time.</param>
        internal ProgramLimitation(int userID, string name, string path, bool occasional, int minutes, bool repeat, int pause, int quantity, bool orderly, TimeSpan fromTime, TimeSpan toTime)
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


        /// <inheritdoc/>
        public int ID { get; set; }

        /// <inheritdoc/>
        public int UserID { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        public bool Occasional { get; set; }

        /// <inheritdoc/>
        public int Minutes { get; set; }

        /// <inheritdoc/>
        public bool Repeat { get; set; }

        /// <inheritdoc/>
        public int Pause { get; set; }

        /// <inheritdoc/>
        public int Quantity { get; set; }

        /// <inheritdoc/>
        public bool Orderly { get; set; }

        /// <inheritdoc/>
        public TimeSpan FromTime { get; set; }

        /// <inheritdoc/>
        public TimeSpan ToTime { get; set; }

        /// <summary>
        /// Gets or sets user.
        /// </summary>
        public virtual User User { get; set; }
    }
}