/*

This sample illustrates how to use the PylonDeviceGrabSingleFrame() convenience
method for grabbing images in a loop. PylonDeviceGrabSingleFrame() grabs one
single frame in single frame mode.

Grabbing in single frame mode is the easiest way to grab images. Note: in single frame
mode the maximum frame rate of the camera can't be achieved. The full frame
rate can be achieved by setting the camera to the continuous frame
mode and by grabbing in overlapped mode, i.e., image acquisition is done in parallel
with image processing. This is illustrated in the OverlappedGrab sample program.

*/

using System;
using System.Collections.Generic;
using System.IO;
using PylonC.NET;

namespace SimpleGrab
{
    class SimpleGrab
    {
        static void Main(string[] args)
        {
            if (args.Length != 2){
                Console.WriteLine("Wrong parameters:");
                Console.WriteLine("[camera serial number] [path to file]");
                return;
            }

            PYLON_DEVICE_HANDLE hDev = new PYLON_DEVICE_HANDLE(); /* Handle for the pylon device. */
            try
            {
                uint numDevices;    /* Number of available devices. */
                const int numGrabs = 1; /* Number of images to grab. */
                PylonBuffer<Byte> imgBuf = null;  /* Buffer used for grabbing. */
                bool isAvail;
                Pylon.Initialize();
                numDevices = Pylon.EnumerateDevices();

                if (0 == numDevices)
                {
                    Console.WriteLine("Error: No devices found");
                    return;
                }

                bool deviceFound = false;
                uint deviceNum = 0;
                for (uint di = 0; di < numDevices; di++) {
                    PYLON_DEVICE_INFO_HANDLE hDi = Pylon.GetDeviceInfoHandle((uint)di);
                    string serial = Pylon.DeviceInfoGetPropertyValueByName(hDi, Pylon.cPylonDeviceInfoSerialNumberKey);
                    deviceNum = di;
                    if (serial.Equals(args[0])) {
                        deviceFound = true;
                        break;
                    }
                }

                if (!deviceFound) {
                    Console.WriteLine("Error: No devices found by serial number");
                    return;
                }

                hDev = Pylon.CreateDeviceByIndex(deviceNum);
                Pylon.DeviceOpen(hDev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);
                isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_PixelFormat_Mono8");

                if (!isAvail)
                {
                    Console.WriteLine("Error: Device doesn't support the Mono8 pixel format");
                    return;
                }

                Pylon.DeviceFeatureFromString(hDev, "PixelFormat", "Mono8");
                isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_TriggerSelector_AcquisitionStart");
                if (isAvail)
                {
                    Pylon.DeviceFeatureFromString(hDev, "TriggerSelector", "AcquisitionStart");
                    Pylon.DeviceFeatureFromString(hDev, "TriggerMode", "Off");
                }

                isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_TriggerSelector_FrameBurstStart");
                if (isAvail)
                {
                    Pylon.DeviceFeatureFromString(hDev, "TriggerSelector", "FrameBurstStart");
                    Pylon.DeviceFeatureFromString(hDev, "TriggerMode", "Off");
                }

                isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_TriggerSelector_FrameStart");
                if (isAvail)
                {
                    Pylon.DeviceFeatureFromString(hDev, "TriggerSelector", "FrameStart");
                    Pylon.DeviceFeatureFromString(hDev, "TriggerMode", "Off");
                }

                isAvail = Pylon.DeviceFeatureIsWritable(hDev, "GevSCPSPacketSize");

                if (isAvail)
                {
                    Pylon.DeviceSetIntegerFeature(hDev, "GevSCPSPacketSize", 1500);
                }
                
                Byte min, max;
                PylonGrabResult_t grabResult;

                if (!Pylon.DeviceGrabSingleFrame(hDev, 0, ref imgBuf, out grabResult, 5000))
                {
                    /* Timeout occurred. */
                    //Console.WriteLine("Frame {0}: timeout.", i + 1);
                    Console.WriteLine("Error: timeout");
                }

                /* Check to see if the image was grabbed successfully. */
                if (grabResult.Status == EPylonGrabStatus.Grabbed)
                {
                    /* Success. Perform image processing. */
                    getMinMax(imgBuf.Array, grabResult.SizeX, grabResult.SizeY, out min, out max);
                    //Console.WriteLine("Grabbed frame {0}. Min. gray value = {1}, Max. gray value = {2}", i + 1, min, max);
                    Console.WriteLine("Frame grabbed success");

                    /* Display image */
                    //Pylon.ImagePersistenceSave<Byte>(EPylonImageFileFormat.ImageFileFormat_Png, "C:\\Users\\v.yakubov\\grabber\\test.png", imgBuf, grabResult.PixelType, (uint)grabResult.SizeX, (uint)grabResult.SizeY, 0, EPylonImageOrientation.ImageOrientation_TopDown);
                    Pylon.ImagePersistenceSave<Byte>(EPylonImageFileFormat.ImageFileFormat_Png, args[1], imgBuf, grabResult.PixelType, (uint)grabResult.SizeX, (uint)grabResult.SizeY, 0, EPylonImageOrientation.ImageOrientation_TopDown);
                }
                else if (grabResult.Status == EPylonGrabStatus.Failed)
                {
                    //Console.Error.WriteLine("Frame {0} wasn't grabbed successfully.  Error code = {1}", i + 1, grabResult.ErrorCode);
                    Console.WriteLine("Error: failed");
                }
                
                imgBuf.Dispose();
                Pylon.DeviceClose(hDev);
                Pylon.DestroyDevice(hDev);
                imgBuf = null;
                Pylon.Terminate();
            }
            catch (Exception e)
            {
                try
                {
                    if (hDev.IsValid)
                    {
                        if (Pylon.DeviceIsOpen(hDev)){Pylon.DeviceClose(hDev);}
                        Pylon.DestroyDevice(hDev);
                    }
                }
                catch (Exception){}
                Pylon.Terminate();  /* Releases all pylon resources. */
                Environment.Exit(1);
            }
        }

        static void getMinMax(Byte[] imageBuffer, long width, long height, out Byte min, out Byte max)
        {
            min = 255; max = 0;
            long imageDataSize = width * height;

            for (long i = 0; i < imageDataSize; ++i)
            {
                Byte val = imageBuffer[i];
                if (val > max)
                    max = val;
                if (val < min)
                    min = val;
            }
        }
    }
}
