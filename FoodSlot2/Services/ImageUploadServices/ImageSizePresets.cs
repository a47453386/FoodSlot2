namespace FoodSlot2.Services.ImageUploadServices
{
    public static class ImageSizePresets
    {
        public static List<ImageSizeOption> Food =>
            new()
            {
                new ImageSizeOption {Width = 256, Height = 256 }
            };
    }
}