using Microsoft.JSInterop;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Website.Extensions
{
    public static class IJSRuntimeExtensions
    {
        public async static Task NavigateToNewTab(this IJSRuntime jsRuntime, string url)
        {
            await jsRuntime.InvokeVoidAsync("open", url, "_blank");
        }

        public async static Task SaveAsAsync(this IJSRuntime jsRuntime, string fileName, byte[] file)
        {
            await jsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(file));
        }

        public async static Task ScrollToElement(this IJSRuntime jsRuntime, string elementId)
        {
            var jsonOptions = JsonConvert.SerializeObject(new ScrollToElementsOptions(), new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            await jsRuntime.InvokeVoidAsync("scrollToElement", elementId, jsonOptions);
        }

        public async static Task ScrollToElement(this IJSRuntime jsRuntime, string elementId, ScrollToElementsOptions options)
        {
            var jsonOptions = JsonConvert.SerializeObject(options, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            await jsRuntime.InvokeVoidAsync("scrollToElement", elementId, jsonOptions);
        }

        public async static Task WaitForElementExists(this IJSRuntime jsRuntime, string selector)
        {
            await jsRuntime.InvokeVoidAsync("waitForElementExists", selector);
        }
    }
    public class ScrollToElementsOptions
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public BlockMode Block { get; set; } = BlockMode.Start;

        [JsonConverter(typeof(StringEnumConverter))]
        public BehaviorMode Behavior { get; set; } = BehaviorMode.Auto;

        [JsonConverter(typeof(StringEnumConverter))]
        public InlineMode Inline { get; set; } = InlineMode.Nearest;

        public enum BlockMode
        {
            [System.Runtime.Serialization.EnumMember(Value = "start")]
            Start,
            [System.Runtime.Serialization.EnumMember(Value = "center")]
            Center,
            [System.Runtime.Serialization.EnumMember(Value = "end")]
            End,
            [System.Runtime.Serialization.EnumMember(Value = "nearest")]
            Nearest
        }

        public enum InlineMode
        {
            [System.Runtime.Serialization.EnumMember(Value = "start")]
            Start,
            [System.Runtime.Serialization.EnumMember(Value = "center")]
            Center,
            [System.Runtime.Serialization.EnumMember(Value = "end")]
            End,
            [System.Runtime.Serialization.EnumMember(Value = "nearest")]
            Nearest
        }

        public enum BehaviorMode
        {
            [System.Runtime.Serialization.EnumMember(Value = "auto")]
            Auto,
            [System.Runtime.Serialization.EnumMember(Value = "smooth")]
            Smooth
        }
    }
}
