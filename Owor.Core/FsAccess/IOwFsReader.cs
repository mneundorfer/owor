namespace Owor.Core.FsAccess
{

    internal interface IOwFsReader
    {

        /// <summary>
        /// Returns the contents of the file within
        /// the device directory - alaways relative to
        /// the configured base path
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GetFileContent(string deviceId, string fileName);

        /// <summary>
        /// Returns an array of device ids found 
        /// in the configured ow base path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string[] GetOwDeviceIds();

    }

}