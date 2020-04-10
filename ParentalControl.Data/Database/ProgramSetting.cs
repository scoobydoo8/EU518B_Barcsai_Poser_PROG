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
    
    public partial class ProgramSetting
    {
        internal ProgramSetting(int userID, string name, string path, bool occasional, int minutes, bool repeat, int pause, int quantity, bool orderly, TimeSpan fromTime, TimeSpan toTime)
        {
            UserID = userID;
            Name = name;
            Path = path;
            Occasional = occasional;
            Minutes = minutes;
            Repeat = repeat;
            Pause = pause;
            Quantity = quantity;
            Orderly = orderly;
            FromTime = fromTime;
            ToTime = toTime;
        }

        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Occasional { get; set; }
        public int Minutes { get; set; }
        public bool Repeat { get; set; }
        public int Pause { get; set; }
        public int Quantity { get; set; }
        public bool Orderly { get; set; }
        public System.TimeSpan FromTime { get; set; }
        public System.TimeSpan ToTime { get; set; }
    
        public virtual User User { get; set; }
    }
}
