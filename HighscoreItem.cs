using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace App3
{
    public class HighscoreItem
    {
        public int Index {get; set;}

        [XmlIgnore]
        public TimeSpan Time { get; set; }

        [XmlAttribute("Timestring", DataType ="duration")]
        public string XmlTime
        {
            get { return XmlConvert.ToString(Time); }
            set { Time = XmlConvert.ToTimeSpan(value); }
        }
        public HighscoreItem()
        {
            Index = 0;
        }
        public HighscoreItem (int scoreIndex, TimeSpan solvingTime)
        {
            Index = scoreIndex;
            Time = solvingTime;
        }
    }
}
