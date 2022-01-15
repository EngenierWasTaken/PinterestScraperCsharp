// See https://aka.ms/new-console-template for more information
using PuppeteerSharp;
namespace PinterestScraper{
    class Scraper{
            static async Task Main(string[] args){
                //Uncomment la linea qui sotto per scaricare chromium
                //await new BrowserFetcher().DownloadAsync();

                //esempio di chiamata della funzione
                var kek = await Scrappy(80, "Instagram");
                for (int item = 0; item < kek.Length; item++){
                    Console.WriteLine(kek[item]);
                }
            static async Task<String[]> Scrappy(int depth, string keyword){
                // depth: all'incirca il numero di elementi da caricare
                // il termine di ricerca
                // ritorna un Array di stringhe con gli URL dell'immagine
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                    {
                        Headless = true
                    }
                );
                var page = await browser.NewPageAsync();
                var pageUrl = String.Format(
                    "https://www.pinterest.it/search/pins/?q={0}&rs=typed&term_meta%5B%5D={0}%7Ctyped", keyword
                );
                await page.GoToAsync(pageUrl);
                await page.WaitForSelectorAsync("div.Collection");
                ElementHandle[] itemsInPage = new ElementHandle[0];
                while(itemsInPage.Length <= depth){
                    await page.EvaluateExpressionAsync(
                         "window.scrollTo(0,document.body.scrollHeight)"
                        );
                    itemsInPage = await page.QuerySelectorAllAsync(
                        "div.Collection-Item"
                    );
                }
                string[] allImagesURLs = new string[itemsInPage.Length];
                for (int item = 0; item < itemsInPage.Length; item++){
                    var image = await itemsInPage[item].QuerySelectorAllAsync(
                        "img"
                        );
                    var content = await page.EvaluateFunctionAsync(
                        "e => e.src", image
                        );
                    allImagesURLs[item] =  content.ToString();
                }
               return allImagesURLs;
            }
        }
    }
}
