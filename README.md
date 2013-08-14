Motion Gestures for C#
=============

This is a drop-in wrapper on top of the classes for interacting with a Leap Motion - it allows for the handling of gesture recognition similar to what you would find in the world of iOS: simple listeners and events abstracted as high as they can go.
 
What you get:
* Pan Gesture
* Swipe Gesture
* Pinch Gesture
* Tap Gesture
* Rotation Gesture

Some things you'll need:
* Leap Motion device
* Download the [Leap Motion SDK](https://developer.leapmotion.com/dashboard) to get the required dlls (more on that in a second)

**Also Available for Mac**

The Motion Gesture Recognizer libraries are also available in Objective-C for mac here: [Motion Gestures for Mac](https://github.com/uacaps/MotionGestureRecognizers-ObjC)

--------------------
## Set-Up ##

There are a few ways to get Motion Gestures into your app, depending on how you want to do it. Please choose method a, b or c for step 1 as you see appropriate.

**Step 1 (a) - Add Motion Gestures Nuget Package (Preferred Method)**

Install the <code>MotionGestures.x.x.x.nupkg</code> Nuget package. This will import the <code>MotionGestures.dll</code> and add it as a reference. It will also add other dependencies associated with MotionGestures (except Leap dlls).

You can also find the nuget package on [Nuget.org](https://www.nuget.org/packages/MotionGestures/)

**Step 1 (b) - Add Motion Gestures dll**

If you don't need access to the code underneath, simply use <code>MotionGestures.dll</code>. Add it to your references by right clicking on "References" -> "Add Reference" and browsing for it. Our dll is dependent on the Leap dlls (Step 2) and the WindowBase framework (WPF).

**Step 1 (c) - Add Classes**

Start off by dragging every file in the **Classes** top-level directory of the repository into your project (or use Cocoapods to handle the dependancy automatically). This includes the following files:

* MotionAverages.cs
* MotionGestureRecognizer.cs
* MotionPanGestureRecognizer.cs
* MotionPinchGestureRecognizer.cs
* MotionRotationGestureRecognizer.cs
* MotionSubscriber.cs
* MotionSubscriberCenter.cs
* MotionSwipeGestureRecognizer.cs
* MotionTapGestureRecognizer.cs
* MotionListener.cs

**Step 2 - Add Leap dlls**

You will also need to drag the following dlls from the leap motion SDK into your project

```csharp
Leap.dll
LeapCSharp.dll
LeapCSharp.NET4.0.dll //Or 3.5, if applicable
```

Also, <code>LeapCSharp.NET4.0.dll</code> will need to be added as a reference, so right click on "References" and select "Add Reference". Then select "Browse.." from the bottom right. Navigate to the dll in your project directory and select "Add". Make sure the newly added dll has a checkmark beside it in the Reference Manager and click "OK".

Now, single click on Leap.dll and look at it's properties in the bottom right of the window. Make sure the build action "Copy to Output Directory" is set to "Copy always". Do the same for LeapCSharp.dll, but not LeapCSharp.NET4.0.dll.

**Step 3 - Set Target Framework**

Set the target framework for your project to .NET 4.0 by double clicking on "Properties" in your solution explorer and selecting ".NET Framework 4" from the "Target Framework" drop down menu under the "Application" tab. Of course if you are using .NET 3.5, you should do this same thing, just for 3.5. Really, just make sure the referenced Leap dll and your target framework are the same.

Now you should be all set!

--------------------
## Creating a Gesture Recognizer / Handler method ##

**Step 1 - Include Classes**

To begin using the different recognizers in your classes just add 

```csharp
using Leap; 
using MotionGestures; 
using MotionGestures.Enums;
```
into the file you want to use the gestures in. For all the provided examples, this is the main window.

**Step 2 - Instantiate Gesture Recognizer**

To intantiate a Motion Gesture Recognizer (We will use a swipe gesture here), type the following:

```csharp
 MotionSwipeGestureRecognizer SwipeGestureRecognizer = new MotionSwipeGestureRecognizer();
            SwipeGestureRecognizer.setMotionSwipeListener(this);
            SwipeGestureRecognizer.NumberOfFingersPerHandRequired = 2;
            SwipeGestureRecognizer.NumberOfHandsRequired = 1;
            SwipeGestureRecognizer.possibleDirections = MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionUp | MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionDown | MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionLeft | MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionRight; //All directions!
            SwipeGestureRecognizer.startListening();
```

This will begin listening for swipe gestures in whatever class it is instantiated. 

**Step 3 - Connect Interface Callback**

You may have noticed that in line 2 of the above code, we said <code>SwipeGestureRecognizer.setMotionSwipeListener(this);</code>, which set the callback to happen the calling class. Each MotionGestureRecognizer has an interface with methods the calling class can implement to handle the detection of gestures. They are listed below:

* IMotionSwipeListener
* IMotionPanListener
* IMotionPinchListener
* IMotionRotationListener
* IMotionTapListener

So, to handle swipe gestures, we will need to implement the <code>IMotionSwipeListener</code> interface. You can then implement it's lone method and fill it with event handling conditions like that below.

```csharp
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
                    System.Console.WriteLine("Swipe Up");
                }
                else if (recognizer.direction == MotionSwipeGestureRecognizerDirection.MotionSwipeGestureRecognizerDirectionDown)
                {
                    System.Console.WriteLine("Swipe Down");
                }
            }
        }
```

If you have any trouble getting things hooked up, we have sample projects in the Demos folder to help see how things should be connected.

And that's it!

--------------------
## Identifiers ##

Not shown in the example handler above is that each recognizer has an identifier property. This is used to distinguish between multiple gestures that implement the same interface method. For instance, if you have a 1 finger swipe and a 4 finger swipe, you can make both gestures class variables and use the following code in the swipe handler to tell them apart.

```csharp
    if (recognizer.identifier == OneFingerSwipeGestureRecognizer.identifier)
        {
             System.Console.WriteLine("One finger swipe!");           
        }
    else if (recognizer.identifier == FourFingerSwipeGestureRecognizer.identifier)
        {
             System.Console.WriteLine("Four finger swipe!");           
        }
```

--------------------
## Extrapolating Location ##

We have provided a static helper method for determining where in a given window a Leap Vector might be. This is particularly helpful for the MotionPanGestureRecognizer, as it allows one to create a virtual mouse or pointer. The method can be found in <code>MotionGestureRecognizer.cs</code> and is called <code>public static Point locationOfVectorInWindow(Leap.Vector leapVector, Window w, double scalar)</code>.

The three parameters are:
* The Leap Vector you want translated to a window point
* The window you would like the point translated to
* A scalar multiplier akin to mouse sensitivity. This controls how much a change in the leap vector affects a change in window points


We have a great demo of how to use this in our demos folder called PanGestureDemo. Be sure to check it out if you want to get a pointer up and running quickly.

--------------------
## Enabling/Disabling Gestures ##

Disabling and enabling gesture is simple! Just call <code>gesture.stopListening();</code> to shut down the gesture and <code>gesture.startListening();</code> to restart it. It's that simple! Now you have the ability to easily build modes of operation based on what is being displayed in a window (i.e. a selection screen may have one set of gestures, whereas a control screen may have another).

--------------------

## License ##

Copyright (c) 2012 The Board of Trustees of The University of Alabama
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

 1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.
 3. Neither the name of the University nor the names of the contributors
    may be used to endorse or promote products derived from this software
    without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
OF THE POSSIBILITY OF SUCH DAMAGE.
