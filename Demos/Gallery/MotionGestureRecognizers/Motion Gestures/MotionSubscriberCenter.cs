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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace MotionGestures
{
    class MotionSubscriberCenter : ILeapCore
    {
        public static volatile MotionSubscriberCenter instance = null;
        private static object syncRoot = new Object();
        private ArrayList Subscribers = new ArrayList();
        private MotionListener listener;


        private MotionSubscriberCenter() { }
       

        public static MotionSubscriberCenter Instance
        {
            get
            {
                if (instance == null)
                {

                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new MotionSubscriberCenter();
                        }
                    }
                }
                return instance;
            }
        }

        // How to call : Motion.StaticMethod(); 
        public static void StaticMethod()
        {

        }


        // How this is called : MotionSingleton.Instance.StartListening();
        public void StartListening()
        {
            if (listener == null)
            {
                listener = new MotionListener();
                listener.run();
            }
        }


        public void StopeListening()
        {
            // Stop 
        }
        public void AddSubscriber(MotionSubscriber subscriberToAdd)
        {
            //If it did not find the subscriber in the list, add it!
            subscriberToAdd.active = true;
            Subscribers.Add(subscriberToAdd);
        }

        public void ActivateSubScriber(long identifier)
        {
            //Check if subscriber is in list already
            foreach (MotionSubscriber subscriber in Subscribers)
            {
                //If it finds the subscriber, make it active
                if (subscriber.identifier.Equals(identifier))
                {
                    subscriber.active = true;
                    return;
                }
            }
        }


        public void DeactivateSubScriber(long identifier)
        {
            //Check if subscriber is in list already
            foreach (MotionSubscriber subscriber in Subscribers)
            {
                //If it finds the subscriber, makes it not active
                if (subscriber.identifier.Equals(identifier))
                {
                    subscriber.active = false;
                    return;
                }
            }
        }

        //Respond to hand updates from the core
        public void positionDidUpdate(HandList hands)
        {
            foreach (MotionSubscriber subscriber in this.Subscribers)
            {
                if (subscriber.active)
                {
                    subscriber.recognizer.positionDidUpdate(hands);
                }
            }
        }

        public void noHands()
        {
            foreach (MotionSubscriber subscriber in this.Subscribers)
            {
                if (subscriber.active)
                {
                    subscriber.recognizer.resetValues();
                }
            }
        }
    }
}
