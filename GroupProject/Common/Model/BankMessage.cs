using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Model
{
    [DataContract]
    public class BankMessage : Message
    {

        [DataMember]
        public string Message { get; set; }
    }
}
