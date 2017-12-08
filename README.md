# ARKit-RoomModeling with Unity

Source code that ports the RoomModeling research project to ARKit implemented in Unity.

## Introduction
The unity-arkit-plugin enables using all the functionality of the ARKit SDK within your Unity projects for iOS. The goal of this project was to port the [`3D aware in-situ room modeling`](http://grail.cs.washington.edu/projects/armodeling/) project to iOS by using ARKit and the ARKit Unity Plugin.

## Approach
The intended approach is similar to the technique described in the [paper](http://grail.cs.washington.edu/projects/armodeling/sankar-armodeling.pdf). At a high level, the idea is to extract dominant planes from furniture items, take an image, retrieve closest matching 3D models and place the model in the scene by matching dominant planes and depth points.

## Progress
I was able to set up the development environment and deploy ARKit enabled apps on my phone. With help from the included samples, I was able to extract dominant planes from the scene as shown below.

<img src="https://github.com/adityas/ARKit-RoomModeling/raw/master/img/IMG-9168.JPG" width="300">
<img src="https://github.com/adityas/ARKit-RoomModeling/raw/master/img/IMG-9169.JPG" width="300">

Separately, I was able to send data from the device to a server via a VPN, where multiple convolutional neural networks are listening for incoming images. Once an image is received the CNN process it and returns a list of candidate 3D models to the client. The client then downloads the top result and places it into the scene.

## Challenges
The open-ended nature of this problem lead to a few unexpected challenges during implementation. They are detailed below: 

- **ARKit Raw Image Acceess**: The major blocking issue that took the most time and is yet unresolved is access to the raw image data in ARKit. In order to send an image to the neural networks, a png image needs to be extracted from the device camera, without any rendered content in the view. However, the experimental ARKit Unity plugin does not provide image access at the moment. I tried several workarounds including using Webcamtexture (this would break ARKit tracking), accessing the render texture prior to content rendering, accessing the YUV texture maps in ARVideo. However, none of these paths led to extracting the image-only. Others have reportedly run into this issue as well [[4]](https://forums.developer.apple.com/thread/82527)

- **Recompiling 3D Models for iOS**: Since my prior work was on Android, all the 3D models (order of tens of thousands) on the server had been compiled and optimized for the android platform. For the models to work on iOS, it was required to recompile each model into an iOS asset bundle. This is a parallelizable, but slow process, since each model takes 1-2 mins to load and compile in Unity batch mode. The scripts for conversion have been uploaded to this repo

- **Plane Edges Not Guaranteed**: ARKit extracts estimated planes from the scene, but does not guarantee that the surface boundaries are exact. The [Human Interface Guidelines](https://developer.apple.com/ios/human-interface-guidelines/technologies/augmented-reality/) in fact mention that boundaries are approximate and should not be used for alignment. However, the reconstruction algorithm relies on accurate boundaries. Ideas to solve this issue are described in _Path Forward_

- **Not Owning the Hardware**: An operational, yet practical issue was not owning the requisite hardware to run Xcode 9 and the iOS 11 SDK. Instead, I had to borrow a loaner laptop which needed to be returned a day earlier than the deadline, this made it difficult to solve the blocking issues and complete the project.

## Learnings
Although I wasn't able to achieve my vision of the application, there were several areas in which I believed I learned a lot. 
- **ARKit SDK**: This was my first time using ARKit and it was a lot of fun to develop on this platform. I have a better sense of its capabilities and limitations and can design my projects to fit them better next time around.

- **ARKit Unity Plugin**: The good folks over at Unity have released an experimental plugin that makes ARKit development very easy on Unity. It was interesting to look through the APIs and hook them into my application.

- **Human Interface Guidelines**: Was interesting to go through the [human interface guidelines](https://developer.apple.com/ios/human-interface-guidelines/technologies/augmented-reality/) prescribed by Apple for ARKit. This will inform future user interface choices when designing for the platform.

## Mistakes
I tried to candidly evaluate why the project didn't succeed in the allotted timeframe. My thoughts are as follows: 

- **Depth of Feasibility Study**: My initial feasibility study when selecting the problem was not in-depth enough. I had understood the capabilities of ARKit and presumed that I had access to the requisite data for my app, however, when it came to implementation, finding that the image access was restricted was disappointing. Finding this out earlier may have helped me pick a different problem or approach.

- **Not Anticipating Cross-Platform Issues**: The 3D model asset bundles were not optimized for iOS and had to be recompiled. This was discovered way too late and there was not enough time to convert all the 3D models. At the time of writing, approximately 3,000 out of 15,000 models have been converted, with the rest in process.

- **Starting Sooner**: After the initial feasibility study and some design drafts I put the project on hold for the holidays. The appearance of blocking issues late in the game contributed to the failure. Instead, having the major components working and in place early would have been the right approach. The fine-tuning and polishing could have been done later.

## Path Forward
Although there were some challenges and mistakes, evaluating them candidly have helped formulate a possible path forward for this project:

- **Image Access via modified SDK**: There are a few options for raw image access. First is to use a modified unity plugin that I [discovered on github](https://github.com/StephenGuanqi/Unity-ARKit-Plugin). This repository has made changes to the plugin's source code in the NativeInterface folder, in order to get the captured image CVPixelBuffer from the ARFrame object, as well as the camera pos, timestamp, lightestimation, etc. 

- **Image Access by Hiding Rendered Content**: Another option would be to momentarily (for even just one frame), hide all rendered content, except the ARCamera image. Taking a snapshot of the render texture at this point will capture only the image pixels and leave out any occluding 3D content or UI elements.

- **Plane Edge Refinement**: Since plane edges are not guaranteed, it may be possible to refine the edges by having the user tag points in the scene, or drag existing edges to match the scene more closely. Refined edges are requisite for a good 3D model placement. Automatic edge refinement may be possible by detecting canny edges and grouping them with hough transforms and retrojecting them into the scene. 

## Conclusion
Despite the challenges and mistakes made in the execution of this project, overall there were some good learnings and takeaways and a potential path forward. It was also fun working on a new and exciting platform that is still relatively unexplored. I look forward to solving the blocking challenges and continuing work on the prototype project in the coming days.

## References

- [1] ARKit Human Interface Guidelines: https://developer.apple.com/ios/human-interface-guidelines/technologies/augmented-reality/
- [2] Unity ARKit plugin: https://bitbucket.org/Unity-Technologies/unity-arkit-plugin
- [3] Modified ARKit plugin: https://github.com/StephenGuanqi/Unity-ARKit-Plugin
- [4] ARKit Image Access issue: https://forums.developer.apple.com/thread/82527
- [5] Plane detection: https://blog.markdaws.net/arkit-by-example-part-2-plane-detection-visualization-10f05876d53
- [6] Geometry with ARKit: https://medium.com/@azamsharp/arkit-geometry-materials-nodes-gestures-oh-my-c4bc04e8aadf

