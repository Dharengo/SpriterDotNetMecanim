using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriterDotNetUnity;

namespace SpriterDotNetMecanim {
	/// <summary>
	/// Bridges communications between AnimatorController and UnitySpriterAnimator.
	/// </summary>
	[RequireComponent (typeof(SpriterDotNetBehaviour), typeof (Animator))]
	public class AnimatorAssist : MonoBehaviour {
		private UnitySpriterAnimator SpriterAnimator;
		private Dictionary<int, string> Animations = new Dictionary<int, string> ();

		/// <summary>
		/// The time it takes in milliseconds to transition from one animation to another.
		/// </summary>
		[Tooltip ("The time it takes in milliseconds to transition from one animation to another.")] 
		public float GlobalTransitionDuration = 500f;

		private IEnumerator Start () {
			do {
				SpriterAnimator = GetComponent<SpriterDotNetBehaviour> ().Animator;
				if (SpriterAnimator != null) break;
				yield return null;
			} while (true);
			foreach (var animation in SpriterAnimator.GetAnimations ())
				Animations.Add (Animator.StringToHash (animation), animation);
		}

		/// <summary>
		/// Not meant to be called manually.
		/// </summary>
		public void Transition (int AnimationID, int LayerID) {
			string animation;
			var length = GlobalTransitionDuration;
			if (Animations.TryGetValue (AnimationID, out animation))
				SpriterAnimator.Transition (animation, length);
		}
	}
}
