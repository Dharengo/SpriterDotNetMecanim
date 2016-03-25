# SpriterDotNetMecanim
An add-on for SpriterDotNet.Unity that bridges the gap between some of Unity's Mecanim features and SpriterDotNet.

# Usage
-Install SpriterDotNet into your project  
-Drop the SpriterDotNetMecanim folder into your project  
-(re)Import your .scml file  
-Use the created AnimatorController as you normally would in a Unity project  
-You can alter the transition duration of all animations globally by editing that property in the AnimatorAssist component  
-You can also specify transition durations for individual transitions through the AnimatorRelay behaviours within the animation states  
-For transitions from the Any state, use the AnyStateRelay attached to the layer's root statemachine (just click anywhere on the background in the controller)  