# CharacterControl Script
### MoveVector Class
- A static class built for a RigidBody controller which calculates rotated and unrotated vectors based on keyboard inputs and applies them
	- Uses Unity's raw input axis, but could potentially take analog input
- ApplyMovement() makes sure the Rigidbody.AddForce doesn't exceed the MaxMoveMagnitude Property
- Does not follow Microsoft's C# formatting conventions
### FindFloorAverageDistance Function
- Creates a ring of downward raycasts around a point in 3D space, starting at positive Z and going clockwise. Returns the average distance between the raycasts and any colliders.
	- Has an option to toggle an additional raycast downwards from the centre of the point
- Potential use for some kind of inverse kinematics driven character controller
### DoubleTapHandler Class
- An instantiable class that compares the times of the same button being pressed twice
	- You have to instantiate it for each key you are testing
- It sucks
### CreateSpringJoint Function
- Instantiates a springjoint component on the parent
- Sets the springjoint parameters based an input euler rotation


# CameraController Class
- Requires a parent gameobject (as the rotation origin) with a child camera
### MoveToFocus Function
- Lerps camera position to an assigned vector3 position and applies the transform

### RotateByMouse Function
- Stores an XRotation and YRotation for the camera
	- Changes in MouseDelta will change these properties and rotate the camera focus gameobject accordingly
- Shouldn't be used if normal quaternions work for the camera controller
	- ie when you don't need to read the euler angle rotation

### DoCameraShake Function
- Shakes the camera exponentially proportional to the Trauma property, within the bounds of +-ShakeMaxZ, based on perlin noise
	- Then it decrements Trauma by TraumaDecrement * Timescale
- Creates a smooth (as in not random) shake that exponentially decreases over time


# PickupItem Namespace
### PickupItem Class
- Communicates with PlayerPickUpUse on the item's side to pick up and put down the item
	- In this case, it just turns it into a child of the player gameobject, but this code can be easily redone by changing the PickUp and PutDown functions
### PlayerPickupUse Class
- Attached to the player gameobject to pick up and put down a PickupItem
- This script does the raycasting to find a pickup item and calls the PickUp function from it's PickupItem script
- Probably not worth reusing the script. Rework it.
	- The PickupPutdown function is quite redundant
	- Make it interact with the PickupItem class more
### UseItem class
- An abstract class that contains the usage function of a PickupItem
	- Extend this class and override the function Use to add custom functionality to a UseItem

# MenuManagement Namespace
- Oh gosh, it's terrible
- Don't use it
### SlowTime Coroutine
- Lerps Time.timeScale to a desired value over a number of seconds
