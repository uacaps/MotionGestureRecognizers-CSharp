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

namespace MotionGestures
{
    public class MotionPinchGestureRecognizer : MotionGestureRecognizer
    {
        private IMotionPinchListener listener;
        private float distance;
        private int decelerationCounter;
        public MotionPinchGestureRecognizerDirection possibleDirections { get; set; }
        public MotionPinchGestureRecognizerDirection direction { get; set; }
        private float MotionMinimumPinchThreshold = 0.01f;

        public override void positionDidUpdate(HandList hands)
        {
            this.hands = hands;

            if (this.hands.Count == this.NumberOfHandsRequired)
            {
                if (isDesiredNumberOfFingersPerHand())
                {
                    if (isPinching()) {
                    switch (this.state) {
                        case MotionGestureRecognizerState.MotionGestureRecognizerStatePossible:
                            //Helps with false positives due to change of direction
                            this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateBegan;
                            break;
                        case MotionGestureRecognizerState.MotionGestureRecognizerStateBegan:
                            this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateChanged;
                            break;
                        case MotionGestureRecognizerState.MotionGestureRecognizerStateChanged:
                            this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateChanged;
                            break;
                        case MotionGestureRecognizerState.MotionGestureRecognizerStateEnded:
                            this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateBegan;
                            break;
                        default:
                            break;
                    }
                }
                else {
                    switch (this.state) {
                        case MotionGestureRecognizerState.MotionGestureRecognizerStateBegan:
                            if (decelerationCounter > 1) {
                                decelerationCounter = 0;
                                this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateEnded;
                            }
                            else {
                                decelerationCounter++;
                            }
                            break;
                        case MotionGestureRecognizerState.MotionGestureRecognizerStateChanged:
                            if (decelerationCounter > 1) {
                                decelerationCounter = 0;
                                this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateEnded;
                            }
                            else {
                                decelerationCounter++;
                            }
                            break;
                        case MotionGestureRecognizerState.MotionGestureRecognizerStateEnded:
                            this.state = MotionGestureRecognizerState.MotionGestureRecognizerStatePossible;
                            break;
                        default:
                            break;
                    }
                }
                
                //Call back
                    callback();
                }
            }
        
        }

        public override void resetValues()
        {
            if (this.state == MotionGestureRecognizerState.MotionGestureRecognizerStateChanged)
            {
                this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateEnded;

                //Callback
                callback();
            }
        }

        private Boolean isPinching()
        {
            //Get center left point and center right point
    Vector leftPoint = leftPinchPoint();
    Vector rightPoint = rightPinchPoint();
    
    if (leftPoint != null && rightPoint != null) {
        //Calculate new distance
        float newDistance = (float)distanceBetweenPoints(leftPoint, rightPoint);
        
        //Check to see if we are properly pinching
        if (this.possibleDirections.HasFlag(MotionPinchGestureRecognizerDirection.MotionPinchGestureRecognizerDirectionIn))
        {
            if (newDistance < distance && Math.Abs(newDistance-distance) > MotionMinimumPinchThreshold) {
                direction = MotionPinchGestureRecognizerDirection.MotionPinchGestureRecognizerDirectionIn;
                distance = newDistance;
                return true;
            }
        }
        if (this.possibleDirections.HasFlag(MotionPinchGestureRecognizerDirection.MotionPinchGestureRecognizerDirectionOut))
        {
            if (newDistance > distance && Math.Abs(newDistance-distance) > MotionMinimumPinchThreshold) {
                direction = MotionPinchGestureRecognizerDirection.MotionPinchGestureRecognizerDirectionOut;
                distance = newDistance;
                return true;
            }
        }
    }
    
        return false;
        }


        private Vector leftPinchPoint()
        {
            if (this.NumberOfHandsRequired == 2 && this.hands.Count == 2) {
                return avgVectorForHand(hands[0]);
    }
    else {
        if (this.hands[0] != null) {
            Hand hand = hands[0];
            if (hand.Fingers.Count == 2) {
                return hand.Fingers[0].TipPosition;
            }
        }
    }
    
    return null;
        }

        private Vector rightPinchPoint()
        {
            if (this.NumberOfHandsRequired == 2 && hands.Count == 2) {
        return avgVectorForHand(hands[1]);
    }
    else {
        if (hands[0] != null) {
            Hand hand = hands[0];
            if (hand.Fingers.Count == 2) {
                return hand.Fingers[1].TipPosition;
            }
        }
    }
    
    return null;
        }

        private Vector avgVectorForHand(Hand hand)
        {
            float totalXPosition = 0;
            float totalYPosition = 0;
            float totalZPosition = 0;
    
            foreach (Finger finger in hand.Fingers)
	{
		  totalXPosition += finger.TipPosition.x;
            totalYPosition += finger.TipPosition.y;
            totalZPosition += finger.TipPosition.z;
	}
    
    //Calculate Averages
    float avgXPos = totalXPosition/hand.Fingers.Count;
    float avgYPos = totalYPosition/hand.Fingers.Count;
    float avgZPos = totalZPosition/hand.Fingers.Count;
    
    //Return Average Vectors
    Vector avgPositionVector = new Vector(avgXPos, avgYPos, avgZPos);
    return avgPositionVector;
        }

        private double distanceBetweenPoints(Vector leftPoint, Vector rightPoint)
        {
            double dist = Math.Sqrt(Math.Pow((leftPoint.x - rightPoint.x), 2) + Math.Pow((leftPoint.y - rightPoint.y), 2) + Math.Pow((leftPoint.z - rightPoint.z), 2));
            return dist;
        }


        private void callback()
        {
            if (listener != null)
            {
                listener.motionDidPinch(this);
            }
        }

        public void setMotionPinchListener(object callingClass)
        {
            listener = (IMotionPinchListener)callingClass;

        }
    }

    

    //Define Interface
    public interface IMotionPinchListener
    {
        void motionDidPinch(MotionPinchGestureRecognizer recognizer);
    }
}
