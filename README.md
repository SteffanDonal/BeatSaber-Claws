# Beat Saber - Claws v1.0.0

Custom mod for Beat Saber that shortens sabers from 100cm to 30cm.

## How to Play

1. [Install this mod](https://www.modsaber.org/mod/claws).
2. In the gameplay settings, find the "Claws" toggle and enable it!

The default saber adjustments that come with Claws are setup for Oculus Touch controllers and regular Vive controllers.

It's worth taking some time to adjust your settings so it's comfortable!


## How to Customize

You can modify the settings in `modprefs.ini` whilst the game is running - it is reloaded every time a song starts.

It's usually easiest to enable NoFail and repeatedly make small adjustments until it's comfortable.

If you've made an alternate grip that's comfortable - [submit an issue](https://github.com/SteffanDonal/BeatSaber-Claws/issues/new) and share the settings with everyone!


### Translation

Translation is the offset that the controller is moved - use it to change _where_ the saber is attached on your hand.

Everything you enter is in meters, so you'd enter `0.05` for `5cm`.

```ini
Translation = Left/Right, Down/Up, Back/Forward
```

The numbers you give are for the _left_ hand, they're mirrored for the right!

* **Example**: If you entered `-0.07, 0, 0`, the sabers would be moved 7cm towards the middle of the room. (Negative X = Left)
* **Example**: If you entered `0, 0.04, 0`, the sabers would both move 4cm upwards.


### Rotation

Rotation is for adjustion which direction the sabers point - you want them pointing directly from your wrist to your middle knuckle.

The units here are degrees.

```ini
Rotation = Up/Down, Left/Right, CW/CCW
```

The numbers you give are for the _left_ hand, they're mirrored for the right!

* **Example**: If you entered `0, -10, 0`, the sabers would point 10 degrees outwards from "forward".
* **Example**: If you entered `90, 0, 0`, the sabers would 90 degrees down - towards the floor.
