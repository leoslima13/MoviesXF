using System;
using FFImageLoading.Forms;
using Movies.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace Movies.Controls
{
    public class MovieBackdropImage : Grid
    {
        private readonly Image _image = new Image
        {
            Aspect = Aspect.AspectFill
        };

        private readonly CachedImage _cachedImage = new CachedImage
        {
            DownsampleToViewSize = true,
            Aspect = Aspect.AspectFill
        };
        
        public MovieBackdropImage()
        {
            Padding = new Thickness(0);
            IsClippedToBounds = true;
            
            _cachedImage.HeightRequest = HeightRequest;
            _cachedImage.WidthRequest = WidthRequest;

            var pancakeView = new PancakeView
            {
                CornerRadius = 16,
                IsClippedToBounds = true,
                Padding = 1,
                Border = new Border
                {
                    Color = Colors.PrimaryColor,
                    Thickness = 2
                },
                Content = _cachedImage
            };

            Children.Add(pancakeView);
        }

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(MovieBackdropImage),
                propertyChanged: OnSourcePropertyChanged);

        public ImageSource Source
        {
            get => (ImageSource) GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not MovieBackdropImage movieBackdropImage) return;
            movieBackdropImage._cachedImage.Source = (ImageSource) newValue;
        }
    }
}