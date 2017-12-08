# ARKit-RoomModeling with Unity

Source code that ports the RoomModeling research project to ARKit implemented in Unity.

## Introduction
The unity-arkit-plugin enables using all the functionality of the ARKit SDK within your Unity projects for iOS. The goal of this project was to port the [`3D aware in-situ room modeling`](http://grail.cs.washington.edu/projects/armodeling/) project to iOS by using ARKit and the ARKit Unity Plugin.

## Approach
The intended approach is similar to the technique described in the [paper](http://grail.cs.washington.edu/projects/armodeling/sankar-armodeling.pdf). At a high level, the idea is to extract dominant planes from furniture items, take an image, retrieve closest matching 3D models and place the model in the scene by matching dominant planes and depth points.

## Progress
I was able to set up the development environment and deploy ARKit enabled apps on my phone. With help from the included samples, I was able to extract dominant planes from the scene as shown below.

Separately, I was able to send data from the device to a server via a VPN, where multiple convolutional neural networks are listening for incoming images. Once an image is received the CNN process it and returns a list of candidate 3D models to the client. The client then downloads the top result and places it into the scene.

## Challenges
- *ARKit Raw Image Acceess*: The major blocking issue that took the most time and is yet unresolved is access to the raw image data in ARKit. In order to send an image to the neural networks, a png image needs to be extracted from the device camera, without any rendered content in the view. However, the experimental ARKit Unity plugin does not provide image access at the moment. I tried several workarounds including using Webcamtexture (this would break ARKit tracking), accessing the render texture prior to content rendering, accessing the YUV texture maps in ARVideo. However, none of these paths led to extracting the image-only.

- *Recompiling 3D Models for iOS*: Since my prior work was on Android, all the 3D models (order of tens of thousands) on the server had been compiled and optimized for the android platform. For the models to work on iOS, it was required to recompile each model into an iOS asset bundle. This is a parallelizable, but slow process, since each model takes 1-2 mins to load and compile in Unity batch mode. The scripts for conversion have been uploaded to this repo

- *Plane Edges Not Guaranteed*: ARKit extracts estimated planes from the scene, but does not guarantee that the surface boundaries are exact. The [Human Interface Guidelines](https://developer.apple.com/ios/human-interface-guidelines/technologies/augmented-reality/) in fact mention that boundaries are approximate and should not be used for alignment. However, the reconstruction algorithm relies on accurate boundaries. Ideas to solve this issue are described in _Path Forward_

- *Not Owning the Hardware*: An operational, yet practical issue was not owning the requisite hardware to run Xcode 9 and the iOS 11 SDK. Instead, I had to borrow a loaner laptop which needed to be returned a day earlier than the deadline, 

## Learnings

## Mistakes

## Path Forward

