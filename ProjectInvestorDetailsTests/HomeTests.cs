using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using ProjectInvestorDetails.Pages;
using RichardSzalay.MockHttp;
using Xunit;

namespace ProjectInvestorDetailsTests;

public class HomeTests : TestContext
{
    [Fact]
    public async Task HomePage_ShouldDisplayInvestorData()
    {
        var httpClient = Services.AddMockHttpClient();

        httpClient
            .When("http://127.0.0.1:8000/api/investors")
            .RespondJson(new[]
            {
                new
                {
                    firm_id = 1,
                    firm_name = "ABC Investments",
                    firm_type = "Venture Capital",
                    date_added = "2024-04-03T00:00:00",
                    address = "123 Main St"
                },
                new
                {
                    firm_id = 2,
                    firm_name = "CBA Investments",
                    firm_type = "Venture Capital",
                    date_added = "2024-04-04T00:00:00",
                    address = "456 Main St"
                },
                new
                {
                    firm_id = 3,
                    firm_name = "XYZ Bank",
                    firm_type = "Bank",
                    date_added = "2024-04-05T00:00:00",
                    address = "789 Main St"
                },
            });

        var component = RenderComponent<Home>();
        component.WaitForElement("tbody");

        // Assert
        var investors = component.Instance.investors;
        Assert.NotNull(investors);
        Assert.Equal(3, investors.Length);

        Assert.Contains(investors, investor => investor.FirmName == "ABC Investments");
        Assert.Contains(investors, investor => investor.FirmName == "CBA Investments");
        Assert.Contains(investors, investor => investor.FirmName == "XYZ Bank");

        Assert.Contains(investors, investor => investor.Type == "Venture Capital");
        Assert.Contains(investors, investor => investor.Type == "Bank");

        Assert.Contains(investors, investor => investor.Address == "123 Main St");
        Assert.Contains(investors, investor => investor.Address == "456 Main St");
        Assert.Contains(investors, investor => investor.Address == "789 Main St");
    }

    [Fact]
    public async Task HomePage_ShouldDisplayLoadingIndicatorWhenDataIsNull()
    {
        var httpClient = Services.AddMockHttpClient();

        httpClient
            .When("http://127.0.0.1:8000/api/investors")
            .RespondJson(new[]
            {
                new { }
            });

        var component = RenderComponent<Home>();

        var loadingIndicator = component.Find("em");
        Assert.NotNull(loadingIndicator);
    }

    [Fact]
    public void ClickingOnInvestorRow_ShouldNavigateToCorrectUrl()
    {
        var httpClient = Services.AddMockHttpClient();
        var navMan = Services.GetRequiredService<FakeNavigationManager>();
        
        httpClient
            .When("http://127.0.0.1:8000/api/investors")
            .RespondJson(new[]
            {
                new
                {
                    firm_id = 1,
                    firm_name = "ABC Investments",
                    firm_type = "Venture Capital",
                    date_added = "2024-04-03T00:00:00",
                    address = "123 Main St"
                }
            });

        var component = RenderComponent<Home>();
        component.WaitForElement("tbody");
        var firmId = 1;
        
        var tableRow = component.Find("tbody tr:contains('ABC Investments')");
        tableRow.Click();
        
        Assert.Equal($"http://localhost/investors/{firmId}", navMan.Uri);
    }
}