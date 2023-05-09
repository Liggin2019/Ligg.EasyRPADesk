
using Ligg.Infrastructure.DataModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Ligg.Infrastructure.Helpers
{
    public static partial class ImageHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        //*get
        public static Image GetImageFromFile(string filePath, bool check = false)
        {
            if (check) FileHelper.CheckPathExistence(filePath);
            else
            {
                if (!File.Exists(filePath))
                    return null;
            }
            try
            {
                //following will popup error "参数无效"
                //因为后续使用时已经结束资源了
                //using (Image image = Image.FromFile(filePath))
                //{
                //    return image;
                //}
                Image image = Image.FromFile(filePath);
                return image;
            }
            //catch (OutOfMemoryException ex)
            catch (Exception ex)
            {
                //The file does not have a valid image format.
                //-or- GDI+ does not support the pixel format of the file
                if (check) throw new ArgumentException(_typeFullName + ".GetImageFromFile error: " + "Out Of memory or file is not image, filePath=" + filePath);//OutOfMemoryException
                else return null;
            }
        }



    }


}
