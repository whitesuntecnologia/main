using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.Pages.MarkupPercentagesUpdateAndDiscounts;
using Xunit;
using Business.MarkupPercentagesUpdateAndDiscounts;

namespace Test.Website.Pages.MarkupPercentagesUpdateAndDiscounts
{
    public class DialogManageDiscountsTest
    {
        private readonly DialogService _dialogService;

        public DialogManageDiscountsTest(DialogService dialogService)
        {
            _dialogService = dialogService;
        }

        [Fact]
        public void Should_Not_Allow_Set_NegativeValues()
        {
            // Arrange 
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddTransient((x) => _dialogService);
            var cut = ctx.RenderComponent<DialogManageDiscounts>(x =>
            {
                x.Add(x => x.Discounts, new List<DiscountDTO>());
            });

            // Act
            var discountInputComponent = cut.FindComponents<RadzenNumeric<decimal>>()
                .Where(x => x.Instance.Name == "discount")
                .FirstOrDefault();

            var discountVhInputComponent = cut.FindComponents<RadzenNumeric<decimal?>>()
                .Where(x => x.Instance.Name == "discountvh")
                .FirstOrDefault();

            var discountInput = discountInputComponent.Find("input");
            var discountVhInput = discountVhInputComponent.Find("input");

            discountInput.Change(new ChangeEventArgs()
            {
                Value = -11.12m
            });

            discountVhInput.Change(new ChangeEventArgs()
            {
                Value = -68.84m
            });

            // Assert
            Assert.Equal(11.12m, (decimal)discountInputComponent.Instance.GetValue());
            Assert.Equal(68.84m, (decimal?)discountVhInputComponent.Instance.GetValue());
        }
    }
}
