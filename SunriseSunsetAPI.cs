using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AutoThemeSwitcher
{
    public class SunriseSunsetAPI
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<SunData> GetSunriseSunsetAsync(double latitude, double longitude, DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Now;
                var dateStr = targetDate.ToString("yyyy-MM-dd");
                var url = $"https://api.sunrise-sunset.org/json?lat={latitude}&lng={longitude}&date={dateStr}&formatted=0";

                Logger.Log($"Fetching sunrise/sunset data from API for {dateStr}...");

                var response = await httpClient.GetStringAsync(url);
                var apiResponse = JsonConvert.DeserializeObject<SunApiResponse>(response);

                if (apiResponse?.Status == "OK" && apiResponse.Results != null)
                {
                    var sunData = new SunData
                    {
                        Sunrise = DateTime.Parse(apiResponse.Results.Sunrise).ToLocalTime(),
                        Sunset = DateTime.Parse(apiResponse.Results.Sunset).ToLocalTime()
                    };

                    Logger.Log($"API data received - Sunrise: {sunData.Sunrise:HH:mm:ss}, Sunset: {sunData.Sunset:HH:mm:ss}");
                    return sunData;
                }

                Logger.Log($"API returned invalid status: {apiResponse?.Status}");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Log($"Error fetching sunrise/sunset data: {ex.Message}");
                return null;
            }
        }
    }

    public class SunData
    {
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
    }

    internal class SunApiResponse
    {
        [JsonProperty("results")]
        public SunResults Results { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    internal class SunResults
    {
        [JsonProperty("sunrise")]
        public string Sunrise { get; set; }

        [JsonProperty("sunset")]
        public string Sunset { get; set; }
    }
}