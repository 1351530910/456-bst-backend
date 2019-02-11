using System;
using System.Collections.Generic;

namespace Application.Model
{

    public class Channel
    {
        public Guid id { get; set; }
        public string FileName { get; set; }
        public string Comment { get; set; }
        public int nbChannels { get; set; }
#warning incomplete data types
        public ICollection<object> Modalities { get; set; }
        public ICollection<object> DisplayableSensorTypes { get; set; }
    }
}
