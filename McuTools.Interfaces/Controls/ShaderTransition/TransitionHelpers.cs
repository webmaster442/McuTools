using McuTools.Interfaces.Effects;
using Microsoft.Expression.Media.Effects;
using System;
using System.Windows;
using System.Windows.Media;
using System.Linq;

namespace McuTools.Interfaces.Controls.ShaderTransition
{
    public static class XamlHelper
    {
        public static void ExecuteOnLoaded(FrameworkElement fe, Action action)
        {
            if (fe.IsLoaded)
            {
                if (action != null)
                {
                    action();
                }
            }
            else
            {
                RoutedEventHandler handler = null;
                handler = delegate
                {
                    fe.Loaded -= handler;

                    if (action != null)
                    {
                        action();
                    }
                };

                fe.Loaded += handler;
            }
        }
    }

    public abstract class TransitionSelector
    {
        public abstract TransitionEffect GetTransition(object oldContent, object newContent, DependencyObject container);
    }

    public class TabControlTransitionSelector : TransitionSelector
    {
        private readonly Random _random = new Random();

        private TransitionEffect[] _transitions;
        private int[] _animcount;

        public TabControlTransitionSelector()
        {
            if (!RenderCapability.IsPixelShaderVersionSupported(2, 0)) return;
            _transitions = new TransitionEffect[]
            {
                new SmoothSwirlGridTransitionEffect(),
                new BlindsTransitionEffect(),
                new CircleRevealTransitionEffect(),
                new CloudRevealTransitionEffect(),
                new FadeTransitionEffect(),
                new PixelateTransitionEffect(),
                new RadialBlurTransitionEffect(),
                new RippleTransitionEffect(),
                new WaveTransitionEffect(),
                new WipeTransitionEffect(),
                new SmoothSwirlGridTransitionEffect(),
                new BlodTransitionEffectEffect(),
                new FoldTransitionEffectEffect(),
                new PivotTransitionEffectEffect(PivotTransitionEffectEffect.Direction.Left),
                new PivotTransitionEffectEffect(PivotTransitionEffectEffect.Direction.Right),
                new SlideInTransitionEffect { SlideDirection = SlideDirection.TopToBottom },
                new SlideInTransitionEffect { SlideDirection = SlideDirection.LeftToRight },
                new SlideInTransitionEffect { SlideDirection = SlideDirection.RightToLeft },
                new SlideInTransitionEffect { SlideDirection = SlideDirection.BottomToTop }
            };
            _transitions = (from i in _transitions orderby _random.Next() select i).ToArray();
            _animcount = new int[_transitions.Length];
        }

        public override TransitionEffect GetTransition(object oldContent, object newContent, DependencyObject container)
        {
            if (_transitions.Length < 1) return null;
            var index = (int)(_random.NextDouble() * _transitions.Length);

            var min = _animcount.Min();
            if (index > min)
            {
                index = Array.IndexOf(_animcount, min);
                _animcount[index]++;
            }
            return _transitions[(int)index];
        }
    }
}