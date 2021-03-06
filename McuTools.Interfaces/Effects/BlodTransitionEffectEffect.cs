//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Expression.Media.Effects;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;


namespace McuTools.Interfaces.Effects
{

    /// <summary>A transition effect </summary>
    public class BlodTransitionEffectEffect : TransitionEffect
    {
        public static readonly DependencyProperty RandomSeedProperty = DependencyProperty.Register("RandomSeed", typeof(double), typeof(BlodTransitionEffectEffect), new UIPropertyMetadata(((double)(0.3D)), PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty Texture2Property = ShaderEffect.RegisterPixelShaderSamplerProperty("Texture2", typeof(BlodTransitionEffectEffect), 1);
        public static readonly DependencyProperty CloudInputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("CloudInput", typeof(BlodTransitionEffectEffect), 2);
        public BlodTransitionEffectEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("/MCUTools.Interfaces;component/Effects/BlodTransitionEffectEffect.ps", UriKind.Relative);
            this.PixelShader = pixelShader;

            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(ProgressProperty);
            this.UpdateShaderValue(RandomSeedProperty);
            this.UpdateShaderValue(Texture2Property);
            this.UpdateShaderValue(CloudInputProperty);
            Random r = new Random();
            this.RandomSeed = r.NextDouble();
        }
        
        public double RandomSeed
        {
            get
            {
                return ((double)(this.GetValue(RandomSeedProperty)));
            }
            set
            {
                this.SetValue(RandomSeedProperty, value);
            }
        }
        public Brush Texture2
        {
            get
            {
                return ((Brush)(this.GetValue(Texture2Property)));
            }
            set
            {
                this.SetValue(Texture2Property, value);
            }
        }
        /// <summary>Another texture passed to the shader to determine drip pattern.</summary>
        public Brush CloudInput
        {
            get
            {
                return ((Brush)(this.GetValue(CloudInputProperty)));
            }
            set
            {
                this.SetValue(CloudInputProperty, value);
            }
        }

        protected override TransitionEffect DeepCopy()
        {
            return new BlodTransitionEffectEffect();
        }
    }
}
