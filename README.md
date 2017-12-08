#ARKit-RoomModeling with Unity

Source code that ports the RoomModeling research project to ARKit implemented in Unity.

## Introduction
The unity-arkit-plugin enables using all the functionality of the ARKit SDK within your Unity projects for iOS. The goal of this project was to port the [`3D aware in-situ room modeling`](http://grail.cs.washington.edu/projects/armodeling/) project to iOS by using ARKit and the ARKit Unity Plugin.

## Approach
The intended approach is similar to the technique described in the [paper](http://grail.cs.washington.edu/projects/armodeling/sankar-armodeling.pdf). At a high level, the idea is to extract dominant planes from furniture items, take an image, retrieve closest matching 3D models and place the model in the scene by matching dominant planes and depth points.

## Progress
I was able to set up the development environment and deploy ARKit enabled apps on my phone. With help from the included samples, I was able to extract dominant planes from the scene as shown below.

Separately, I was able to send data from the device to a server via a VPN, where multiple convolutional neural networks are listening for incoming images. Once an image is received the CNN process it and returns a list of candidate 3D models to the client. The client then downloads the top result and places it into the scene.

## Challenges
- 

## Learnings

## Mistakes

## Path Forward

