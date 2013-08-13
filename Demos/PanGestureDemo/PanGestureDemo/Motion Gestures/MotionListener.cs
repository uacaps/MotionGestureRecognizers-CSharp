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
using System.Linq;
using Leap;
using MotionGestures.Enums;

namespace MotionGestures
{
    class MotionListener: Listener
    {
        private static object SuperLock = new Object();
        private Controller controller;
        private Boolean isListening = false;
        //ILeapCore leapCoreInterface = new MotionListener();

        public void PrintSafeMessage(String message)
        {
            lock (SuperLock)
            {
                Console.WriteLine(message);
            }
        }


        public override void OnConnect(Controller controller)
        {
           
        }

        public override void OnFrame(Controller controller)
        {
            Frame currentFrame = controller.Frame();
            if (!currentFrame.Hands.Empty)
            {
                //Hand firstHand = currentFrame.Hands[0];
                //FingerList fingers = firstHand.Fingers;
                MotionSubscriberCenter.Instance.positionDidUpdate(currentFrame.Hands);
            }
            else
            {
                MotionSubscriberCenter.Instance.noHands();
            }
        }

        public void run()
        {
            if (!isListening)
            {
                isListening = true;
                controller = new Controller();
                controller.AddListener(this);
            }
        }

        public delegate void SwipeEvent(MotionSwipeGestureRecognizerDirection sd);
        public event SwipeEvent LeapSwipe;

        private int fingersCount;

        public void SwipeAction(FingerList fingers, MotionSwipeGestureRecognizerDirection sd)
        {
            /*
            fingersCount = fingers.Count();
            if (fingersCount == 5)
            {
                switch (sd)
                {
                    case SwipeDirection.Left:
                        if (LeapSwipe != null)
                        {
                            LeapSwipe(SwipeDirection.Left);
                        }
                        break;
                    case SwipeDirection.Right:
                        if (LeapSwipe != null)
                        {
                            LeapSwipe(SwipeDirection.Right);
                        }
                        break;
                    case SwipeDirection.Up:
                        if (LeapSwipe != null)
                        {
                            LeapSwipe(SwipeDirection.Up);
                        }
                        break;
                    case SwipeDirection.Down:
                        if (LeapSwipe != null)
                        {
                            LeapSwipe(SwipeDirection.Down);
                        }
                        break;
                }
            }*/
        }
    }

    public interface ILeapCore
    {
        void positionDidUpdate(HandList hands);
    }
}
