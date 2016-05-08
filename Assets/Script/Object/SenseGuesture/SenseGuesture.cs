using UnityEngine;
using System.Collections;

public class SenseGuesture : MonoBehaviour {

	public virtual void DealSwipe( SwipeGesture guesture )
	{
	}

	public virtual void DealTap( TapGesture guesture )
	{
	}

	public virtual void DealOnFingerDown( FingerDownEvent e )
	{
	}
	public virtual void DealOnFingerHover( FingerHoverEvent e )
	{
	}
	public virtual void DealOnFingerUp( FingerUpEvent e )
	{
	}

	public virtual void DealOnFingerMotion( FingerMotionEvent e )
	{}
}
