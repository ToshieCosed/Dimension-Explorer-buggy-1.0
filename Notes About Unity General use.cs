/*
Basic Unity Development Notes:

X/Y: X is horizontal axis, left or right. X-- = left. X++ = right.
     Y is vertical axis, up or down. Y-- = Down. Y++ = up.

gameobject is always a local reference within any script that is a MonoBehavior.
Example of a MonoBehavior script is:
*/

namespace project{
class yourclass : MonoBehavior {

	public void start
	{

	}

public void update()
{

}

}
}


/*
When you want to load resources in a new Unity project always make sure they exist in the Resources folder.
If the Resources folder doesn't exist in your Unity project folder structure you need to add it at the same root level as your assets folder.
Or I believe you can also put it in Assets/Resources.

In order to load a Texture2D you do this:
*/
Texture2D yourtexname = Resources.Load<Texture2D>("FilePath");

/*
Note: When loading resources this way you don't need to add the folder name or the file extension.

The Unity System works on GameObjects and GameComponents.
Using GetComponent<ComponentType> from within a script that is attached to a Unity GameObject you can get the reference
to just about any active game object's live components such as Sprite, or SpriteRenderer or such as the Box Collider 2D.
Also to note: Typing gameobject locally within any MonoBehavior script will reference the game object directly
which that script is attached to.
*/