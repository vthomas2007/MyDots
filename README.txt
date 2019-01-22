“MyDots” was created with Unity 2018.2.17.

The game supports PC inputs. Click and drag to connect the dots and press Escape to end the game.

In the Unity editor, there are numerous attributes that can be tweaked to modify the experience. Some fun ones to play with include:

- WIDTH (Dots Game Manager): Number of columns

- HEIGHT (Dots Game Manager): Number of rows

- Available Colors (Color Pool): Adjustable length array of possible dot colors

- Initial Dot Color Strategy (Dots Game Manager): Reference to a concrete subclass of “BaseDotColorStrategy.” This enables custom logic to be defined for initial dot color assignment. It defaults to “RandomDotColorStrategy,”, which simply picks a random color for each dot.

- Refill Dot Color Strategy (Dots Game Manager): Same as “Initial Dot Color Strategy”, but applies to every dot after the initial set. Also defaults to “RandomDotColorStrategy.”

To customize the dot color strategies:
1) Add a component to “DotsGame” that derives from “BaseDotColorStrategy.” There are a few sample scripts in the “Assets/Scripts/DotColorStrategies” directory you can play with, and it’s easy to write your own!
2) Drag the component into the appropriate attribute in the “Dots Game Manager” (“Initial Dot Color Strategy” and/or “Refill Dot Color Strategy”).

Enjoy!