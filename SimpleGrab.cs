

using System;
using System.Collections.Generic;
using System.IO;
using PylonC.NET;

namespace SimpleGrab
{
    class SimpleGrab
    {

        
        static string arg_cameraSerialNumber   = "";
        static string arg_pathToFile           = "";
        static string arg_imageFormat          = "PNG";
        static string arg_packageSize          = "1500";
        static string arg_interPackageDelay    = "1000";
        static string arg_attemptsToGrab       = "1";
        static string arg_exposureTime         = "35000";

        const string par_cameraSerialNumber   = "-s";
        const string par_pathToFile           = "-o";
        const string par_imageFormat          = "-f";
        const string par_packageSize          = "-p";
        const string par_interPackageDelay    = "-d";
        const string par_attemptsToGrab       = "-a";
        const string par_exposureTime         = "-e";

        static void Main(string[] args)
        {
            for(uint i=0; i<args.Length; i++){
                switch(args[i]){
                    case par_cameraSerialNumber:
                        arg_cameraSerialNumber = args[i+1];
                        i++;
                        break;
                    case par_pathToFile:
                        arg_pathToFile = args[i+1];
                        i++;
                        break;
                    case par_imageFormat:
                        arg_imageFormat = args[i+1];
                        i++;
                        break;
                    case par_packageSize:
                        arg_packageSize = args[i+1];
                        i++;
                        break;
                    case par_interPackageDelay:
                        arg_interPackageDelay = args[i+1];
                        i++;
                        break;
                    case par_attemptsToGrab:
                        arg_attemptsToGrab = args[i + 1];
                        i++;
                        break;
                    case par_exposureTime:
                        arg_exposureTime = args[i + 1];
                        i++;
                        break;
                    default:
                        break;
                }
            }

            //arg_exposureTime = "35000";

            bool error = false;

            if(arg_pathToFile.Length == 0){
                Console.WriteLine("Path to file is empty");
                error = true;
            }

            if( !(arg_imageFormat.Equals("BMP") || !arg_imageFormat.Equals("PNG") || !arg_imageFormat.Equals("JPG") || !arg_imageFormat.Equals("RAW") || !arg_imageFormat.Equals("TIFF"))){
                Console.WriteLine("File format should be [BMP|PNG|JPG|RAW|TIFF]");
                error = true;
            }

            if(arg_cameraSerialNumber.Length == 0){
                Console.WriteLine("Camera serial number is empty");
                error = true;
            }

            int exposureTime = 0;
            try
            {
                exposureTime = Int32.Parse(arg_exposureTime);
            }
            catch (Exception e)
            {
                Console.WriteLine("Wrong exposure time value");
                error = true;
            }

            int interPackageDelay = 0;
            try
            {
                interPackageDelay = Int32.Parse(arg_interPackageDelay);
            }
            catch (Exception e)
            {
                Console.WriteLine("Wrong interPackageDelay value");
                error = true;
            }

            int attemptsToGrap = 0;
            try
            {
                attemptsToGrap = Int16.Parse(arg_attemptsToGrab);
            }catch (Exception e) {
                Console.WriteLine("Wrong attempts to grab value");
                error = true;
            }
            //sodapef
            if(error){
                Console.WriteLine("Parameters usage:");
                Console.WriteLine("-s  Camera serial number");
                Console.WriteLine("-o  Path to file");
                Console.WriteLine("-d  Inter package delay in ticks (default 1000)");
                Console.WriteLine("-a  Attempts tp grab image (default 1)");
                Console.WriteLine("-p  Package size (default 1500)");
                Console.WriteLine("-e  Exposure time (default 35000)");
                Console.WriteLine("-f  Image format [BMP|PNG|JPG|RAW|TIFF]");
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
                    if (serial.Equals(arg_cameraSerialNumber)) {
                        deviceFound = true;
                        break;
                    }
                }

                if (!deviceFound) {
                    Console.WriteLine("Error: No devices found by serial number");
                    return;
                }

                hDev = Pylon.CreateDeviceByIndex(deviceNum);
                Pylon.DeviceOpen(hDev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream | Pylon.cPylonAccessModeExclusive );
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
                    int packageSize = 1500;
                    try{
                        packageSize = Int32.Parse(arg_packageSize);
                    }
                    catch (Exception e) { }
                    Pylon.DeviceSetIntegerFeature(hDev, "GevSCPSPacketSize", packageSize);
                }

                isAvail = Pylon.DeviceFeatureIsWritable(hDev, "GevSCPD");
                
                if (isAvail)
                {
                    Pylon.DeviceSetIntegerFeature(hDev, "GevSCPSPacketSize", interPackageDelay);
                }

                Pylon.DeviceFeatureFromString(hDev, "ExposureAuto", "Off");

                /*
                isAvail = Pylon.DeviceFeatureIsWritable(hDev, "ExposureTimeRaw");
                if (isAvail)
                {
                    try
                    {
                        Pylon.DeviceSetFloatFeature(hDev, "ExposureTimeRaw", exposureTime);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Some error 1");
                    }
                }*/

                
                isAvail = Pylon.DeviceFeatureIsWritable(hDev, "ExposureTimeAbs");
                if (isAvail)
                {
                    Pylon.DeviceSetFloatFeature(hDev, "ExposureTimeAbs", (long)exposureTime);
                    /*
                    try{
                        Pylon.DeviceSetFloatFeature(hDev, "ExposureTimeAbs", (long)exposureTime);
                    }catch (Exception e) {
                        //Console.WriteLine("Some error 2");
                    }
                    */
                }

                Byte min, max;
                PylonGrabResult_t grabResult;

                for (int attempt=0; attempt<attemptsToGrap; attempt++) { 
                

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
                        if(arg_imageFormat.Equals("PNG")) Pylon.ImagePersistenceSave<Byte>(EPylonImageFileFormat.ImageFileFormat_Png, arg_pathToFile, imgBuf, grabResult.PixelType, (uint)grabResult.SizeX, (uint)grabResult.SizeY, 0, EPylonImageOrientation.ImageOrientation_TopDown);
                        else if (arg_imageFormat.Equals("JPG")) Pylon.ImagePersistenceSave<Byte>(EPylonImageFileFormat.ImageFileFormat_Jpeg, arg_pathToFile, imgBuf, grabResult.PixelType, (uint)grabResult.SizeX, (uint)grabResult.SizeY, 0, EPylonImageOrientation.ImageOrientation_TopDown);
                        else if (arg_imageFormat.Equals("RAW")) Pylon.ImagePersistenceSave<Byte>(EPylonImageFileFormat.ImageFileFormat_Raw, arg_pathToFile, imgBuf, grabResult.PixelType, (uint)grabResult.SizeX, (uint)grabResult.SizeY, 0, EPylonImageOrientation.ImageOrientation_TopDown);
                        else if (arg_imageFormat.Equals("TIFF")) Pylon.ImagePersistenceSave<Byte>(EPylonImageFileFormat.ImageFileFormat_Tiff, arg_pathToFile, imgBuf, grabResult.PixelType, (uint)grabResult.SizeX, (uint)grabResult.SizeY, 0, EPylonImageOrientation.ImageOrientation_TopDown);
                        else if (arg_imageFormat.Equals("BMP")) Pylon.ImagePersistenceSave<Byte>(EPylonImageFileFormat.ImageFileFormat_Bmp, arg_pathToFile, imgBuf, grabResult.PixelType, (uint)grabResult.SizeX, (uint)grabResult.SizeY, 0, EPylonImageOrientation.ImageOrientation_TopDown);
                        else Pylon.ImagePersistenceSave<Byte>(EPylonImageFileFormat.ImageFileFormat_Bmp, arg_pathToFile, imgBuf, grabResult.PixelType, (uint)grabResult.SizeX, (uint)grabResult.SizeY, 0, EPylonImageOrientation.ImageOrientation_TopDown);

                        break;

                    }
                    else if (grabResult.Status == EPylonGrabStatus.Failed)
                    {
                        //Console.Error.WriteLine("Frame {0} wasn't grabbed successfully.  Error code = {1}", i + 1, grabResult.ErrorCode);
                        Console.WriteLine("Error: failed");
                    }

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
