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
using MotionGestures.Enums;
using Leap;

namespace MotionGestures
{
    public class MotionRotationRecognizer : MotionGestureRecognizer
    {
        private IMotionRotationListener listener;
        private Vector handDirection = new Vector();
        public MotionRotationGestureRecognizerDirection PossibleDirections;
        public MotionRotationGestureRecognizerDirection Direction;
        public int MinimumNumberOfFingersRequired = 3;
        private float MotionMinimumRotationThreshold = 0.01f;
        private int decelerationCounter;

        public override void positionDidUpdate(HandList hands)
        {
            this.hands = hands;

            if (this.hands.Count == this.NumberOfHandsRequired)
            {
                if (totalFingers() >= this.MinimumNumberOfFingersRequired)
                {
                    if (isRotating())
                    {
                        switch (this.state)
                        {
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

        public override void resetValues()
        {
            if (this.state == MotionGestureRecognizerState.MotionGestureRecognizerStateChanged)
            {
                this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateEnded;

                //Callback
                callback();
            }
        }

        private Boolean isRotating()
        {
            if (this.hands[0] != null)
            {
                //Get Direction Vector
                Vector newDirection = this.hands[0].Direction;

                float difference = (100 * newDirection.x) - (100 * handDirection.x);
                float absoluteDifference = Math.Abs(difference);

                if (this.PossibleDirections.HasFlag(MotionRotationGestureRecognizerDirection.MotionRotationGestureRecognizerDirectionClockwise))
                {

                    if (newDirection.x > handDirection.x && absoluteDifference > MotionMinimumRotationThreshold)
                    {
                        Direction = MotionRotationGestureRecognizerDirection.MotionRotationGestureRecognizerDirectionClockwise;
                        handDirection = newDirection;
                        return true;
                    }

                }
                if (this.PossibleDirections.HasFlag(MotionRotationGestureRecognizerDirection.MotionRotationGestureRecognizerDirectionCounterClockwise))
                {
                    if (newDirection.x < handDirection.x && absoluteDifference > MotionMinimumRotationThreshold)
                    {
                        Direction = MotionRotationGestureRecognizerDirection.MotionRotationGestureRecognizerDirectionCounterClockwise;
                        handDirection = newDirection;
                        return true;
                    }

                    handDirection = newDirection;
                }
            }
            return false;
        }

        private int totalFingers()
        {
            int total = 0;
            foreach (Hand hand in this.hands)
            {
                total += hand.Fingers.Count;
            }

            return total;
        }

        private void callback()
        {
            if (listener != null)
            {
                listener.motionDidRotate(this);
            }
        }

        public void setMotionRotationListener(object callingClass)
        {
            listener = (IMotionRotationListener)callingClass;

        }
    }

    //Define Interface
    public interface IMotionRotationListener
    {
        void motionDidRotate(MotionRotationRecognizer recognizer);
    }
}
