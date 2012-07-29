using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestGifanimae
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap animatedImage = new Bitmap(@"C:\Users\gangathara rai\Documents\GitHub\WPF-Apllications\Games\ChokaBhara-Win8style\ChokaBhara-Win8style\bin\Debug\animated_loader.gif");
        bool currentlyAnimating = false;
        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            this.pictureBox1.Image = System.Drawing.Image.FromFile(@"C:\Users\gangathara rai\Documents\GitHub\WPF-Apllications\Games\ChokaBhara-Win8style\ChokaBhara-Win8style\bin\Debug\animated_loader.gif");
        }
        //This method begins the animation.
        public void AnimateImage()
        {
            if (!currentlyAnimating)
            {

                //Begin the animation only once.
               // ImageAnimator.Animate(animatedImage, new EventHandler(this.OnFrameChanged));
                currentlyAnimating = true;
            }
        }
        private void OnFrameChanged(object o, EventArgs e)
        {

            //Force a call to the Paint event handler.

            this.pictureBox1.Image = animatedImage; 
            AnimateImage();

            //Get the next frame ready for rendering.
            ImageAnimator.UpdateFrames();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            AnimateImage();
        }
    }


public class animateImage : Form 
{

    //Create a Bitmpap Object.
    Bitmap animatedImage = new Bitmap(@"C:\Users\gangathara rai\Documents\GitHub\WPF-Apllications\Games\ChokaBhara-Win8style\ChokaBhara-Win8style\bin\Debug\animated_loader.gif");
    bool currentlyAnimating = false;

    //This method begins the animation.
    public void AnimateImage() 
    {
        if (!currentlyAnimating) 
        {

            //Begin the animation only once.
            ImageAnimator.Animate(animatedImage, new EventHandler(this.OnFrameChanged));
            currentlyAnimating = true;
        }
    }

    private void OnFrameChanged(object o, EventArgs e) 
    {

        //Force a call to the Paint event handler.

        AnimateImage();

        //Get the next frame ready for rendering.
        ImageAnimator.UpdateFrames();
    }

    protected override void OnPaint(PaintEventArgs e) 
    {

        //Begin the animation.
        AnimateImage();

        //Get the next frame ready for rendering.
        ImageAnimator.UpdateFrames();

        //Draw the next frame in the animation.
        e.Graphics.DrawImage(this.animatedImage, new Point(0, 0));
    }

    public static void Main() 
    {
        Application.Run(new animateImage());
        
    }
}
}
