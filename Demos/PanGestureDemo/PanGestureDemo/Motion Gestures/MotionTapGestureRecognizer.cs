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
using Leap;
using MotionGestures.Enums;

namespace MotionGestures
{
    public class MotionTapGestureRecognizer : MotionGestureRecognizer
    {
        private IMotionTapListener listener;
        public Vector TapPosition;
        public Vector TapVelocity;
        public MotionTapGestureRecognizerDirection PossibleDirections;
        public MotionTapGestureRecognizerDirection Direction;
        private double MotionTapDownBeginYThreshold = -300;
        private double MotionTapDownBeginZThreshold = -100;
        private double MotionTapDownBeginZEndThreshold = -80;

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
		                this.TapPosition = averages.positionAverage;
                        this.TapVelocity = averages.velocityAverage;

                        processTouchVectors(averages);
	                }

                    //Callback
                    callback();
                }
            }
        }

        private void processTouchVectors(MotionAverages averages)
        {
            switch (this.state)
            {
                case MotionGestureRecognizerState.MotionGestureRecognizerStatePossible:
                    if (averages.velocityAverage.y < MotionTapDownBeginYThreshold && averages.velocityAverage.z < MotionTapDownBeginZThreshold)
                    {
                        this.Direction = MotionTapGestureRecognizerDirection.MotionTapGestureRecognizerDirectionDown;
                        this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateBegan;
                    }
                    break;
                case MotionGestureRecognizerState.MotionGestureRecognizerStateBegan:
                    this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateChanged;
                    if (averages.velocityAverage.y > MotionTapDownBeginYThreshold && averages.velocityAverage.z > MotionTapDownBeginZThreshold)
                    {
                        this.Direction = MotionTapGestureRecognizerDirection.MotionTapGestureRecognizerDirectionUp;
                    }
                    break;
                case MotionGestureRecognizerState.MotionGestureRecognizerStateChanged:
                    this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateChanged;

                    if (averages.velocityAverage.y > MotionTapDownBeginZThreshold && averages.velocityAverage.z > MotionTapDownBeginZEndThreshold)
                    {
                        this.Direction = MotionTapGestureRecognizerDirection.MotionTapGestureRecognizerDirectionUp;
                        this.state = MotionGestureRecognizerState.MotionGestureRecognizerStateEnded;
                    }
                    else if (averages.velocityAverage.y > MotionTapDownBeginYThreshold && averages.velocityAverage.z > MotionTapDownBeginZThreshold)
                    {
                        this.Direction = MotionTapGestureRecognizerDirection.MotionTapGestureRecognizerDirectionUp;
                    }
                    break;
                case MotionGestureRecognizerState.MotionGestureRecognizerStateEnded:
                    this.state = MotionGestureRecognizerState.MotionGestureRecognizerStatePossible;
                    break;
                default:
                    break;
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

        private void callback()
        {
            if (listener != null)
            {
                listener.motionDidTap(this);
            }
        }

        public void setMotionTapListener(object callingClass)
        {
            listener = (IMotionTapListener)callingClass;

        }
    }

    //Define Interface
    public interface IMotionTapListener
    {
        void motionDidTap(MotionTapGestureRecognizer recognizer);
    }
}
