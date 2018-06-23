using System;
using System.Windows;
using System.Windows.Controls;

namespace VisualisationHeuristique.View.Resources
{
    /// <summary>
    /// Logique d'interaction pour IntSpinner.xaml
    /// </summary>
    public partial class IntSpinner : Border
    {
        #region Static fields and methods
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(IntSpinner), new PropertyMetadata(0, ValueChangedCallback));

        private static void ValueChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((IntSpinner)obj).IntTextBox.Text = e.NewValue.ToString();
        }
        #endregion

        /// <summary>
        /// Obtient ou definit la valeur entiere actuelle dans le spinner
        /// </summary>
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public IntSpinner()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Repositionne le contenu du TextBox quand il perd le focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IntTextBox.Text = GetValue(ValueProperty).ToString();
            IntTextBox.ScrollToHorizontalOffset(double.MinValue);
        }

        /// <summary>
        /// Appele quand le text change.
        /// Verifie que la valeur saisie est un entier: si ca n'est pas le cas, ignore le changement
        /// </summary>
        /// <param name="sender">Inutilise</param>
        /// <param name="e">Inutilise</param>
        private void IntTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int intValue = 0;
            int current = Value;
            if (string.IsNullOrEmpty(IntTextBox.Text) || int.TryParse(IntTextBox.Text.Replace(" ", ""), out intValue))
            {
                if (current != intValue)
                    Value = intValue;
            }
            else
            {
                int saveCursor = Math.Max(0, IntTextBox.SelectionStart - 1);
                IntTextBox.Text = current.ToString();
                IntTextBox.SelectionStart = saveCursor;
            }
        }

        /// <summary>
        /// Value += 1
        /// </summary>
        /// <param name="sender">Inutilise</param>
        /// <param name="e">Inutilise</param>
        private void ValueUp(object sender, RoutedEventArgs e)
        {
            Value += 1;
        }

        /// <summary>
        /// Value -= 1
        /// </summary>
        /// <param name="sender">Inutilise</param>
        /// <param name="e">Inutilise</param>
        private void ValueDown(object sender, RoutedEventArgs e)
        {
            int current = Value;
            if (current > 0)
                Value = current - 1;
            else
                Value = 0;
        }
    }
}