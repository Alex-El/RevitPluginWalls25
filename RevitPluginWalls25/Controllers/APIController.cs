using Microsoft.Win32;
using RevitPluginWalls.Abstract;
using RevitPluginWalls.CommandData;
using RevitPluginWalls.Models;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Windows.Graphics.Printing3D;


namespace RevitPluginWalls.Controllers
{
    internal class APIController : IAPIController
    {
        public bool RequestData(CommandDataStorage data, out string result)
        {
            result = "";

            string baseurl1 = $"{data.Url}/{data.ProcjectId}/levels";
            var client1 = new HttpClient { BaseAddress = new Uri(baseurl1) };
            client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes($"{data.Login}:{data.Password}")));

            var response1 = client1.GetAsync(baseurl1).Result;

            if (!response1.IsSuccessStatusCode)
            {
                result = $"Запрос уровней не успешен. Код ответа: {response1.StatusCode}";
                return false;
            }

            string responseBody1 = response1.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(responseBody1))
            {
                result = $"Не удалось получить уровни.";
                return false;
            }

            try
            {
                data.Levels = JsonSerializer.Deserialize<List<LevelDTOModel>>(responseBody1);
            }
            catch
            {
                result = $"Не удалось распознать уровни.";
                return false;
            }

            string baseurl2 = $"{data.Url}/{data.ProcjectId}/walls";
            var client2 = new HttpClient { BaseAddress = new Uri(baseurl2) };
            client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        System.Text.ASCIIEncoding.ASCII.GetBytes($"{data.Login}:{data.Password}")));

            var response2 = client2.GetAsync(baseurl2).Result;

            if (!response2.IsSuccessStatusCode)
            {
                result = $"Запрос стен не успешен. Код ответа: {response2.StatusCode}";
                return false;
            }

            string responseBody2 = response2.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(responseBody2))
            {
                result = $"Не удалось получить стены.";
                return false;
            }

            try
            {
                data.Walls = JsonSerializer.Deserialize<List<WallDTOModel>>(responseBody2);
            }
            catch
            {
                result = $"Не удалось распознать стены.";
                return false;
            }

            return true;


            //try
            //{
            //    data.Levels = ReadAndParseFile<List<LevelDTOModel>>();
            //    data.Walls = ReadAndParseFile<List<WallDTOModel>>();

            //    result = "";
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    result = "Load data error: " + ex.Message;
            //    return false;
            //}
        }

        T ReadAndParseFile<T>() where T : new()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                string fileText = File.ReadAllText(openFileDialog.FileName);
                T model = JsonSerializer.Deserialize<T>(fileText);

                return model;
            }
            return new T();
        }
    }
}
