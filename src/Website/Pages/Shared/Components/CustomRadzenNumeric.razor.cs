using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;

namespace Website.Pages.Shared.Components
{
    public partial class CustomRadzenNumeric<TValue> : RadzenNumeric<TValue>
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        protected Queue<Action> queue = new();
        
        private int maxIntegers;
        private int maxDecimals;
        private TValue? maxValue;
        private bool unsigned = true;

        private int _js_maxIntegers;
        private int _js_maxDecimals;
        private bool _js_unsigned = true;

        #pragma warning disable BL0007
        [Parameter]
        public int MaxIntegers { get => maxIntegers; set => SetMaxIntegers(value); }

        [Parameter]
        public bool Unsigned  { get => unsigned; set => SetUnsigned(value); }

        [Parameter]
        public int MaxDecimals
        {
            get => maxDecimals;
            set
            {
                if(maxDecimals != value)
                    SetMaxDecimals(value);
            }
        } 

        [Parameter]
        public TValue? MaxValue { get => maxValue; set => SetMaxValue(value); }
        #pragma warning restore BL0007

        protected IJSObjectReference module = null!;

        protected override async Task OnInitializedAsync()
        {
            await RefreshSettings(maxIntegers, maxDecimals, unsigned);
            await base.OnInitializedAsync();
        }

        public async Task RefreshSettings(int maxIntegers, int maxDecimals, bool unsigned = true)
        {
            
            module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/custom-radzen-numeric.js");
            if (module is not null)
            {
                await module.InvokeAsync<bool>("initializeInput", GetId(), false);
                SetMaxIntegers(maxIntegers);
                SetMaxDecimals(maxDecimals);
                SetUnsigned(unsigned);
                SetMaxValue(maxValue);
            }
            while (queue.Count > 0)
            {
                var action = queue.Dequeue();
                action.Invoke();
            }
        }

        private async void SetUnsigned(bool value)
        {
            unsigned = value;
            if (module is null)
            {
                queue.Enqueue(() => SetUnsigned(value));
                return;
            }

            try
            {
                var success = await module.InvokeAsync<bool>("setUnsigned", GetId(), value);
                if (success)
                {
                    _js_unsigned = value;
                }

            }
            catch (Exception)
            {


            }
}

        private async void SetMaxValue(TValue? value)
        {
            if (value is null)
                return;

            if (module is null)
            {
                queue.Enqueue(() => SetMaxValue(value));
                return;
            }

            if (MaxValue != null && MaxValue.Equals(value))
            {
                return;
            }

            var success = await module.InvokeAsync<bool>("setMaxValue", GetId(), value);
            if (success)
            {
                maxValue = value;
            }
        }

        private async void SetMaxDecimals(int value)
        {
            maxDecimals = value;
            if (module is null)
            {
                queue.Enqueue(() => SetMaxDecimals(value));
                return;
            }

            try
            {
                var success = await module.InvokeAsync<bool>("setMaxDecimals", GetId(), value);
                if (success)
                {
                    success = await module.InvokeAsync<bool>("setAllowsDecimals", GetId(), value > 0);
                }

                if (success)
                {
                    _js_maxDecimals = value;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async void SetMaxIntegers(int value)
        {
            maxIntegers = value;

            if (module is null)
            {
                queue.Enqueue(() => SetMaxIntegers(value));
                return;
            }

            try
            {
                var success = await module.InvokeAsync<bool>("setMaxIntegers", GetId(), value);
                if (success)
                {
                    _js_maxIntegers = value;
                }
            }
            catch (Exception)
            {

               
            }
            
        }
    }
}
