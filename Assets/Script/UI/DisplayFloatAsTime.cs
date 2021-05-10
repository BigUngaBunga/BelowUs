namespace BelowUs
{
    public class DisplayFloatAsTime : DisplayFloatNbr
    {
        protected override void UpdateTextValues() => text.text = enableMaximum ? FormatAsTime(resource.CurrentValue) + separator + resource.MaximumValue.Value : FormatAsTime(resource.CurrentValue);

        public override void HandleResourceChanged(float currentValue, float maxValue) =>
             text.text = enableMaximum ? FormatAsTime(currentValue) + separator + FormatAsTime(maxValue) : FormatAsTime(currentValue).ToString();

        private string FormatAsTime(float time)
        {
            float seconds = time % 60;
            float minutes = (time - seconds) / 60;
            return minutes > 0 ? $"{minutes}m {GetRounded(seconds)}s" : $"{GetRounded(seconds)}s";
        }
    }
}

