# HoloLensUDP

[![unstable](http://badges.github.io/stability-badges/dist/unstable.svg)](http://github.com/badges/stability-badges)

Unity prefabs and scripts for UDP communication between Microsoft HoloLens and arbitrary machines.

- **Prefabs**
  - **UDPCommunication**: Foundation prefab that does most of the work. You must have this in the scene and set it up correctly. For most purposes you can get away without modifying this at all.
  - **UDPResponder**: Prefab with UDPResponse script, for responding to incoming messages.
  - **UDPGenerator**: Prefab with UDPGeneration script, for sending messages.
- **Scripts**
  - **UDPCommunication**: Implements UDP communication functionality. For most purposes you shouldn't have to modify this.
   - **UDPResponse**: Implements example behavior for sending UDP messages on each Update. You can modify this, but you can also set its the DataString property externally.
  - **UDPGeneration**: Implements example behavior for responding to incoming UDP messages. You will probably have to modify this.
- **Scenes**
  - **Test**: Test/demo scene that shows how it all comes together.

Derived from [this post by DrNeurosurg on the Windows Mixed Reality Developer Forum](https://forums.hololens.com/discussion/7980/udp-communication-solved).

This is only Unity assets for one HoloLens. You're going to need to have someone else to talk to. Check out [QualisysUDP](https://github.com/mbaytas/QualisysUDP) for talking to a motion capture system, and [MyoUDP](https://github.com/mbaytas/MyoUDP) for talking to Myo armbands.

**These assets do not work inside the Unity editor / HoloLens emulator. They must be deployed on HoloLens device to function.** This is because the HoloLens uses the asynchronous programming features in UWP, which Unity currently does not support, so a lot of functionality comes inside '#if !UNITY_EDITOR' blocks.

## Dependencies

Requires [HoloToolkit-Unity](https://github.com/Microsoft/HoloToolkit-Unity) (v1.5.7), because I can't live without its Build Window.

## Instructions

### ...for People Who Know What They Are Doing

1) Drop in the UDPCommunication prefab. This can be configured as a sender, listener, or both. (I found it preferable to have different instances and set different port numbers for sending and listening.) Listeners and senders will both need an Internal Port number, an External IP, and an External Port, as well as the Send Ping property set to true. Only the listener requires a UDP Event.

2) Drop in a UDPResponder or UDPGenerator prefab. The UDPResponder needs to be hooked up using the UDPEvent property on the UDPCommunication prefab; then you can modify the UDPResponse script to change behavior. The UDPGenerator has a UDPCommGameObject property to hook it up, and a string property you can set; or you could modify its code.

### ...for N00bs

1) Clone/download and copy into own project.

2) Import [HoloToolkit-Unity](https://github.com/Microsoft/HoloToolkit-Unity). If this is the first time you have done this, feel enlightened.

3) Open up the scene called Test..

3) Observe that there are four important game objects in the scene: UDPCommunication_Listener, UDPResponder, UDPCommunication_Sender, and UDPGenerator.

4) Observe that UDPCommunication_Listener and UDPCommunication_Sender are instances of the same UDPCommunication prefab.

5) Read and try to make sense of the previous section. If you fail, Google the following: Unity tutorials, UDP communication.

6) Deploy on device. It will not work inside the Unity editor / HoloLens emulator.

## Notes

- I found it preferable to set up different listeners and senders for different concerns/streams, rather than having only one of each to handle everything.

- You might get non-specific errors when building for the device. These sometimes happen randomly. Just try again.

---

This is derived from my work with [Qualisys AB](http://www.qualisys.com/), the [HAPPERN research group at Koç University](https://happern.ku.edu.tr/), and the [t2i Interaction Laboratory at Chalmers University of Technology](http://t2i.se/).
