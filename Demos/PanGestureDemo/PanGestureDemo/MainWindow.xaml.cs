using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Leap;
using MotionGestures;
using MotionGestures.Enums;


namespace PanGestureDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMotionPanListener
    {

        public MainWindow()
        {
           InitializeComponent();
            
        }

        private void addPanRecognizer()
        {
            MotionPanGetsureRecognizer PanGestureRecognizer = new MotionPanGetsureRecognizer();
            PanGestureRecognizer.setMotionPanListener(this);
            PanGestureRecognizer.NumberOfFingersPerHandRequired = 1;
            PanGestureRecognizer.NumberOfHandsRequired = 1;
            PanGestureRecognizer.startListening();
        }

        public void motionDidPan(MotionPanGetsureRecognizer recognizer)
        {
            if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateBegan)
            {
                System.Console.WriteLine("Pan did Begin");
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateChanged)
            {
                Point newPoint = MotionGestureRecognizer.locationOfVectorInWindow(recognizer.centerPoint, this, 2);
                Thickness t = new Thickness(newPoint.X, newPoint.Y, 100, 100);
                
                mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.mainCanvas.Margin = t));
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateEnded)
            {
                System.Console.WriteLine("Pan did End");
            }
        }

        private void WindowIsLoaded(object sender, RoutedEventArgs e)
        {
            addPanRecognizer();
        }

    
    }
}
