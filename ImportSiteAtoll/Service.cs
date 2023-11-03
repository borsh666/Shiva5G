using System.Diagnostics;
using Microsoft.AspNetCore.Components.Forms;


namespace ImportSiteAtoll
{
    public class Service
    {
        public async Task<byte[]> InputFileToBytes(InputFileChangeEventArgs excel)
        {
            var file = excel.GetMultipleFiles(1)[0];

            using MemoryStream ms = new();
            await file.OpenReadStream(file.Size).CopyToAsync(ms);

            var bytes = ms.ToArray();
            PassBytesToStnInput(bytes);
            return bytes;
        }

        public void PassBytesToStnInput(byte[] bytes)
        {
            var pythonAppPath = @"D:\Projects\SV\Python\ReadBytesFromStandardInput\";
            var appName = Path.Combine(pythonAppPath, "main.py");

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = Path.Combine(pythonAppPath,"venv", "Scripts", "python.exe");
            p.StartInfo.Arguments = $"{appName}";
            p.StartInfo.RedirectStandardInput = true;
            p.Start();

            p.StandardInput.BaseStream.Write(bytes, 0, bytes.Length);
            p.StandardInput.BaseStream.Flush();
            p.StandardInput.BaseStream.Close();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }
    }
}
