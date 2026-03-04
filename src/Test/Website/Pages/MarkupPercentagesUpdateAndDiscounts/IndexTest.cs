using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Bunit;
using IndexPage = Website.Pages.MarkupPercentagesUpdateAndDiscounts.Index;
using Repository.Interface;
using DataTransferObject;
using Business.Interface;
using DataAccess.Entities;
using Business.MarkupPercentagesUpdateAndDiscounts;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen.Blazor;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Components;
using Website.Pages.MarkupPercentagesUpdateAndDiscounts;

namespace Test.Website.Pages.MarkupPercentagesUpdateAndDiscounts
{
    public class IndexTest
    {
        private readonly NotificationService _notificationService;
        private readonly DialogService _dialogService;

        public IndexTest(NotificationService notificationService, DialogService dialogService)
        {
            _notificationService = notificationService;
            _dialogService = dialogService;
        }

        [Fact]
        public void Should_Not_Allow_Set_NegativeValues()
        {
            // Arrange 
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddTransient((x) => new Mock<ICombosBL<GenericComboDTO>>().Object);
            ctx.Services.AddTransient((x) => new Mock<ICategoriasComercialesBL<CategoriasComercialesDTO>>().Object);
            ctx.Services.AddTransient((x) => new Mock<IMarkupPercentagesUpdateAndDiscountsBL<DataGridItemDTO>>().Object);
            ctx.Services.AddTransient((x) => new Mock<IParametrosBL<ParametrosDTO>>().Object);
            ctx.Services.AddTransient((x) => _notificationService);
            ctx.Services.AddTransient((x) => _dialogService);
            var cut = ctx.RenderComponent<IndexPage>();

            // Act
            var markupInputComponent = cut.FindComponents<RadzenNumeric<decimal?>>()
                .Where(x => x.Instance.Name == "markup")
                .FirstOrDefault();

            var markupVhInputComponent = cut.FindComponents<RadzenNumeric<decimal?>>()
                .Where(x => x.Instance.Name == "markupvh")
                .FirstOrDefault();

            var markupInput = markupInputComponent.Find("input");
            var markupVhInput = markupVhInputComponent.Find("input");

            markupInput.Change(new ChangeEventArgs()
            {
                Value = -11.12m
            });

            markupVhInput.Change(new ChangeEventArgs()
            {
                Value = -68.84m
            });

            // Assert
            Assert.Equal(11.12m, (decimal?)markupInputComponent.Instance.GetValue());
            Assert.Equal(68.84m, (decimal?)markupVhInputComponent.Instance.GetValue());
        }
    }
}
