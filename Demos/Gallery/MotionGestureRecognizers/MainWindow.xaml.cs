using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Leap;
using MotionGestures.Enums;

namespace MotionGestures
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMotionSwipeListener, IMotionPanListener, IMotionPinchListener, IMotionRotationListener, IMotionTapListener
    {
        private MotionPanGetsureRecognizer PanGestureRecognizer;
        private MotionSwipeGestureRecognizer SwipeGestureRecognizer;

        public MainWindow()
        {
            //Add listeners
            addPanRecognizer();
            addSwipeRecognizer();
            addPinchRecognizer();
            addTapRecognizer();
            //addRotationRecognizer();
        }

        //*****Recognizer Creation ********

        private void addPanRecognizer()
        {
            PanGestureRecognizer = new MotionPanGetsureRecognizer();
            PanGestureRecognizer.setMotionPanListener(this);
            PanGestureRecognizer.NumberOfFingersPerHandRequired = 1;
            PanGestureRecognizer.NumberOfHandsRequired = 1;
            PanGestureRecognizer.startListening();
        }

        private void addSwipeRecognizer()
        {
            SwipeGestureRecognizer = new MotionSwipeGestureRecognizer();
            SwipeGestureRecognizer.setMotionSwipeListener(this);
            SwipeGestureRecognizer.NumberOfFingersPerHandRequired = 4;
            SwipeGestureRecognizer.NumberOfHandsRequired = 1;
            SwipeGestureRecognizer.possibleDirections = MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionUp | MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionDown | MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionLeft | MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionRight;
            SwipeGestureRecognizer.startListening();
        }

        private void addPinchRecognizer()
        {
            MotionPinchGestureRecognizer pinchRecognizer = new MotionPinchGestureRecognizer();
            pinchRecognizer.setMotionPinchListener(this);
            pinchRecognizer.NumberOfFingersPerHandRequired = 2;
            pinchRecognizer.NumberOfHandsRequired = 1;
            pinchRecognizer.possibleDirections = MotionPinchGestureRecognizerDirection.MotionPinchGestureRecognizerDirectionIn | MotionPinchGestureRecognizerDirection.MotionPinchGestureRecognizerDirectionOut;
            pinchRecognizer.startListening();
        }

        private void addRotationRecognizer()
        {
            MotionRotationRecognizer rotationRecognizer = new MotionRotationRecognizer();
            rotationRecognizer.setMotionRotationListener(this);
            rotationRecognizer.NumberOfHandsRequired = 1;
            rotationRecognizer.MinimumNumberOfFingersRequired = 3;
            rotationRecognizer.PossibleDirections = MotionRotationGestureRecognizerDirection.MotionRotationGestureRecognizerDirectionClockwise | MotionRotationGestureRecognizerDirection.MotionRotationGestureRecognizerDirectionCounterClockwise;
            rotationRecognizer.startListening();
        }

        private void addTapRecognizer()
        {
            MotionTapGestureRecognizer tapRecognizer = new MotionTapGestureRecognizer();
            tapRecognizer.setMotionTapListener(this);
            tapRecognizer.NumberOfFingersPerHandRequired = 1;
            tapRecognizer.NumberOfHandsRequired = 1;
            tapRecognizer.PossibleDirections = MotionTapGestureRecognizerDirection.MotionTapGestureRecognizerDirectionDown | MotionTapGestureRecognizerDirection.MotionTapGestureRecognizerDirectionUp;
            tapRecognizer.startListening();
        }

        //******Handlers***********

        public void motionDidSwipe(MotionSwipeGestureRecognizer recognizer)
        {
            if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateBegan)
            {
                System.Console.WriteLine("Swipe did Begin");
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateChanged)
            {
                //System.Console.WriteLine("Swipe did Change");
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateEnded)
            {
                //Handle swipe end
                if (recognizer.direction == MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionLeft)
                {
                    System.Console.WriteLine("Swipe Left");
                }
                else if (recognizer.direction == MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionRight)
                {
                    System.Console.WriteLine("Swipe Right");
                }
                else if (recognizer.direction == MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionUp)
                {
                    //An example of how to handle multiple gestures of the same type
                    if (recognizer.identifier == SwipeGestureRecognizer.identifier)
                    {
                        System.Console.WriteLine("Swipe Up");
                    }
                }
                else if (recognizer.direction == MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionDown)
                {
                    if (recognizer.identifier == SwipeGestureRecognizer.identifier)
                    {
                        System.Console.WriteLine("Swipe Down");
                    }
                }
            }
        }



        public void motionDidPan(MotionPanGetsureRecognizer recognizer)
        {
            if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateBegan)
            {
                System.Console.WriteLine("Pan did Begin");
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateChanged)
            {
                System.Console.WriteLine("Pan did Change  x:"+recognizer.centerPoint.x+"     y:"+recognizer.centerPoint.y+"     z:"+recognizer.centerPoint.z);
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateEnded)
            {
                System.Console.WriteLine("Pan did End");
            }
        }

        public void motionDidPinch(MotionPinchGestureRecognizer recognizer)
        {
            if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateBegan)
            {
                System.Console.WriteLine("Pinch did Begin");
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateChanged)
            {
                if (recognizer.direction == MotionPinchGestureRecognizerDirection.MotionPinchGestureRecognizerDirectionIn)
                {
                    System.Console.WriteLine("Pinching In");
                }
                else if (recognizer.direction == MotionPinchGestureRecognizerDirection.MotionPinchGestureRecognizerDirectionOut)
                {
                    System.Console.WriteLine("Pinching Out");
                }
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateEnded)
            {
                System.Console.WriteLine("Pinch did End");
            }
        }

        public void motionDidRotate(MotionRotationRecognizer recognizer)
        {
            if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateBegan)
            {
                System.Console.WriteLine("Rotate did Begin");
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateChanged)
            {
                if (recognizer.Direction == MotionRotationGestureRecognizerDirection.MotionRotationGestureRecognizerDirectionClockwise)
                {
                    System.Console.WriteLine("Rotating Clockwise");
                }
                else if (recognizer.Direction == MotionRotationGestureRecognizerDirection.MotionRotationGestureRecognizerDirectionCounterClockwise)
                {
                    System.Console.WriteLine("Rotating Counter Clockwise");
                }
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateEnded)
            {
                System.Console.WriteLine("Rotate did End");
            }
        }

        public void motionDidTap(MotionTapGestureRecognizer recognizer)
        {
            if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateBegan)
            {
                
            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateChanged)
            {

            }
            else if (recognizer.state == MotionGestureRecognizerState.MotionGestureRecognizerStateEnded)
            {
                if (recognizer.Direction == MotionTapGestureRecognizerDirection.MotionTapGestureRecognizerDirectionUp)
                {
                    System.Console.WriteLine("Did touch up");
                }
            }
        }
    }
}
