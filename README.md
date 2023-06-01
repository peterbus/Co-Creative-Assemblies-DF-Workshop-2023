# Co-Creative-Assemblies-DF-Workshop-2023
The provided repository contains a Unity project linked with Rhino Grasshopper file utilising the UDP protocol (User Datagram Protocol). 
The project operates with the ML agents for Unity tool in two possible modes :

1. Co-crete a digital component-driven assembly in an interactive cooperation process between an AI agent and a Masterbuilder. In this case the process is more interactive, intuitive and emergent.
2. Re-create a component-driven assembly previously built by a Masterbuilder (the Player), which is more like imitation of the Masterbuilder. 

The tools required: Unity, ML agent tool for Unity 2.0.1, ML Agents for Python environment 0.30.0, Visual studio, Python framework 3.9 and above, Anaconda, Pytorch 2.0.0, Tensorflow, Rhino/Grasshopper.
Addons for Grasshopper: gHowl, Robots.

The configuration platform then combines deep reinforcement learning approaches, namely Generative adversarial imitation learning (GAIL) and behavioral cloning, utilising the Proximal Policy Optimisation (PPO) algorithm.
Please refer to the configuration_r.yaml file (in the root folder) for more detailed parameters for the agent training setup. You can experiment with those and tune them to obtain possibly different results. 
