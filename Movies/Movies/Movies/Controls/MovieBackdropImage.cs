using FFImageLoading.Forms;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace Movies.Controls
{
    public class MovieBackdropImage : Grid
    {
        private readonly CachedImage _image = new CachedImage
        {
            Aspect = Aspect.AspectFill,
            DownsampleToViewSize = true
        };

        private readonly PancakeView _pancakeView = new PancakeView
        {
            Border = new Border()
        };

        public MovieBackdropImage()
        {
            IsClippedToBounds = true;
            
            _pancakeView.CornerRadius = CornerRadius;
            _pancakeView.IsClippedToBounds = true;
            _pancakeView.Padding = ContentPadding;
            _pancakeView.Border.Color = BorderColor;
            _pancakeView.Border.Thickness = BorderThickness;
            _pancakeView.Content = _image;
            

            Children.Add(_pancakeView);
        }

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(MovieBackdropImage),
                propertyChanged: OnSourcePropertyChanged);
        
        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(MovieBackdropImage),
                propertyChanged: OnCornerRadiusPropertyChanged);
        
        public static readonly BindableProperty BorderThicknessProperty =
            BindableProperty.Create(nameof(BorderThickness), typeof(int), typeof(MovieBackdropImage),
                propertyChanged: OnBorderThicknessPropertyChanged);
        
        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(MovieBackdropImage),
                propertyChanged: OnBorderColorPropertyChanged);
        
        public static readonly BindableProperty ContentPaddingProperty =
            BindableProperty.Create(nameof(ContentPadding), typeof(Thickness), typeof(MovieBackdropImage),
                propertyChanged: OnContentPaddingPropertyChanged);

        public ImageSource Source
        {
            get => (ImageSource) GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        
        public CornerRadius CornerRadius
        {
            get => (CornerRadius) GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        
        public int BorderThickness
        {
            get => (int) GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }
        
        public Color BorderColor
        {
            get => (Color) GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }
        
        public Thickness ContentPadding
        {
            get => (Thickness) GetValue(ContentPaddingProperty);
            set => SetValue(ContentPaddingProperty, value);
        }

        private static void OnSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not MovieBackdropImage movieBackdropImage) return;
            movieBackdropImage._image.Source = (ImageSource) newValue;
        }
        
        private static void OnCornerRadiusPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not MovieBackdropImage movieBackdropImage) return;
            movieBackdropImage._pancakeView.CornerRadius = (CornerRadius) newValue;
        }
        
        private static void OnBorderThicknessPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not MovieBackdropImage movieBackdropImage) return;
            movieBackdropImage._pancakeView.Border.Thickness = (int) newValue;
        }
        
        private static void OnBorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not MovieBackdropImage movieBackdropImage) return;
            movieBackdropImage._pancakeView.Border.Color = (Color) newValue;
        }
        
        private static void OnContentPaddingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not MovieBackdropImage movieBackdropImage) return;
            movieBackdropImage._pancakeView.Padding = (Thickness) newValue;
        }
    }
}