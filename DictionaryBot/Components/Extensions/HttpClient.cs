namespace DictionaryBot.Components.Extensions
{
    public static class HttpClientExtension
    {
        // Credits to Tony (https://stackoverflow.com/questions/45711428/download-file-with-webclient-or-httpclient)
        public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string FileName)
        {
            using (var s = await client.GetStreamAsync(uri))
            {
                using (var fs = new FileStream(FileName, FileMode.CreateNew))
                {
                    await s.CopyToAsync(fs);
                }
            }
        }
    }
}
