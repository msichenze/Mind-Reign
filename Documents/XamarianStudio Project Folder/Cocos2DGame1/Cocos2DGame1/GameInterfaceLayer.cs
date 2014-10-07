using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace MindRain
{
	public class GameInterfaceLayer: CCLayer
	{
		public CCSize screenSize = CCDirector.SharedDirector.WinSize;
		CCLabel VelLabelX = new CCLabel ("SpdX ", "Times New Roman", 24);
		CCLabel VelLabelY = new CCLabel ("SpdY ", "Times New Roman", 24);
		public GameInterfaceLayer ()
		{

			VelLabelX.SetPosition (screenSize.Width-1800, screenSize.Height -800);
			VelLabelY.SetPosition (screenSize.Width-1800, screenSize.Height -840);
			AddChild (VelLabelX,0);
			AddChild (VelLabelY, 0);
		}
		public void updateVelLabel(float velocityX, float velocityY)
		{
			VelLabelX.Text = "SpdX " + (int)velocityX;
			VelLabelY.Text = "SpdY " + (int)velocityY;
		}
	}
}

