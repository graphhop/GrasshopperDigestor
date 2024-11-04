using Eto.Forms;
using Eto.Drawing;

namespace GraphHop.PluginRhino
{
    public class MessageDialog : Dialog
    {
        public MessageDialog(string message, string imagePath)
        {
            Title = "Grasshopper File Summary";
            ClientSize = new Size(600, 800);
            Padding = 10;

            var textBox = new TextArea
            {
                Text = message,
                ReadOnly = true,
                Wrap = true
            };

            // Load the image if imagePath is not null
            Bitmap image = null;
            if (!string.IsNullOrEmpty(imagePath))
            {
                image = new Bitmap(imagePath);
            }

            // Create an ImageView control
            var imageView = new ImageView
            {
                Image = image,
                Size = new Size(200, 200) // Set the size of the ImageView
            };

            var layout = new DynamicLayout();
            if (image != null)
            {
                layout.Add(imageView, yscale: false);
            }
            layout.Add(textBox, yscale: true);

            Content = layout;

            var okButton = new Button { Text = "OK" };
            okButton.Click += (sender, e) => Close();

            DefaultButton = okButton;
            AbortButton = okButton;

            PositiveButtons.Add(okButton);
        }
    }
}
