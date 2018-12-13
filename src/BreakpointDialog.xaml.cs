using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace armsim
{
    /// <summary>
    /// Interaction logic for BreakpointDialog.xaml
    /// </summary>
    public partial class BreakpointDialog : Window
    {
        List<int> breakPoints;
        public BreakpointDialog(ref List<int> breakPoints)
        {
            InitializeComponent();

            this.breakPoints = breakPoints;
            UpdateBrList();
        }

        void UpdateBrList()
        {
            Lst_BreakPoints.Items.Clear();
            for(int i=0; i<breakPoints.Count;++i)
            {
                Lst_BreakPoints.Items.Add("0x" + breakPoints[i].ToString("X8"));
            }
        }

        private void Btn_Enter_Click(object sender, RoutedEventArgs e)
        {
            AddBreakpoint();
            Txt_Address.Text = "";
            UpdateBrList();
        }

        void AddBreakpoint()
        {
            int address = 0;
            if (Txt_Address.Text.Trim() == "")
                return;

            try
            {
                address = int.Parse(Txt_Address.Text, System.Globalization.NumberStyles.HexNumber);
                if (address % 4 != 0)
                    throw new Exception("Must be a number devisable by 4.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error parsing address:\n" + ex.Message + "\nExample input: 1EE759EA4");
                return;
            }
            bool clear = true;
            for (int i = 0; i < breakPoints.Count; ++i)
                if (breakPoints[i] == address)
                {
                    clear = false;
                    break;
                }
            if (clear)
            {
                breakPoints.Add(address);
                breakPoints.Sort();
            }
        }

        private void Lst_BreakPoints_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            breakPoints.RemoveAt(Lst_BreakPoints.SelectedIndex);
            Lst_BreakPoints.Items.RemoveAt(Lst_BreakPoints.SelectedIndex);

            foreach(int x in breakPoints)
            {
                Console.WriteLine(x);
            }
        }

        private void Btn_Done_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
