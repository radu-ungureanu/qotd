using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataSourceProvider
{
    public sealed class DataService
    {
        public async Task<Quote> GetQuoteOfTheDayAsync()
        {
            string content = "The computer is not connected to the internet. Try again later.", author = "";

            var httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync("http://www.brainyquote.com/link/quotebr.js");

            if (httpResponse.IsSuccessStatusCode)
            {
                var htmlContent = await httpResponse.Content.ReadAsStringAsync();
                var temp1 = htmlContent.Substring(htmlContent.IndexOf("\n") + 1);
                var temp2 = temp1.Substring(temp1.IndexOf("\n") + 1);
                var contentChunk = temp2.Substring(0, temp2.IndexOf("\n"));
                content = contentChunk.Substring(contentChunk.IndexOf("\"") + 1, contentChunk.IndexOf("<") - contentChunk.IndexOf("\"") - 1);
                var temp3 = temp2.Remove(0, contentChunk.Length + 1);
                var authorChunk = temp3.Substring(0, temp3.IndexOf("\n"));
                var temp4 = authorChunk.Remove(0, authorChunk.IndexOf(">") + 1);
                author = temp4.Remove(temp4.IndexOf("<"));
            }

            return new Quote { Content = content, Author = author };
        }
    }
}
