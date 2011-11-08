using System.Windows.Controls;
using System.Windows.Interactivity;

namespace EvolutionHighwayApp.Utils
{
    public class TextBoxUpdateBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, TextChangedEventArgs args)
        {
            var bindingExpr = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpr != null) bindingExpr.UpdateSource();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }
    }
}
