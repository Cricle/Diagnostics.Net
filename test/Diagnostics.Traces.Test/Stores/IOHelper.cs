namespace Diagnostics.Traces.Test.Stores
{
    internal static class IOHelper
    {
        public static void Init(string dirName)
        {
            var dir = new DirectoryInfo(dirName);

            if (dir.Exists)
            {
                DeleteDirectoryFiles(dir);
            }
            else
            {
                dir.Create();
            }
        }

        public static void Cleanup(string dirName)
        {
            var dir = new DirectoryInfo(dirName);
            DeleteDirectoryFiles(dir);
            try
            {
                dir.Delete();
            }
            catch (Exception) { }
        }

        private static void DeleteDirectoryFiles(DirectoryInfo info)
        {
            foreach (var item in info.GetFiles("*.*", SearchOption.AllDirectories))
            {
                try
                {
                    item.Delete();
                }
                catch (Exception) { }
            }
            foreach (var item in info.GetDirectories("*", SearchOption.AllDirectories))
            {
                try
                {
                    item.Delete();
                }
                catch (Exception) { }
            }
        }
    }
}
