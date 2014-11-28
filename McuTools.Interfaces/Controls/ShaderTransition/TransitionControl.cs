using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Expression.Media.Effects;

namespace McuTools.Interfaces.Controls.ShaderTransition
{
    public class TransitionControl : ContentControl
    {
        private ContentPresenter _contentPresenter;

        static TransitionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitionControl), new FrameworkPropertyMetadata(typeof(TransitionControl)));

            ContentProperty.OverrideMetadata(
                typeof(TransitionControl), new FrameworkPropertyMetadata(OnContentPropertyChanged));
        }

        #region ContentTransitionSelector

        public TransitionSelector ContentTransitionSelector
        {
            get { return (TransitionSelector)GetValue(ContentTransitionSelectorProperty); }
            set { SetValue(ContentTransitionSelectorProperty, value); }
        }

        public static readonly DependencyProperty ContentTransitionSelectorProperty =
            DependencyProperty.Register("ContentTransitionSelector", typeof(TransitionSelector),
                                        typeof(TransitionControl), new UIPropertyMetadata(null));

        #endregion

        #region Duration

        /// <summary>
        /// Gets or sets transition duration.
        /// </summary>
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TransitionControl),
                                        new UIPropertyMetadata(TimeSpan.FromSeconds(1)));

        #endregion

        #region EasingFunction

        /// <summary>
        /// Gets or sets <see cref="IEasingFunction"/> instance.
        /// </summary>
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(TransitionControl),
                                        new UIPropertyMetadata(null));

        #endregion

        #region EnableTransitions

        public bool EnableTransitions
        {
            get { return (bool) GetValue(EnableTransitionsProperty); }
            set { SetValue(EnableTransitionsProperty, value); }
        }

        public static readonly DependencyProperty EnableTransitionsProperty =
            DependencyProperty.Register("EnableTransitions", typeof (bool), typeof (TransitionControl),
                                        new UIPropertyMetadata(true));

        #endregion
        
        #region Transition animation

        private static void OnContentPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
        {
            var oldContent = args.OldValue;
            var newContent = args.NewValue;

            if (oldContent != null && newContent != null)
            {
                var transitionControl = (TransitionControl) dp;
                transitionControl.AnimateContent(oldContent, newContent);
            }
            else
            {
                var transitionControl = (TransitionControl) dp;
                transitionControl.AnimateContent(newContent);
            }
        }

        private void AnimateContent(object content)
        {
            XamlHelper.ExecuteOnLoaded(this, () =>
            {
                _contentPresenter.Content = content;
            });
        }

        private void AnimateContent(object oldContent, object newContent)
        {
            var oldContentVisual = GetVisualChild();
            var tier = (RenderCapability.Tier >> 16);

            // if we dont have a selector, or the visual content is not a FE, do not animate
            if (EnableTransitions == false || ContentTransitionSelector == null || oldContentVisual == null || tier < 2)
            {
                SetNonVisualChild(newContent);
                return;
            }
            
            // create the transition
            TransitionEffect transitionEffect = ContentTransitionSelector.GetTransition(oldContent, newContent, this);
            if (transitionEffect == null)
            {
                throw new InvalidOperationException("Returned transition effect is null.");
            }

            // create the animation
            DoubleAnimation da = new DoubleAnimation(0.0, 1.0, new Duration(Duration), FillBehavior.HoldEnd);
            da.Completed += delegate
            {
                ApplyEffect(null);
            };
            if (EasingFunction != null)
            {
                da.EasingFunction = EasingFunction;
            }
            else
            {
                da.AccelerationRatio = 0.5;
                da.DecelerationRatio = 0.5;
            }
            transitionEffect.BeginAnimation(TransitionEffect.ProgressProperty, da);

            VisualBrush oldVisualBrush = new VisualBrush(oldContentVisual);
            transitionEffect.OldImage = oldVisualBrush;

            SetNonVisualChild(newContent);
            ApplyEffect(transitionEffect);
        }

        private FrameworkElement GetVisualChild()
        {
            try
            {
                var visualChild = VisualTreeHelper.GetChild(_contentPresenter, 0) as FrameworkElement;
                return visualChild;
            }
            catch (ArgumentOutOfRangeException) { return null; }
        }

        private void SetNonVisualChild(object content)
        {
            _contentPresenter.Content = content;
        }

        private void ApplyEffect(TransitionEffect effect)
        {
            _contentPresenter.Effect = effect;
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _contentPresenter = (ContentPresenter)Template.FindName("PART_ContentHost", this);
        }

        #endregion
    }
}