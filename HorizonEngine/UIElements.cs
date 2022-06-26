using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace HorizonEngine
{
	class UIElement
	{
		Vector2i Anchor = new Vector2i(0, 0);
		Vector2i Position = new Vector2i(0, 0);
		Vector2i Size = new Vector2i(100, 100);
		Color4 Color = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
	}

	class Text : UIElement
	{
		string TextString = "Text";
	}

	class Slider : UIElement
	{
		Vector2i CursorSize = new Vector2i(10, 10);
		float SliderPosition = 0f;
	}
}
