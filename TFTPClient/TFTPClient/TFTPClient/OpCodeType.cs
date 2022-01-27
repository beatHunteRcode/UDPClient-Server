using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTPClient
{
    enum OpCodeType
    {
        RRQ = (short)1,
        WRQ = (short)2,
        DATA = (short)3,
        ACK = (short)4,
        ERROR = (short)5
    }
}
