# HoloLensUDP

Unity prefabs for UDP communication between Microsoft HoloLens and arbitrary machines.

Derived from [this post by DrNeurosurg on the Windows Mixed Reality Developer Forum](https://forums.hololens.com/discussion/7980/udp-communication-solved).

Requires [HoloToolkit-Unity](https://github.com/Microsoft/HoloToolkit-Unity) (v1.5.7), because I can't live without its Build Window.

This is only Unity assets for one HoloLens. You're going to need to have someone else to talk to.

## Instructions

### ...for People Who Know What They Are Doing

1) Drop in the UDPCommunication prefab. This can be configured as a sender, listener, or both. I found it preferable to have different instances and set different port numbers for sending and listening.

2) Drop in a UDPResponder or UDPSender prefab. The UDPResponder needs to be hooked up using the UDPEvent property on the UDPCommunication prefab; then you can modify the UDPResponse script to change behavior. The UDPGenerator has a UDPCommGameObject property to hook it up, and a string property you can set; or you could modify its code.

### ...for N00bs

1) Clone/download and copy into own project.

2) Import [HoloToolkit-Unity](https://github.com/Microsoft/HoloToolkit-Unity). If this is the first time you have done this, feel enlightened.

3) Open up the scene called Test..

3) Observe that there are four important game objects in the scene: UDPCommunication_Listener, UDPResponder, UDPCommunication_Sender, and UDPGenerator.

4) Observe that UDPCommunication_Listener and UDPCommunication_Sender are instances of the same UDPCommunication prefab.

5) Read and try to make sense of the previous section. If you fail, Google the following: Unity tutorials, UDP communication, HoloLens tutorials.

## Tips

- I found it preferable to set up different listeners and senders for different concerns/streams, rather than having only one of each to handle everything.

This is derived from my work with [Qualisys AB](http://www.qualisys.com/), the [HAPPERN research group at Koç University](https://happern.ku.edu.tr/), and the [t2i Interaction Laboratory at Chalmers University of Technology](http://t2i.se/).
