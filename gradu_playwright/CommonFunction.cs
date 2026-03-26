using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Globalization;

namespace gradu_playwright.CommonFunction;

public class CoffeeActions
{
    // List of common functions to be used across tests; identify reusable methods here
    // Setup deals with going to the base URL, so no need to duplicate that here

  private readonly IPage _page;
    public CoffeeActions(IPage page)
    {
        _page = page;
    }

    private ILocator CoffeeItem (string name) => _page.Locator($"[data-test=\"{name}\"]");
    private ILocator CartPage => _page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = "Cart" });
    private ILocator PayButton => _page.Locator("[data-test=\"checkout\"]");
    private ILocator PromotionAcceptButton => _page.GetByRole(AriaRole.Button, new() { Name = "Yes, of course!" });
    // Won't be used in thesis, but included for completeness sake
    private ILocator PromotionDeclineButton => _page.GetByRole(AriaRole.Button, new() { Name = "Nawh, I'll skip."});
    private ILocator CheckoutUserName => _page.Locator("#name");
    private ILocator CheckoutEmail => _page.Locator("#email");
    //There is a good chance that this won't be used in thesis, but professional standards demand it to be included
    private ILocator PromotionCheckbox => _page.Locator("#promotion");
    private ILocator CheckoutSubmitButton => _page.GetByRole(AriaRole.Button, new() { Name = "Submit" });
    private ILocator PurchaseSuccessMessage => _page.GetByText("Thanks for your purchase", new() { Exact = false });

    // Following buttons are from cart page. Proper POM would separate them into different classes, but for thesis purposes this is sufficient

    private ILocator PlusCoffeeCartButton(string coffeeName) => _page.GetByRole(AriaRole.Button, new() { Name = $"Add one {coffeeName}" });
    private ILocator MinusCoffeeCartButton(string coffeeName) => _page.GetByRole(AriaRole.Button, new() { Name = $"Remove one {coffeeName}" });
    private ILocator DeleteCoffeeCartButton(string coffeeName) => _page.GetByRole(AriaRole.Button, new() { Name = $"Remove all {coffeeName}" });
    private ILocator CartRow(string coffeeName) =>    _page.Locator("li.list-item").Filter(new() { HasText = coffeeName });
    //There is a shadow DOM concerning discounted mocha, so we need to filter the unit description locator to only include the one that has the price in it; otherwise we would get the wrong text content and fail to parse the price
    private ILocator CartRowDesc(string coffeeName) =>    CartRow(coffeeName).Locator(".unit-desc").Filter(new() { HasText = "$" });


    //After locators, we define common functions for tests in Tests.cs to use
    public async Task AddCoffeeToCart(string coffeeName)
    {
        await CoffeeItem(coffeeName).ClickAsync();
    }
    public async Task GoToCartPage()
    {
        await CartPage.ClickAsync();
    }
    public async Task OpenCheckout()
    {
        await PayButton.ClickAsync();
    }
    // Fills the checkout form with provided name and email, without checking the promotion checkbox; see if you write example that needs it
    public async Task FillCheckoutForm(string name, string email)
    {
        await CheckoutUserName.FillAsync(name);
        await CheckoutEmail.FillAsync(email);

    }
    public async Task SubmitCheckout()
    {
        await CheckoutSubmitButton.ClickAsync();
    }
    public async Task PlusOneCoffeeInCart(string coffeeName)
    {
        await PlusCoffeeCartButton(coffeeName).ClickAsync();
    }
    public async Task MinusOneCoffeeInCart(string coffeeName)
    {
        await MinusCoffeeCartButton(coffeeName).ClickAsync();
    }
    public async Task<int> GetTotalUnits(string coffeeName)
    {
    var text = await CartRowDesc(coffeeName).InnerTextAsync();
    var match = Regex.Match(text, @"x\s*(\d+)");
    return int.Parse(match.Groups[1].Value);
    }
    public async Task<decimal> GetUnitPrice(string coffeeName)
{
    var descs = await CartRow(coffeeName).Locator(".unit-desc").AllTextContentsAsync();
    Console.WriteLine(string.Join(" | ", descs.Select(s => $"'{s}'")));

    var text = await CartRowDesc(coffeeName).InnerTextAsync();
    var match = Regex.Match(text, @"\$(\d+\.\d{2})");

    if (!match.Success)
        throw new Exception($"Could not parse unit price from '{text}' for '{coffeeName}'");

    return decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
}
    public async Task CheckPurchaseSuccessMessageVisible()
    {
        await PurchaseSuccessMessage.IsVisibleAsync();
    }
    public async Task AcceptPromotion()
    {
        await PromotionAcceptButton.ClickAsync();
    }
    // Again, won't be used in thesis but included for completeness sake
    public async Task DeclinePromotion()
    {
        await PromotionDeclineButton.ClickAsync();
    }

}