using System.IO;

namespace Zero.IO
{
    public static class FileHelper
    {
        /// <summary>
        /// 检查删除存在的文件
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        public static void DeleteIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
