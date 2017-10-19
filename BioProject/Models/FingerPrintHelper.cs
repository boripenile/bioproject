using BioProject.Dtos;
using BioProject.Helper;
using GriauleFingerprintLibrary;
using GriauleFingerprintLibrary.DataTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace BioProject.Models
{
    [Serializable]
    public sealed class FingerPrintHelper
    {
        public FingerprintCore RefFingercore;
        public GrCaptureImageFormat GrImageFormat;

        private static volatile FingerPrintHelper instance;
        private static object syncRoot = new Object();

        private FingerPrintHelper() { }

        public static FingerPrintHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new FingerPrintHelper();
                    }
                }

                return instance;
            }
        }

        public void Init()
        {
            RefFingercore = new FingerprintCore();
            RefFingercore.Initialize();
            System.Diagnostics.Debug.WriteLine("Finger print core initialized...");
            RefFingercore.SetVerifyParameters(20, 180);
            RefFingercore.SetIdentifyParameters(80, 180);
            GrImageFormat = new GrCaptureImageFormat();
            GrImageFormat = GrCaptureImageFormat.GRCAP_IMAGE_FORMAT_BMP;
        }

        public FingerprintRawImage GetRawImageFromByte(FingerPrint fingerImage)
        {
            System.Diagnostics.Debug.WriteLine("I am here 000");
            string convertedGZip = Utils.DecompressToBase64FromDesktop(fingerImage.FingerImage);
            byte[] fingerImageByte = Convert.FromBase64String(convertedGZip);
            // Create a managed array.
            byte[] managedArray = fingerImageByte;
            System.Diagnostics.Debug.WriteLine("I am here AAA");
            FingerprintRawImage myRawImage = null;
            // Initialize unmanaged memory to hold the array.
            int size = Marshal.SizeOf(managedArray[0]) * managedArray.Length;
            System.Diagnostics.Debug.WriteLine("I am here 111");
            IntPtr pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(managedArray, 0, pnt, managedArray.Length);
                System.Diagnostics.Debug.WriteLine("I am here 222");
                myRawImage = new FingerprintRawImage(pnt, fingerImage.Width, 
                    fingerImage.Height, fingerImage.Resolution);

                System.Diagnostics.Debug.WriteLine("I am here 333");

            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return myRawImage;

        }

        public FingerprintTemplate ConvertRawImageToTemplate(FingerprintRawImage rawImage)
        {
            GriauleFingerprintLibrary.DataTypes.FingerprintTemplate _templateConvertT = null;

            RefFingercore.Extract(rawImage, ref _templateConvertT);

            return _templateConvertT;
        }

        //public FingerprintTemplate ConvertToTemplate(SearchFingerDto sourceFinger)
        //{
        //    var finger = new Finger()
        //    {
        //        FingerImage = sourceFinger.AnyFingerPrint,
        //        Width = sourceFinger.Width,
        //        Height = sourceFinger.Height,
        //        Resolution = sourceFinger.Resolution
        //    };

        //    FingerprintRawImage imageRaw = GetRawImageFromByte(finger);

        //    GriauleFingerprintLibrary.DataTypes.FingerprintTemplate _templateConvertT = new FingerprintTemplate();

        //    RefFingercore.Extract(imageRaw, ref _templateConvertT);

        //    return _templateConvertT;
        //}


        // ImageConverter object used to convert byte arrays containing JPEG or PNG file images into
        //  Bitmap objects. This is static and only gets instantiated once.
        private readonly ImageConverter _imageConverter = new ImageConverter();
        /// <summary>
        /// Method that uses the ImageConverter object in .Net Framework to convert a byte array,
        /// presumably containing a JPEG or PNG file image, into a Bitmap object, which can also be
        /// used as an Image object.
        /// </summary>
        /// <param name="byteArray">byte array containing JPEG or PNG file image or similar</param>
        /// <returns>Bitmap object if it works, else exception is thrown</returns>
        /// 
        public Image GetImageFromByteArray(byte[] byteArray)
        {

            Bitmap bm = (Bitmap)_imageConverter.ConvertFrom(byteArray);

            if (bm != null && (bm.HorizontalResolution != (int)bm.HorizontalResolution ||
                               bm.VerticalResolution != (int)bm.VerticalResolution))
            {

                bm.SetResolution((int)(bm.HorizontalResolution + 0.5f),
                    (int)(bm.VerticalResolution + 0.5f));
            }
            bm.SetResolution((int)(bm.HorizontalResolution + 0.5f),
                (int)(bm.VerticalResolution + 0.5f));
            return bm;
        }

    }
}