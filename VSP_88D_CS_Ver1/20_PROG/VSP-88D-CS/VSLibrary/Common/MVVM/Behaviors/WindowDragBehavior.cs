using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace VSLibrary.Common.MVVM.Behaviors;

/// <summary>
/// A behavior that enables drag functionality for a window using UI interactions.
/// </summary>
public class WindowDragBehavior : Behavior<UIElement>
{
    /// <summary>
    /// Called when the behavior is attached to the target <see cref="UIElement"/>.
    /// Subscribes to the mouse left button down event.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    /// <summary>
    /// Called when the behavior is detached from the target <see cref="UIElement"/>.
    /// Unsubscribes from the mouse left button down event.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
    }

    /// <summary>
    /// Called when the left mouse button is pressed.
    /// Finds the window containing the current UIElement and initiates a drag move operation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The mouse button event arguments.</param>
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Window? window = Window.GetWindow(AssociatedObject);
        if (e.ButtonState == MouseButtonState.Pressed)
            window?.DragMove();
    }
}
