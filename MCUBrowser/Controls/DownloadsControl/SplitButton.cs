/********************************************************************************
 *    Project   : Awesomium.NET (TabbedWPFSample)
 *    File      : SplitButton.cs
 *    Version   : 1.7.0.0 
 *    Date      : 3/10/2013
 *    Author    : Perikles C. Stephanidis (perikles@awesomium.com)
 *    Copyright : ©2013 Awesomium Technologies LLC
 *    
 *    This code is provided "AS IS" and for demonstration purposes only,
 *    without warranty of any kind.
 *     
 *-------------------------------------------------------------------------------
 *
 *    Notes     :
 *
 *    
 *    
 *    
 ********************************************************************************/

#region Using
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#endregion

namespace TabbedWPFSample
{
    [TemplatePart( Name = PART_ContentPresenter, Type = typeof( ContentPresenter ) )]
    [TemplatePart( Name = PART_Popup, Type = typeof( Popup ) )]
    [TemplatePart( Name = PART_ActionButton, Type = typeof( Button ) )]
    class SplitButton : ContentControl, ICommandSource
    {
        #region Fields
        private const string PART_DropDownButton = "PART_DropDownButton";
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_Popup = "PART_Popup";
        private const string PART_ActionButton = "PART_ActionButton";


        private ContentPresenter _contentPresenter;
        private Popup _popup;
        #endregion

        #region Events
        public event RoutedEventHandler Click
        {
            add { AddHandler( ClickEvent, value ); }
            remove { RemoveHandler( ClickEvent, value ); }
        }

        public static readonly RoutedEvent ClickEvent = 
            EventManager.RegisterRoutedEvent( "Click",
            RoutingStrategy.Bubble, typeof( RoutedEventHandler ),
            typeof( SplitButton ) );

        public event RoutedEventHandler Opened
        {
            add { AddHandler( OpenedEvent, value ); }
            remove { RemoveHandler( OpenedEvent, value ); }
        }

        public static readonly RoutedEvent OpenedEvent = 
            EventManager.RegisterRoutedEvent( "Opened",
            RoutingStrategy.Bubble, typeof( RoutedEventHandler ),
            typeof( SplitButton ) );

        public event RoutedEventHandler Closed
        {
            add { AddHandler( ClosedEvent, value ); }
            remove { RemoveHandler( ClosedEvent, value ); }
        }

        public static readonly RoutedEvent ClosedEvent = 
            EventManager.RegisterRoutedEvent( "Closed",
            RoutingStrategy.Bubble, typeof( RoutedEventHandler ),
            typeof( SplitButton ) );
        #endregion



        #region Ctors
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( SplitButton ), new FrameworkPropertyMetadata( typeof( SplitButton ) ) );
        }

        public SplitButton()
        {
            Keyboard.AddKeyDownHandler( this, OnKeyDown );
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler( this, OnMouseDownOutsideCapturedElement );
        }
        #endregion


        #region Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button = GetTemplateChild( PART_ActionButton ) as Button;

            _contentPresenter = GetTemplateChild( PART_ContentPresenter ) as ContentPresenter;

            if ( _popup != null )
                _popup.Opened -= Popup_Opened;

            _popup = GetTemplateChild( PART_Popup ) as Popup;

            if ( _popup != null )
                _popup.Opened += Popup_Opened;
        }

        private void CanExecuteChanged()
        {
            if ( Command != null )
            {
                RoutedCommand command = Command as RoutedCommand;

                // If a RoutedCommand.
                if ( command != null )
                    IsEnabled = command.CanExecute( CommandParameter, CommandTarget ) ? true : false;
                // If a not RoutedCommand.
                else
                    IsEnabled = Command.CanExecute( CommandParameter ) ? true : false;
            }
        }

        /// <summary>
        /// Closes the drop down.
        /// </summary>
        private void CloseDropDown( bool isFocusOnButton )
        {
            if ( IsOpen )
                IsOpen = false;
            ReleaseMouseCapture();

            if ( isFocusOnButton )
                Button.Focus();
        }

        protected virtual void OnClick()
        {
            RaiseRoutedEvent( SplitButton.ClickEvent );
            RaiseCommand();
        }

        /// <summary>
        /// Raises routed events.
        /// </summary>
        private void RaiseRoutedEvent( RoutedEvent routedEvent )
        {
            RoutedEventArgs args = new RoutedEventArgs( routedEvent, this );
            RaiseEvent( args );
        }

        /// <summary>
        /// Raises the command's Execute event.
        /// </summary>
        private void RaiseCommand()
        {
            if ( Command != null )
            {
                RoutedCommand routedCommand = Command as RoutedCommand;

                if ( routedCommand == null )
                    ( (ICommand)Command ).Execute( CommandParameter );
                else
                    routedCommand.Execute( CommandParameter, CommandTarget );
            }
        }

        /// <summary>
        /// Unhooks a command from the Command property.
        /// </summary>
        /// <param name="oldCommand">The old command.</param>
        /// <param name="newCommand">The new command.</param>
        private void UnhookCommand( ICommand oldCommand, ICommand newCommand )
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        /// <summary>
        /// Hooks up a command to the CanExecuteChnaged event handler.
        /// </summary>
        /// <param name="oldCommand">The old command.</param>
        /// <param name="newCommand">The new command.</param>
        private void HookUpCommand( ICommand oldCommand, ICommand newCommand )
        {
            EventHandler handler = new EventHandler( CanExecuteChanged );
            canExecuteChangedHandler = handler;
            if ( newCommand != null )
                newCommand.CanExecuteChanged += canExecuteChangedHandler;
        }
        #endregion

        #region Properties

        #region Button
        private ButtonBase _button;

        protected ButtonBase Button
        {
            get
            {
                return _button;
            }
            set
            {
                if ( _button != null )
                    _button.Click -= DropDownButton_Click;

                _button = value;

                if ( _button != null )
                    _button.Click += DropDownButton_Click;
            }
        }
        #endregion

        #region DropDownContent
        public object DropDownContent
        {
            get { return (object)GetValue( DropDownContentProperty ); }
            set { SetValue( DropDownContentProperty, value ); }
        }

        public static readonly DependencyProperty DropDownContentProperty = 
            DependencyProperty.Register( "DropDownContent",
            typeof( object ), typeof( SplitButton ),
            new UIPropertyMetadata( null, OnDropDownContentChanged ) );

        private static void OnDropDownContentChanged( DependencyObject o, DependencyPropertyChangedEventArgs e )
        {
            SplitButton dropDownButton = o as SplitButton;

            if ( dropDownButton != null )
                dropDownButton.OnDropDownContentChanged( (object)e.OldValue, (object)e.NewValue );
        }

        protected virtual void OnDropDownContentChanged( object oldValue, object newValue )
        {
            //
        }
        #endregion

        #region IsOpen
        public bool IsOpen
        {
            get { return (bool)GetValue( IsOpenProperty ); }
            set { SetValue( IsOpenProperty, value ); }
        }

        public static readonly DependencyProperty IsOpenProperty = 
            DependencyProperty.Register( "IsOpen",
            typeof( bool ), typeof( SplitButton ),
            new UIPropertyMetadata( false, OnIsOpenChanged ) );

        private static void OnIsOpenChanged( DependencyObject o, DependencyPropertyChangedEventArgs e )
        {
            SplitButton dropDownButton = o as SplitButton;

            if ( dropDownButton != null )
                dropDownButton.OnIsOpenChanged( (bool)e.OldValue, (bool)e.NewValue );
        }

        protected virtual void OnIsOpenChanged( bool oldValue, bool newValue )
        {
            if ( newValue )
                RaiseRoutedEvent( SplitButton.OpenedEvent );
            else
                RaiseRoutedEvent( SplitButton.ClosedEvent );
        }
        #endregion

        #region CornerRadius
        public static CornerRadius GetCornerRadius( DependencyObject obj )
        {
            return (CornerRadius)obj.GetValue( CornerRadiusProperty );
        }

        public static void SetCornerRadius( DependencyObject obj, CornerRadius value )
        {
            obj.SetValue( CornerRadiusProperty, value );
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached( "CornerRadius",
            typeof( CornerRadius ), typeof( SplitButton ),
            new UIPropertyMetadata( new CornerRadius( 0 ) ) );
        #endregion        

        #endregion

        #region Event Handlers
        private void OnKeyDown( object sender, KeyEventArgs e )
        {
            if ( !IsOpen )
            {
                if ( Utils.IsKeyModifyingPopupState( e ) )
                {
                    IsOpen = true;
                    // ContentPresenter items will get focus in Popup_Opened().
                    e.Handled = true;
                }
            }
            else
            {
                if ( Utils.IsKeyModifyingPopupState( e ) )
                {
                    CloseDropDown( true );
                    e.Handled = true;
                }
                else if ( e.Key == Key.Escape )
                {
                    CloseDropDown( true );
                    e.Handled = true;
                }
            }
        }

        private void OnMouseDownOutsideCapturedElement( object sender, MouseButtonEventArgs e )
        {
            CloseDropDown( false );
        }

        private void DropDownButton_Click( object sender, RoutedEventArgs e )
        {
            OnClick();
        }

        void CanExecuteChanged( object sender, EventArgs e )
        {
            CanExecuteChanged();
        }

        private void Popup_Opened( object sender, EventArgs e )
        {
            // Set the focus on the content of the ContentPresenter.
            if ( _contentPresenter != null )
            {
                DependencyObject o = _contentPresenter.Content as DependencyObject;
                while ( o != null && ( VisualTreeHelper.GetChildrenCount( o ) > 0 ) )
                {
                    if ( o is UIElement )
                    {
                        if ( ( (UIElement)o ).Focusable )
                            break;
                    }
                    o = VisualTreeHelper.GetChild( o, 0 );
                }

                ( (UIElement)o ).Focus();
            }
        }
        #endregion

        #region ICommandSource
        // Keeps a copy of the CanExecuteChnaged handler so it doesn't get garbage collected.
        private EventHandler canExecuteChangedHandler;

        #region Command
        [TypeConverter( typeof( CommandConverter ) )]
        public ICommand Command
        {
            get { return (ICommand)GetValue( CommandProperty ); }
            set { SetValue( CommandProperty, value ); }
        }

        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register( "Command",
            typeof( ICommand ), typeof( SplitButton ),
            new PropertyMetadata( (ICommand)null, OnCommandChanged ) );

        private static void OnCommandChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            SplitButton dropDownButton = d as SplitButton;

            if ( dropDownButton != null )
                dropDownButton.OnCommandChanged( (ICommand)e.OldValue, (ICommand)e.NewValue );
        }

        protected virtual void OnCommandChanged( ICommand oldValue, ICommand newValue )
        {
            // If old command is not null, then we need to remove the handlers.
            if ( oldValue != null )
                UnhookCommand( oldValue, newValue );

            HookUpCommand( oldValue, newValue );

            CanExecuteChanged(); //may need to call this when changing the command parameter or target.
        }
        #endregion

        #region CommandParameter
        public object CommandParameter
        {
            get { return GetValue( CommandParameterProperty ); }
            set { SetValue( CommandParameterProperty, value ); }
        }

        public static readonly DependencyProperty CommandParameterProperty = 
            DependencyProperty.Register( "CommandParameter",
            typeof( object ), typeof( SplitButton ),
            new PropertyMetadata( null ) );
        #endregion

        #region CommandTarget
        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue( CommandTargetProperty ); }
            set { SetValue( CommandTargetProperty, value ); }
        }

        public static readonly DependencyProperty CommandTargetProperty = 
            DependencyProperty.Register( "CommandTarget",
            typeof( IInputElement ), typeof( SplitButton ),
            new PropertyMetadata( null ) );
        #endregion

        #endregion
    }
}