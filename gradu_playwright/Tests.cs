using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using gradu_playwright.CommonFunction;
using gradu_playwright.Testing;

namespace gradu_playwright.Tests;

    public class Tests : BaseSetup
    {

        [Test]
        //First test: stay on coffee-cart page, select Flat White, checkout and check that text "Thanks for your purchase" appears
        public async Task OrderFlatWhite()
        {
            var coffeeActions = new CoffeeActions(Page);
            await coffeeActions.AddCoffeeToCart("Flat_White");
            await coffeeActions.GoToCartPage();
            await coffeeActions.OpenCheckout();
            await coffeeActions.FillCheckoutForm("Niko Leppanen", "thesis@gradu.fi");
            await coffeeActions.SubmitCheckout();
            await coffeeActions.CheckPurchaseSuccessMessageVisible();
        }

        [Test]
        //Second test: add Cafe Latte to cart twice, go to cart page and verify that the total unit is 2
        public async Task AddCafeLatteTwice()
        {
            var coffeeActions = new CoffeeActions(Page);
            await coffeeActions.AddCoffeeToCart("Cafe_Latte");
            await coffeeActions.GoToCartPage();
            await coffeeActions.PlusOneCoffeeInCart("Cafe Latte");
            var units = await coffeeActions.GetTotalUnits("Cafe Latte");
            Assert.That(units, Is.EqualTo(2));
        }

        
        //Third test: add three Cappucinos to cart, get discount promotion and accept it, go to cart page and verify that discounted mocha price is correct
        [Test]
        public async Task CappucinoPromotionAccepted()
        {
            var coffeeActions = new CoffeeActions(Page);
            await coffeeActions.AddCoffeeToCart("Cappuccino");
            await coffeeActions.AddCoffeeToCart("Cappuccino");
            await coffeeActions.AddCoffeeToCart("Cappuccino");
            await coffeeActions.AcceptPromotion();
            await coffeeActions.GoToCartPage();
            var unitPrice = await coffeeActions.GetUnitPrice("(Discounted) Mocha");
            Assert.That(unitPrice, Is.EqualTo(4.00m));
        }

        [Test]
        //Fourth test: broken test case: add cappucino, go to cart page and verify that there are two cappucinos in the cart (this will fail)
        public async Task BrokenTestCaseExample()
        {
            var coffeeActions = new CoffeeActions(Page);
            await coffeeActions.AddCoffeeToCart("Cappuccino");
            await coffeeActions.GoToCartPage();
            var units = await coffeeActions.GetTotalUnits("Cappuccino");
            Assert.That(units, Is.EqualTo(2));
        }
    }
