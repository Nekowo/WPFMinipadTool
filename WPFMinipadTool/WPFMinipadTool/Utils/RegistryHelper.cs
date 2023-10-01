﻿using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace MinipadWPFTest.Utils
{
    public class RegistryHelper
    {
        public static IEnumerable<int> GetPortsByIDs(int vid, int pid)
        {
            // Concat the prefix of the registry key for the device together.
            string prefix = $"VID_{vid.ToString("X").PadLeft(4, '0')}&PID_{pid.ToString("X").PadLeft(4, '0')}";

            // Go through all USB devices in the registry.
            RegistryKey usb = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\USB")!;
            foreach (var key in usb.GetSubKeyNames().Select(x => usb.OpenSubKey(x)!))
            {
                // Check whether the device starts with the prefix defined before.
                if (!key.Name.Split('\\')[^1].StartsWith(prefix))
                    continue;

                // Find a subkey that contains a "Device Parameters" subkey.
                foreach (var subKey in key.GetSubKeyNames().Select(x => key.OpenSubKey(x)!))
                {
                    RegistryKey? deviceParameters = subKey.OpenSubKey("Device Parameters");
                    if (deviceParameters == null)
                        continue;

                    // Get the COM port from the "Device Parameters" subkey.
                    string? portName = (string?)deviceParameters.GetValue("PortName");
                    if (portName == null)
                        continue;

                    // yield return the COM port in order to return all ports for matching devices.
                    yield return int.Parse(portName.Substring(3));
                }
            }
        }
    }
}
