﻿// Copyright © 2006-2010 Travis Robinson. All rights reserved.
// 
// website: http://sourceforge.net/projects/libusbdotnet
// e-mail:  libusbdotnet@gmail.com
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
// for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA. or 
// visit www.gnu.org.
// 
// 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LibUsbDotNet.Descriptors;
using LibUsb.Common;
using LibUsbDotNet.Info;
using LibUsbDotNet;
using System.Linq;

namespace MonoLibUsb.Descriptors
{
    /// <summary>
    /// A structure representing the standard USB interface descriptor. This
    /// descriptor is documented in section 9.6.5 of the USB 2.0 specification.
    /// All multiple-byte fields are represented in host-endian format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = MonoUsbApi.LIBUSB_PACK)]
    public class MonoUsbAltInterfaceDescriptor : UsbInterfaceDescriptor
    {
        ///<summary> Array of endpoint descriptors. This length of this array is determined by the bNumEndpoints field.</summary>
        private readonly IntPtr pEndpointDescriptors;

        ///<summary> Extra descriptors. If libusb encounters unknown interface descriptors, it will store them here, should you wish to parse them.</summary>
        private IntPtr pExtraBytes;

        ///<summary> Length of the extra descriptors, in bytes.</summary>
        public readonly int ExtraLength;

        ///<summary> Extra descriptors. If libusb encounters unknown interface descriptors, it will store them here, should you wish to parse them.</summary>
        public byte[] ExtraBytes
        {
            get
            {
                byte[] bytes = new byte[ExtraLength];
                Marshal.Copy(pExtraBytes, bytes, 0, bytes.Length);
                return bytes;
            }
        }

        ///<summary> Array of endpoint descriptors. This length of this array is determined by the bNumEndpoints field.</summary>
        public List<IUsbEndpointDescriptor> EndpointList
        {
            get
            {
                List<IUsbEndpointDescriptor> endpointList = new List<IUsbEndpointDescriptor>();
                int iEndpoint;
                for (iEndpoint = 0; iEndpoint < EndpointCount; iEndpoint++)
                {
                    IntPtr pNextInterface = new IntPtr(pEndpointDescriptors.ToInt64() + (Marshal.SizeOf(typeof (MonoUsbEndpointDescriptor))*iEndpoint));
                    MonoUsbEndpointDescriptor monoUsbEndpoint = new MonoUsbEndpointDescriptor();
                    Marshal.PtrToStructure(pNextInterface, monoUsbEndpoint);
                    endpointList.Add(monoUsbEndpoint);
                }

                return endpointList;
            }
        }

        public UsbInterfaceInfo ToUsbInterfaceInfo(UsbDevice usbDevice)
        {
            return new UsbInterfaceInfo(usbDevice, this, this.EndpointList);
        }
    }
}