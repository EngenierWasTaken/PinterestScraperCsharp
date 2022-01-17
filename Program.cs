// See https://aka.ms/new-console-template for more information
using PuppeteerSharp;
using System.Diagnostics;
using HtmlAgilityPack;
namespace PinterestScraper{
    class Scraper{
            static async Task Main(string[] args){
                //Uncomment la linea qui sotto per scaricare chromium
                //await new BrowserFetcher().DownloadAsync();
                
                //esempio di chiamata della funzione
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                var kek = await Scrappy(100, "Instagram");
                /*for (int item = 0; item < kek.Length; item++){
                    Console.WriteLine(kek[item]);
                }*/
                stopWatch.Stop();
                
                // Get the elapsed time as a TimeSpan value.
                Console.WriteLine("FINE PROGRAMMA");
                
                WriteTime(stopWatch);
                Console.WriteLine(kek.Length);
                Console.WriteLine(GC.GetTotalMemory(true));
                // Format and display the TimeSpan value.
                
            }
            static void WriteTime(Stopwatch stop){
                TimeSpan ts = stop.Elapsed;
                string elapsedTime = String.Format(
                    "{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10
                );
                Console.WriteLine("RunTime " + elapsedTime);
            }

            static async Task<Page> InizializePage(Browser browser){
                Stopwatch stop = new Stopwatch();
                stop.Start();
                
                var page = await browser.NewPageAsync();
                await page.SetViewportAsync(
                    new ViewPortOptions{Width = 1920, Height = 1080}
                );
                
                stop.Stop();
                Console.WriteLine("Tempo Inizializzazione PAGE: ");
                WriteTime(stop);
                Console.WriteLine(GC.GetTotalMemory(true));
                return page;
            }

            static async Task<Browser> InizializeBrowser(){
                Stopwatch stop = new Stopwatch();
                stop.Start();
                
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                    {
                        Headless = false,
                    }
                );
                
                stop.Stop();
                Console.WriteLine("Tempo INIZIALIZZAZIONE BROWSER: ");
                WriteTime(stop);
                Console.WriteLine(GC.GetTotalMemory(true));
                return browser;
            }

            static async Task<Page> SetPage(string pageUrl, Page page){
                Stopwatch stop = new Stopwatch();
                stop.Start();
                
                await page.GoToAsync(pageUrl);
                
                stop.Stop();
                Console.WriteLine("Tempo SET PAGE: ");
                WriteTime(stop);
                Console.WriteLine(GC.GetTotalMemory(true));
                return page; 
            }

            static async Task<ElementHandle[]> GetItemsInPage(
                    ElementHandle[] itemsInPage,
                    Page page, int depth
                ){
                    Stopwatch stop = new Stopwatch();
                    stop.Start();
                
                    await page.WaitForSelectorAsync("div.Collection");
                    while (itemsInPage.Length < depth){
                        await page.EvaluateExpressionAsync(
                            "window.scrollTo(0,document.body.scrollHeight);"
                        );
                        itemsInPage = await page.QuerySelectorAllAsync(
                            "div.Collection-Item");
                        Console.WriteLine(itemsInPage.Length);
                    }
                    stop.Stop();
                    Console.WriteLine("Tempo GET ITEMS: ");
                    WriteTime(stop);
                    Console.WriteLine(GC.GetTotalMemory(true));
                    return itemsInPage;
                }

            static async Task<String[]> GetAllImages(
                    ElementHandle[] itemsInPage, Page page,
                    string[] allImagesURLs){
                    Stopwatch stop = new Stopwatch();
                    stop.Start();
                    for (int item = 0; item < itemsInPage.Length; item++){
                        var image = await itemsInPage[item]
                            .QuerySelectorAllAsync(
                                "img"
                            );
                        var content = await page
                            .EvaluateFunctionAsync(
                                "e => e.src", image
                            );
                        allImagesURLs[item] =  content.ToString();
                    }
                    stop.Stop();
                    Console.WriteLine("Tempo GET IMAGES: ");
                    WriteTime(stop);
                    Console.WriteLine(GC.GetTotalMemory(true));
                    return allImagesURLs;

                }

            static async Task<String[]> Scrappy(int depth, string keyword){
                // depth: all'incirca il numero di elementi da caricare
                // il termine di ricerca
                var browser = await InizializeBrowser(); 
                var page = await InizializePage(browser);
                var pageUrl = String.Format(
                    "https://www.pinterest.com/search/pins/?q={0}&rs=typed&term_meta%5B%5D={0}%7Ctyped", keyword
                );
                page = await SetPage(pageUrl, page);
                
                ElementHandle[] itemsInPage = new ElementHandle[0];
                itemsInPage = await GetItemsInPage(
                    itemsInPage, page, depth
                );

                string[] allImagesURLs = new string[itemsInPage.Length];
                allImagesURLs = await GetAllImages(
                    itemsInPage, page, allImagesURLs
                );
                await browser.CloseAsync();
                return allImagesURLs;
            }
        }
    }
