using System;
using System.Threading.Tasks;
using AngleSharp;
using System.Linq;
using System.IO;
public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Argument Error");
            return;
        }

        var price = GetThomannGenelec7050PriceAsync().GetAwaiter().GetResult();
        Console.WriteLine(price);

        AppendFile(args[0], price);
    }

    private static void AppendFile(string path, int price)
    {
        if (String.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path can not be null");
        }
        path = Path.GetFullPath(path);
        File.AppendAllText(path, $"{DateTime.Now.ToString("yyy/MM/dd-HH:mm:ss")},{price}\n");
    }

    private static async ValueTask<int> GetThomannGenelec7050PriceAsync()
    {
        var config = AngleSharp.Configuration.Default.WithDefaultLoader();
        var browser = BrowsingContext.New(config);

        // 這邊用的型別是 AngleSharp 提供的 AngleSharp.Dom.Url
        var url = new AngleSharp.Dom.Url("https://www.thomann.de/gb/genelec_7050_cpm.htm");

        // 使用 OpenAsync 來打開網頁抓回內容
        var document = await browser.OpenAsync(url);
        var inner = document.QuerySelectorAll("body > div.thomann-page.thomann-page-en.fx-page > div > div.thomann-content.thomann-content-module-prod.thomann-content-route-main > div > div > div.fx-container.fx-container--with-margin.fx-product-orderable.product-main-content.fx-content-product-grid__col > div > div.fx-grid__col.fx-col--12.fx-col--lg-4 > div > div.product-price-box > div.fx-content-product__sidebar.price-and-availability > div.price-wrapper > div")
        .First();
        var priceSymbolHtml = inner.FirstElementChild.OuterHtml;
        var price = inner.InnerHtml.Replace(priceSymbolHtml, string.Empty).Trim();
        return int.Parse(price);
    }
}