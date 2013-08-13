//  Copyright (c) 2012 The Board of Trustees of The University of Alabama
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions
//  are met:
//
//  1. Redistributions of source code must retain the above copyright
//  notice, this list of conditions and the following disclaimer.
//  2. Redistributions in binary form must reproduce the above copyright
//  notice, this list of conditions and the following disclaimer in the
//  documentation and/or other materials provided with the distribution.
//  3. Neither the name of the University nor the names of the contributors
//  may be used to endorse or promote products derived from this software
//  without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
//  FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
//  THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
//  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
//  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
//  STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
//  OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotionGestures;
using Leap;
using MotionGestures.Enums;
using System.Windows;

namespace MotionGestures
{

    public class MotionGestureRecognizer
    {
        public int NumberOfFingersPerHandRequired { get; set; }
        public int NumberOfHandsRequired { get; set; }
        public MotionGestureRecognizerState state;
        public long identifier;
        public HandList hands;

        //Private bound variables
        public static double MotionXDomain = 900;
        public static double MotionXMinimum = -450;
        public static double MotionXMaximum = 450;
        public static double MotionYDomain = 500;
        public static double MotionYMinimum = 100;
        public static double MotionYMaximum = 600;
        public static double MotionZDomain = 700;
        public static double MotionZMinimum = -350;
        public static double MotionZMaximum = 350;

        //IMotion leapCoreInterface = new MotionListener();
        public virtual void positionDidUpdate(HandList hands)
        {
            throw new NotImplementedException();
        }

        public virtual void resetValues()
        {

        }

        public static Point locationOfVectorInWindow(Leap.Vector leapVector, Window w, double scaler)
        {
            double XScale = w.RenderSize.Width / MotionXDomain;
            double YScale = w.RenderSize.Height / MotionYDomain;

            //Calculate scaled X value
            double xPosition = 0;
            if (leapVector.x <= 0)
            {
                if (leapVector.x * scaler < MotionXMinimum)
                {
                    xPosition = leapVector.x * scaler - MotionXMinimum;
                }
                else
                {
                    xPosition = (double)Math.Abs(MotionXMinimum - leapVector.x * scaler);
                }
            }
            else
            {
                xPosition = leapVector.x * 2 + MotionXDomain / 2;
            }

            //Calculate scaled Y value
            double yPosition = 0;
            yPosition = leapVector.y;

            double yMidpoint = MotionYDomain / 2;
            double yDifference = yPosition - (yMidpoint); //+ or - version

            yDifference = yDifference * scaler;
            double newY = yDifference + MotionYDomain / 2;

            //Calculate window scaled X and Y
            double windowScaledX = xPosition * XScale;
            double windowScaledY = newY * YScale;

            //Return point
            Point p = new Point(windowScaledX, windowScaledY);
            return p;
        }

        //Registers or reactivates the current recognizer with the subscriber system
        public void startListening()
        {
            if (this.identifier == 0)
            {
                MotionSubscriber subscriber = new MotionSubscriber();
                subscriber.identifier = longRandom();
                subscriber.active = true;
                subscriber.recognizer = this;
                this.identifier = subscriber.identifier;

                MotionSubscriberCenter.Instance.AddSubscriber(subscriber);
            }
            else
            {
                MotionSubscriberCenter.Instance.ActivateSubScriber(identifier);
            }

            MotionSubscriberCenter.Instance.StartListening();
        }

        public void stopListening()
        {
            MotionSubscriberCenter.Instance.DeactivateSubScriber(this.identifier);
        }

        //Checks the finger count of each hand to make sure the desired number of fingers is present
        protected Boolean isDesiredNumberOfFingersPerHand()
        {
            foreach (Hand hand in this.hands)
            {
                if (hand.Fingers.Count != this.NumberOfFingersPerHandRequired)
                {
                    return false;
                }
            }
            return true;
        }

        //Returns the average position from all hands. Useful for things like swipe
        protected MotionAverages averageVectorForHands()
        {
            float totalXPosition = 0;
            float totalYPosition = 0;
            float totalZPosition = 0;
    
            float totalXVelocity = 0;
            float totalYVelocity = 0;
            float totalZVelocity = 0;
    
            int totalNumberOfFingers = 0;
    
            //Collect Totals
            foreach (Hand hand in this.hands)
	        {
		 foreach (Finger finger in hand.Fingers)
	{
		  totalNumberOfFingers += 1;
            
                totalXPosition += finger.TipPosition.x;
                totalYPosition += finger.TipPosition.y;
                totalZPosition += finger.TipPosition.z;
            
                totalXVelocity += finger.TipVelocity.x;
                totalYVelocity += finger.TipVelocity.y;
                totalZVelocity += finger.TipVelocity.z;
	    }
	}
    
    
    //Calculate Averages
    float avgXPos = totalXPosition/totalNumberOfFingers;
    float avgYPos = totalYPosition/totalNumberOfFingers;
    float avgZPos = totalZPosition/totalNumberOfFingers;
    
    float avgXVel = totalXVelocity/totalNumberOfFingers;
    float avgYVel = totalYVelocity/totalNumberOfFingers;
    float avgZVel = totalZVelocity/totalNumberOfFingers;
    
    //Create Average Vectors

    Leap.Vector avgPositionVector = new Leap.Vector(avgXPos, avgYPos, avgZPos);
    Leap.Vector avgVelocityVector = new Leap.Vector(avgXVel, avgYVel, avgZVel);

            //Return averages
    MotionAverages averages = new MotionAverages(avgPositionVector, avgVelocityVector);
            return averages;
        }


        //Creates a long between 1 and a very big number
        long longRandom()
        {
            Random rand = new Random();
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (99999999999999999 - 1)) + 1);
        }
    }
}
