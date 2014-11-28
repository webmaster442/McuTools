using McuTools.Interfaces.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace McuTools.Interfaces.Controls
{
    /// <summary>
    /// Interaction logic for ImageButtonContent.xaml
    /// </summary>
    public partial class ImageButton : Button
    {
        public ImageButton()
        {
            InitializeComponent();
            SetColor();
        }

        public static DependencyProperty ImageTextProperty = DependencyProperty.Register("ImageText", typeof(string), typeof(ImageButton));
        public static DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButton));

        public string ImageText
        {
            get { return (string)GetValue(ImageTextProperty); }
            set { SetValue(ImageTextProperty, value); }
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public override string ToString()
        {
            return ImageText.ToString();
        }

        private void SetColor()
        {
            Storyboard sb;
            ColorAnimationUsingKeyFrames anim;
            EasingColorKeyFrame frame;

            sb = WpfHelpers.FindStoryBoard(this, "In");
            for (int i = 2; i < 4; i++)
            {
                anim = (ColorAnimationUsingKeyFrames)sb.Children[i];
                frame = (EasingColorKeyFrame)anim.KeyFrames[1];
                frame.Value = SystemColors.HotTrackBrush.Color;
            }

            for (int i = 2; i < 4; i++)
            {
                sb = WpfHelpers.FindStoryBoard(this, "Out");
                anim = (ColorAnimationUsingKeyFrames)sb.Children[i];
                frame = (EasingColorKeyFrame)anim.KeyFrames[0];
                frame.Value = SystemColors.HotTrackBrush.Color;
            }
        }

        private void ImgButton_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
