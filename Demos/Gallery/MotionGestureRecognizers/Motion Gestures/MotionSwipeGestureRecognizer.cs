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
    public class MotionSwipeGestureRecognizer : MotionGestureRecognizer
    {
        private int accelerationCounter;
        private int decelerationCounter;
        public MotionSwipeGestureRecognizerDirection possibleDirections { get; set; }
        public MotionSwipeGestureRecognizerDirection direction { get; set; }
        private IMotionSwipeListener listener;
        public float minimumSwipeThreshold = 0.8f; //defaults to 0.8
        private float swipeMaximum = 1000;


        public override void positionDidUpdate(HandList hands)
        {
            this.hands = hands;

            if (this.hands.Count == this.NumberOfHandsRequired)
            {
                if (isDesiredNumberOfFingersPerHand())
                {
                    MotionAverages averages = averageVectorForHands();

                    if (averages != null)
                    {
                        if (velocityHighEnough(averages))
                        {
                            switch (this.state) {
                            case MotionGestureRecognizerState.MotionGestureRecognizerStatePossible:
                                //Helps with false positives due to change of direction
                                if (accelerationCounter > 3) {
                                    accelerationCounter = 0;
                                    this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateBegan;
                                }
                                else {
                                    accelerationCounter++;
                                }
                                break;
                            case MotionGestureRecognizerState.MotionGestureRecognizerStateBegan:
                                this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateChanged;
                                break;
                            case MotionGestureRecognizerState.MotionGestureRecognizerStateChanged:
                                this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateChanged;
                                break;
                                
                            default:
                                break;
                        }
                        }
                        else
                        {
                            switch (this.state)
                            {
                                case MotionGestureRecognizerState.MotionGestureRecognizerStateBegan:
                                    if (decelerationCounter > 1)
                                    {
                                        decelerationCounter = 0;
                                        this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateEnded;
                                    }
                                    else
                                    {
                                        decelerationCounter++;
                                    }
                                    break;
                                case MotionGestureRecognizerState.MotionGestureRecognizerStateChanged:
                                    if (decelerationCounter > 1)
                                    {
                                        decelerationCounter = 0;
                                        this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateEnded;
                                    }
                                    else
                                    {
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

                        //Callback
                        callback();
                    }

                }
            }
            
        }

        public override void resetValues()
        {
            decelerationCounter = 0;
            accelerationCounter = 0;

            if (this.state == MotionGestureRecognizerState.MotionGestureRecognizerStateChanged)
            {
                this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateEnded;

                //Callback
                //callback();
            }
        }

        private Boolean velocityHighEnough(MotionAverages averages)
        {
            if (this.possibleDirections.HasFlag(MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionRight))
            {
                if (averages.velocityAverage.x < -swipeMaximum * minimumSwipeThreshold)
                {
                    direction = MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionRight;
                    return true;
                }
            }
            if (this.possibleDirections.HasFlag(MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionLeft))
            {
                if (averages.velocityAverage.x > swipeMaximum * minimumSwipeThreshold)
                {
                    direction = MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionLeft;
                    return true;
                }
            }
            if (this.possibleDirections.HasFlag(MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionDown))
            {
                if (averages.velocityAverage.y < -swipeMaximum * minimumSwipeThreshold)
                {
                    direction = MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionDown;
                    return true;
                }
            }
            if (this.possibleDirections.HasFlag(MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionUp))
            {
                if (averages.velocityAverage.y > swipeMaximum * minimumSwipeThreshold)
                {
                    direction = MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionUp;
                    return true;
                }
            }
            if (this.possibleDirections.HasFlag(MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionIn))
            {
                if (averages.velocityAverage.z < -swipeMaximum * minimumSwipeThreshold)
                {
                    direction = MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionIn;
                    return true;
                }
            }
            if (this.possibleDirections.HasFlag(MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionOut))
            {
                if (averages.velocityAverage.z > swipeMaximum * minimumSwipeThreshold)
                {
                    direction = MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionOut;
                    return true;
                }
            }
         

            return false;
        }

        private void callback()
        {
            if (listener != null)
            {
                listener.motionDidSwipe(this);
            }
        }

        public void setMotionSwipeListener(object callingClass)
        {
            listener = (IMotionSwipeListener)callingClass;
            
        }
    }

    //Define Interface
    public interface IMotionSwipeListener
    {
        void motionDidSwipe(MotionSwipeGestureRecognizer recognizer);
    }
}
