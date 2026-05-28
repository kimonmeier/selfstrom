using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStrom.Api.Models;

public enum DeviceType
{
    Switch_CH_V1 = 101,   // WSW
    Bulb = 102,           // WRS
    Button_Plus = 103,    // WBP
    Button = 104,         // WBS
    LED_Strip = 105,      // WRS
    Switch_CH_V2 = 106,   // WS2
    Switch_EU = 107,      // WSE
    Motion_Sensor = 110,  // WMS
    Modulo_CUBO = 113,    // WLL
    Button_Plus_2nd = 118, // BP2
    Switch_Zero = 120     // LCS
}
