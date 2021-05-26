namespace BelowUs
{
    public class DisplayFloatAsTime : DisplayFloatNbr
    {
        public override void UpdateTextValue() =>
             text.text = enableMaximum ? FormatAsTime(resource.CurrentValue) + separator + FormatAsTime(resource.MaximumValue.Value) : FormatAsTime(resource.CurrentValue).ToString();

        private string FormatAsTime(float time)
        {
            float seconds = time % 60;
            float minutes = (time - seconds) / 60;
            return minutes > 0 ? $"{minutes}m {GetRounded(seconds)}s" : $"{GetRounded(seconds)}s";
        }
    }
}

