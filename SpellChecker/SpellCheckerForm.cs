using System;
using System.Windows.Forms;

namespace SpellChecker
{
    public partial class SpellCheckerForm : Form
    {
        public SpellCheckerForm()
        {
            InitializeComponent();
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            textBoxOutput.Clear();
            string text = textBoxInput.Text + "\r\n";
            string[] input = text.Split(new string[] { "===\r" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (input.Length <= 2)
                    throw new IndexOutOfRangeException();

                string dictionary = input[0];
                SpellingCorrection spelling = new SpellingCorrection(dictionary);

                string[] linesToCorrection = input[1].Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                string[] correctedTextLines = spelling.CorrectTextLines(linesToCorrection);

                foreach (string line in correctedTextLines)
                {
                    textBoxOutput.Text += line;
                    textBoxOutput.Text += Environment.NewLine;
                }
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Invalid input", "Error");
            }
            catch (SpellingCorrection.DictionaryException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
    }
}
