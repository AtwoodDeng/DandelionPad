using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureManager : MonoBehaviour {

	public GestureManager() { s_Instance = this; }
	public static GestureManager Instance { get { return s_Instance; } }
	private static GestureManager s_Instance;


	//blow the patels when swipe
	void OnSwipe( SwipeGesture gesture )
    {
        // if (swipeTime <= 0 )
        //    return;
        // swipeTime = swipeTime - 1;
        
		// make sure we started the swipe gesture on our swipe object
		GameObject selection = gesture.StartSelection;  // we use the object the swipe started on, instead of the current one
        
		if (selection == null )
            return;
		SenseGuesture sense = selection.GetComponent<SenseGuesture>();

		if ( sense != null )
			sense.DealSwipe( gesture );

    }

	void OnTap( TapGesture gesture )
	{
		GameObject selection = gesture.StartSelection;  
		if (selection == null )
			return;

		Debug.Log("on Tap select " + selection.name);
		SenseGuesture sense = selection.GetComponent<SenseGuesture>();
		if ( sense != null )
			sense.DealTap( gesture );
		
	}

	void OnFingerDown( FingerDownEvent e )
	{
		GameObject selection = e.Selection;  
		if (selection == null )
			return;

		SenseGuesture sense = selection.GetComponent<SenseGuesture>();
		if ( sense != null )
			sense.DealOnFingerDown( e );

	}

	void OnFingerUp( FingerUpEvent e )
	{
		GameObject selection = e.Selection;  
		if (selection == null )
			return;

		SenseGuesture sense = selection.GetComponent<SenseGuesture>();
		if ( sense != null )
			sense.DealOnFingerUp( e );
	}

	void OnFingerHover( FingerHoverEvent e )
	{
		
		GameObject selection = e.Selection;  
		if (selection == null )
			return;

		SenseGuesture sense = selection.GetComponent<SenseGuesture>();
		if ( sense != null )
			sense.DealOnFingerHover( e );
	}

	void OnFingerMoveBack( FingerMotionEvent e )
	{
		GameObject selection = e.Selection;  
		if (selection == null )
			return;

		SenseGuesture sense = selection.GetComponent<SenseGuesture>();
		if ( sense != null )
			sense.DealOnFingerMotion( e );
	}



}
