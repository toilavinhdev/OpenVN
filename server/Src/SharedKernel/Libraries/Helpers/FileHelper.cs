﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharedKernel.Libraries
{
    public static class FileHelper
    {
        private static List<string> ImageExtensions = new List<string>()
        {
                "apng",
                "avif",
                "gif",
                "jpg",
                "jpeg",
                "jfif",
                "pjpeg",
                "pjp",
                "png",
                "svg",
                "webp",
        };

        private static List<string> VideoExtensions = new List<string>()
        {
            "m2v",
            "mpg",
            "mp2",
            "mpeg",
            "mpe",
            "mpv",
            "mp4",
            "m4p",
            "m4v",
            "mov",
        };


        public static double ConvertBytesToKilobytes(long bytes)
        {
            return 1.0 * bytes / 1024;
        }


        public static double ConvertBytesToMegabytes(long bytes)
        {
            return ConvertBytesToKilobytes(bytes) / 1024;
        }

        public static double ConvertBytesToGigabytes(long bytes)
        {
            return ConvertBytesToMegabytes(bytes) / 1024;
        }

        public static long ConvertKilobytesToBytes(long kilobytes)
        {
            return kilobytes * 1024;
        }

        public static long ConvertMegabytesToBytes(long megabytes)
        {
            return megabytes * 1024 * 1024;
        }

        public static long ConvertGigabytesToBytes(long gigabytes)
        {
            return gigabytes * 1024 * 1024 * 1024;
        }

        public static bool IsImage(IFormFile file)
        {
            if (file == null)
            {
                return false;
            }
            var ext = Path.GetExtension(file.FileName);
            return ImageExtensions.Find(extension => $".{extension}".Equals(ext.ToLower())) != null;
        }

        public static bool IsImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            var ext = Path.GetExtension(fileName);
            return ImageExtensions.Find(extension => $".{extension}".Equals(ext.ToLower())) != null;
        }

        public static bool IsVideo(IFormFile file)
        {
            if (file == null)
            {
                return false;
            }
            var ext = Path.GetExtension(file.FileName);
            return VideoExtensions.Find(extension => $".{extension}".Equals(ext.ToLower())) != null;
        }

        public static bool IsVideo(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            var ext = Path.GetExtension(fileName);
            return VideoExtensions.Find(extension => $".{extension}".Equals(ext.ToLower())) != null;
        }
    }
}
