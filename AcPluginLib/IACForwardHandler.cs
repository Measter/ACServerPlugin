using System;
using System.Collections.Generic;
using System.Text;

namespace AcPluginLib
{
    public interface IACForwardHandler
    {
        ForwardHandling OnForward( ACSCommand packetID, byte[] rawBytes );
    }
}
